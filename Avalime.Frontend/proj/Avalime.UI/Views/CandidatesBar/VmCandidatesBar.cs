using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalime.Core.Infra.Log;
using Avalime.Core.Ime;
using Avalime.Core.Keys;
using Avalime.UI.Views.Candidate;
using Avalime.ViewModels;
using Avalonia.Media;
using Avalonia.Threading;
using static Avalime.Core.Keys.KeyChars;
using KS = Avalime.Core.Keys.KeyStates;

namespace Avalime.UI.Views.CandidatesBar;
using Ctx = VmCandidatesBar;

public partial class VmCandidatesBar : ViewModelBase
	, IDisposable
{
	public ISvcIme ImeState{get;}

	readonly EventHandler<IEnumerable<IKeyEvent>> _afterInputHandler;

	public VmCandidatesBar(ISvcIme ImeState){
		this.ImeState = ImeState;
		AppLog.Info($"[Life] VmCandidatesBar ctor vm#{GetHashCode()}");
		_afterInputHandler = (s, e)=>{
			var sw = System.Diagnostics.Stopwatch.StartNew();
			AppLog.Debug($"[Perf] VmCandidatesBar.AfterInput start: {sw.ElapsedMilliseconds}ms");
			var candidates = ImeState.Candidates.Data;
			if(candidates is null){
				Dispatcher.UIThread.Post(() => CandVms.Clear());
				return;
			}
			var highlightedIndex = ImeState.Candidates.HighlightedIndex;
			// 候選欄在 Android hide/show 後若每次整批替換 CandVms，
			// Avalonia 會重建整排候選控件並重新跑一次布局/綁定。
			// 這裡改成原地覆寫 VM，讓候選視圖盡量被復用，避免聯想詞上屏後的額外卡頓。
			Dispatcher.UIThread.Post(() => ApplyCandidates(candidates, highlightedIndex));
			AppLog.Debug($"[Perf] VmCandidatesBar.AfterInput done: {sw.ElapsedMilliseconds}ms, candidates: {candidates.Count}");
		};
		ImeState.AfterInput += _afterInputHandler;
	}

	void ApplyCandidates(IList<ICandidate> candidates, int highlightedIndex){
		// 先裁掉多餘項，再原地更新已有 VM，最後只為新增項創建 VM。
		while(CandVms.Count > candidates.Count){
			CandVms.RemoveAt(CandVms.Count - 1);
		}

		for(var index = 0; index < candidates.Count; index++){
			var vm = index < CandVms.Count ? CandVms[index] : MkCandVm();
			FillCandVm(vm, candidates[index], index, index == highlightedIndex);
			if(index >= CandVms.Count){
				CandVms.Add(vm);
			}
		}
	}

	VmCandidate MkCandVm(){
		var ans = VmCandidate.Mk();
		ans.Background = UiCfg.Inst.CandidateBgColor;
		return ans;
	}

	void FillCandVm(VmCandidate vm, ICandidate cand, int index, bool isHighlighted){
		// Click 也隨當前 index 一併覆寫，避免復用 VM 後還指向舊候選位次。
		vm.Text = cand.Text ?? "";
		vm.Comment = cand.Comment ?? "";
		vm.Index = index;
		vm.Foreground = isHighlighted ? UiCfg.Inst.MainColor : Brushes.White;
		vm.Click = () => {
			var key = IndexToKey(index);
			ImeState.InputSafely([
				new KeyEvent{KeyChar = key, KeyState = KS.Down},
				new KeyEvent{KeyChar = key, KeyState = KS.Up}
			], ex => HandleErr(ex));
		};
	}

	static IKeyChar IndexToKey(int index) => index switch {
		0 => D1,
		1 => D2,
		2 => D3,
		3 => D4,
		4 => D5,
		5 => D6,
		6 => D7,
		7 => D8,
		8 => D9,
		9 => D0,
		_ => D1
	};

	public ObservableCollection<VmCandidate> CandVms{
		get => field;
		set => SetProperty(ref field, value);
	} = [];

	public void Dispose()
	{
		AppLog.Info($"[Life] VmCandidatesBar dispose vm#{GetHashCode()}");
		ImeState.AfterInput -= _afterInputHandler;
	}
}

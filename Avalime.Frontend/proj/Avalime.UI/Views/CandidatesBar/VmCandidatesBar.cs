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
			Dispatcher.UIThread.Post(() => ApplyCandidates(candidates, highlightedIndex));
			AppLog.Debug($"[Perf] VmCandidatesBar.AfterInput done: {sw.ElapsedMilliseconds}ms, candidates: {candidates.Count}");
		};
		ImeState.AfterInput += _afterInputHandler;
	}

	void ApplyCandidates(IList<ICandidate> candidates, int highlightedIndex){
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

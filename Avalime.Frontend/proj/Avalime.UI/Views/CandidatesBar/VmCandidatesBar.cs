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
		_afterInputHandler = (s, e)=>{
			var sw = System.Diagnostics.Stopwatch.StartNew();
			AppLog.Debug($"[Perf] VmCandidatesBar.AfterInput start: {sw.ElapsedMilliseconds}ms");
			var newList = new ObservableCollection<VmCandidate>();
			var candidates = ImeState.Candidates.Data;
			if(candidates is null){return;}
			var highlightedIndex = ImeState.Candidates.HighlightedIndex;
			for(var index = 0; index < candidates.Count; index++){
				var vm = ToCand(candidates[index], index, index == highlightedIndex);
				newList.Add(vm);
			}
			Dispatcher.UIThread.Post(() => CandVms = newList);
			AppLog.Debug($"[Perf] VmCandidatesBar.AfterInput done: {sw.ElapsedMilliseconds}ms, candidates: {newList.Count}");
		};
		ImeState.AfterInput += _afterInputHandler;
	}

	public VmCandidate ToCand(ICandidate cand, int index, bool isHighlighted){
		var ans = VmCandidate.Mk();
		ans.Text = cand.Text ?? "";
		ans.Comment = cand.Comment ?? "";
		ans.Index = index;
		ans.Background = UiCfg.Inst.CandidateBgColor;
		ans.Foreground = isHighlighted ? UiCfg.Inst.MainColor : Brushes.White;
		ans.Click = () => {
			var key = IndexToKey(index);
			ImeState.InputSafely([
				new KeyEvent{KeyChar = key, KeyState = KS.Down},
				new KeyEvent{KeyChar = key, KeyState = KS.Up}
			], ex => HandleErr(ex));
		};
		return ans;
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
		ImeState.AfterInput -= _afterInputHandler;
	}
}

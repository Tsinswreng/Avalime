using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.UI.Views.Candidate;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsInterop;
using Rime.Api;
using static Avalime.Core.Keys.KeyChars;
using KS = Avalime.Core.Keys.KeyStates;

namespace Avalime.UI.Views.candidatesBar;
using Ctx = VmCandidatesBar;

unsafe public partial class VmCandidatesBar : ViewModelBase
{
	public static Ctx Mk(){return new Ctx();}

	public ImeState ImeState{get;set;} = App.SvcP.GetRequiredService<ImeState>();
	public RimeConnectionState RimeConnection{get;set;} = App.SvcP.GetRequiredService<RimeConnectionState>();

	public VmCandidatesBar(){
		ImeState.AfterInput += (s,e)=>{
			var sw = System.Diagnostics.Stopwatch.StartNew();
			System.Diagnostics.Debug.WriteLine($"[Perf] VmCandidatesBar.AfterInput start: {sw.ElapsedMilliseconds}ms");
			var rime = RimeConnection.Setup;
			if(rime is null) return;
			var rimeApi = rime.apiFn;
			var iterrator = new RimeCandidateListIterator();
			if(rimeApi.candidate_list_begin(rime.rimeSessionId, &iterrator) != RimeUtil.True){
				return;
			}
			var newList = new ObservableCollection<VmCandidate>();
			var count = 0;
			const int maxCandidates = 16;
			for(;count < maxCandidates && rimeApi.candidate_list_next(&iterrator) == RimeUtil.True;){
				var ua = ToCand(iterrator.candidate, count);
				newList.Add(ua);
				count++;
			}
			rimeApi.candidate_list_end(&iterrator);
			CandVms = newList; // 一次性替換，只觸發一次 PropertyChanged
			System.Diagnostics.Debug.WriteLine($"[Perf] VmCandidatesBar.AfterInput done: {sw.ElapsedMilliseconds}ms, candidates: {count}");
		};
	}

	public VmCandidate ToCand(in RimeCandidate cand, int index){
		var ans = VmCandidate.Mk();
		ans.Text = ToolCStr.ToCsStr(cand.text)??"";
		ans.Comment = ToolCStr.ToCsStr(cand.comment)??"";
		ans.Index = index;
		ans.Click = () => {
			var key = IndexToKey(index);
			ImeState.Input([
				new KeyEvent{KeyChar = key, KeyState = KS.Down},
				new KeyEvent{KeyChar = key, KeyState = KS.Up}
			]);
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
}

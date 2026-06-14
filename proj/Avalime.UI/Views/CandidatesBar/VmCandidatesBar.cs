using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.UI.Views.Candidate;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsInterop;
using Rime.Api;

namespace Avalime.UI.Views.candidatesBar;
using Ctx = VmCandidatesBar;

unsafe public partial class VmCandidatesBar : ViewModelBase
{
	public static Ctx Mk(){return new Ctx();}

	public static List<Ctx> Samples = [];
	static VmCandidatesBar(){
		{
			var s = Mk();
			for(var i = 0; i < 100; i++){
				s.CandVms.Add(VmCandidate.Samples[0]);
				s.CandVms.Add(VmCandidate.Samples[1]);
			}
			Samples.Add(s);
		}
	}

	public ImeState ImeState{get;set;} = App.SvcP.GetRequiredService<ImeState>();

	public VmCandidatesBar(){
		ImeState.AfterInput += (s,e)=>{
			CandVms.Clear();
			var rime = RimeSetup.Inst;
			var rimeApi = rime.apiFn;
			var iterrator = new RimeCandidateListIterator();
			if(rimeApi.candidate_list_begin(rime.rimeSessionId, &iterrator) != RimeUtil.True){
				return;
			}
			for(;rimeApi.candidate_list_next(&iterrator) == RimeUtil.True;){
				var ua = ToCand(iterrator.candidate);
				CandVms.Add(ua);
			}
		};
	}

	public VmCandidate ToCand(in RimeCandidate cand){
		var ans = VmCandidate.Mk();
		ans.Text = ToolCStr.ToCsStr(cand.text)??"";
		ans.Comment = ToolCStr.ToCsStr(cand.comment)??"";
		return ans;
	}

	public ObservableCollection<VmCandidate> CandVms{
		get => field;
		set => SetProperty(ref field, value);
	} = [];
}

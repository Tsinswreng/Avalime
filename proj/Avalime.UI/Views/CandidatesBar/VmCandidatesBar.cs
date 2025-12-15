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
unsafe public partial class VmCandidatesBar
	:ViewModelBase
{
	public static List<Ctx> Samples = [];
	static VmCandidatesBar(){
		{
			var s = new Ctx();
			Samples.Add(s);
			for(var i = 0; i < 100; i++){
				s.CandVms.Add(VmCandidate.Samples[0]);
				s.CandVms.Add(VmCandidate.Samples[1]);
			}
		}
	}

	public ImeState ImeState{get;set;} = App.SvcP.GetRequiredService<ImeState>();

	public VmCandidatesBar(){
		ImeState.AfterInput += (s,e)=>{
			//G.debug("candidatesBar");//t
			CandVms.Clear();
			var rime = RimeSetup.Inst;
			var rimeApi = rime.apiFn;
			var iterrator = new RimeCandidateListIterator();

			if( rimeApi.candidate_list_begin(rime.rimeSessionId, &iterrator) != RimeUtil.True ){
				//G.debug("no candidates"); //t
				return;
			}
			for(;rimeApi.candidate_list_next(&iterrator)==RimeUtil.True;){
				var ua = ToCand(iterrator.candidate);
				CandVms.Add(ua);
			}
		};
	}


	public VmCandidate ToCand(
		in RimeCandidate cand
	){
		var ans = new VmCandidate();
		ans.Text = ToolCStr.ToCsStr(cand.text)??"";
		ans.Comment = ToolCStr.ToCsStr(cand.comment)??"";
		return ans;
	}


	public ObservableCollection<VmCandidate> CandVms{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = [];



}

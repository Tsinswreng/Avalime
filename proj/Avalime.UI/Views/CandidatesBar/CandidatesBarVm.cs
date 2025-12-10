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
using Ctx = CandidatesBarVm;
unsafe public partial class CandidatesBarVm
	:ViewModelBase
{
	public static List<Ctx> samples = [];
	static CandidatesBarVm(){
		{
			var s = new Ctx();
			samples.Add(s);
			for(var i = 0; i < 100; i++){
				s.candVms.Add(VmCandidate.Samples[0]);
				s.candVms.Add(VmCandidate.Samples[1]);
			}
		}
	}

	public ImeState imeState{get;set;} = App.ServiceProvider.GetRequiredService<ImeState>();

	public CandidatesBarVm(){
		imeState.AfterInput += (s,e)=>{
			//G.debug("candidatesBar");//t
			candVms.Clear();
			var rime = RimeSetup.inst;
			var rimeApi = rime.apiFn;
			var iterrator = new RimeCandidateListIterator();

			if( rimeApi.candidate_list_begin(rime.rimeSessionId, &iterrator) != RimeUtil.True ){
				//G.debug("no candidates"); //t
				return;
			}
			for(;rimeApi.candidate_list_next(&iterrator)==RimeUtil.True;){
				var ua = toCand(iterrator.candidate);
				candVms.Add(ua);
			}
		};
	}


	public VmCandidate toCand(
		in RimeCandidate cand
	){
		var ans = new VmCandidate();
		ans.Text = ToolCStr.ToCsStr(cand.text)??"";
		ans.Comment = ToolCStr.ToCsStr(cand.comment)??"";
		return ans;
	}



	protected ObservableCollection<VmCandidate> _candVms = [];
	public ObservableCollection<VmCandidate> candVms{
		get{return _candVms;}
		set{SetProperty(ref _candVms, value);}
	}



}

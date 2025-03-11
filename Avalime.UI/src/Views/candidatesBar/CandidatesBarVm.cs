using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalime.Core.keys;
using Avalime.UI.views.candidate;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Rime.Api;
using Shr.Interop;

namespace Avalime.UI.views.candidatesBar;
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
				s.candVms.Add(CandidateVm.samples[0]);
				s.candVms.Add(CandidateVm.samples[1]);
			}
		}
	}

	public ImeState imeState{get;set;} = App.ServiceProvider.GetRequiredService<ImeState>();

	public CandidatesBarVm(){
		imeState.onInput += (s,e)=>{
			var rime = RimeSetup.inst;
			var rimeApi = rime.apiFn;
			var iterrator = new RimeCandidateListIterator();
			if( rimeApi.candidate_list_begin(rime.rimeSessionId, &iterrator) != RimeUtil.True ){
				System.Console.WriteLine("no candidates"); //t
				return;
			}
			for(;rimeApi.candidate_list_next(&iterrator)==RimeUtil.True;){
				var ua = toCand(iterrator.candidate);
				candVms.Add(ua);
			}
		};
	}


	public CandidateVm toCand(
		in RimeCandidate cand
	){
		var ans = new CandidateVm();
		ans.text = CStrUtil.cStrToCsStr(cand.text)??"";
		ans.comment = CStrUtil.cStrToCsStr(cand.comment)??"";
		return ans;
	}



	protected ObservableCollection<CandidateVm> _candVms = [];
	public ObservableCollection<CandidateVm> candVms{
		get{return _candVms;}
		set{SetProperty(ref _candVms, value);}
	}



}
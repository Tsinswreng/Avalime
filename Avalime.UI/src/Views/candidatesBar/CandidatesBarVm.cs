using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalime.UI.views.candidate;
using Avalime.ViewModels;

namespace Avalime.UI.views.candidatesBar;
using Ctx = CandidatesBarVm;
public partial class CandidatesBarVm
	:ViewModelBase
{
	public static List<Ctx> samples = [];
	static CandidatesBarVm(){
		{
			var s = new Ctx();
			samples.Add(s);
			s.candVms.Add(CandidateVm.samples[0]);
			s.candVms.Add(CandidateVm.samples[1]);
		}
	}

	protected ObservableCollection<CandidateVm> _candVms = [];
	public ObservableCollection<CandidateVm> candVms{
		get{return _candVms;}
		set{SetProperty(ref _candVms, value);}
	}



}
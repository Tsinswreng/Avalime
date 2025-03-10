using System.Collections;
using System.Collections.Generic;
using Avalime.ViewModels;

namespace Avalime.UI.views.candidate;
using Ctx = CandidateVm;
public class CandidateVm
	:ViewModelBase
{
	public static List<CandidateVm> samples = [];
	static CandidateVm(){
		{
			var s = new Ctx();
			samples.Add(s);
			s.text = "之";
			s.comment = "tɯ";
		}
	}

	protected str _text="";
	public str text{
		get{return _text;}
		set{SetProperty(ref _text, value);}
	}

	protected str? _comment;
	public str? comment{
		get{return _comment;}
		set{SetProperty(ref _comment, value);}
	}

}
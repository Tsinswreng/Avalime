using System.Collections;
using System.Collections.Generic;
using Avalime.ViewModels;

namespace Avalime.UI.Views.Candidate;
using Ctx = VmCandidate;
public class VmCandidate
	:ViewModelBase
{
	public static List<VmCandidate> Samples = [];
	static VmCandidate(){
		{
			var s = new Ctx();
			Samples.Add(s);
			s.Text = "之";
			s.Comment = "tɯ";
		}
		{
			var s = new Ctx();
			Samples.Add(s);
			s.Text = "之前";
			s.Comment = "tɯ dzˁɛn";
		}
	}

	public str Text{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	public str? Comment{
		get{return field;}
		set{SetProperty(ref field, value);}
	}

}

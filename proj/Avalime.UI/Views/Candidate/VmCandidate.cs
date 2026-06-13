using System.Collections.Generic;
using Avalime.ViewModels;

namespace Avalime.UI.Views.Candidate;
using Ctx = VmCandidate;

public class VmCandidate : ViewModelBase
{
	//無參構造器
	protected VmCandidate(){}
	//工廠
	public static Ctx Mk(){return new Ctx();}

	public static List<Ctx> Samples = [];
	static VmCandidate(){
		{
			var s = Mk();
			s.Text = "之";
			s.Comment = "tɯ";
			Samples.Add(s);
		}
		{
			var s = Mk();
			s.Text = "之前";
			s.Comment = "tɯ dzˁɛn";
			Samples.Add(s);
		}
	}

	public str Text{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	public str? Comment{
		get => field;
		set => SetProperty(ref field, value);
	}
}

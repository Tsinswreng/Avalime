using System.Collections.Generic;
using Avalime.ViewModels;
using Avalonia.Media;

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

	public int Index{
		get => field;
		set => SetProperty(ref field, value);
	}

	/// <summary>點擊此候選詞時觸發（由 VmCandidatesBar 設定）</summary>
	public Action? Click{get;set;}

	public str Text{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	public str? Comment{
		get => field;
		set => SetProperty(ref field, value);
	}

	public IBrush Background{
		get => field;
		set => SetProperty(ref field, value);
	} = UiCfg.Inst.CandidateBgColor;
}

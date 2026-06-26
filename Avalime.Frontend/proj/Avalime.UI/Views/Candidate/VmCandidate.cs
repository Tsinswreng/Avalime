using System.Collections.Generic;
using Avalime.ViewModels;
using Avalonia.Media;

namespace Avalime.UI.Views.Candidate;
using Ctx = VmCandidate;

public class VmCandidate : ViewModelBase
{
	protected VmCandidate(){}

	public static Ctx Mk(){return new Ctx();}

	public int Index{
		get => field;
		set => SetProperty(ref field, value);
	}

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

	public IBrush Foreground{
		get => field;
		set => SetProperty(ref field, value);
	} = Brushes.White;

	public double MinWidth{
		get => field;
		set => SetProperty(ref field, value);
	}
}


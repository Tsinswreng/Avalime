using System.Collections.ObjectModel;
using Avalime.ViewModels;

namespace Avalime.UI.Views.ViewRimeLog;

public class VmRimeLog : ViewModelBase
{
	public ObservableCollection<string> Lines { get; }

	public VmRimeLog(RimeLogBuffer RimeLogBuffer){
		Lines = RimeLogBuffer.Lines;
	}
}

using System.Collections.ObjectModel;
using Avalime.Core.Infra;
using Avalime.ViewModels;

namespace Avalime.UI.Views.RimeLog;

public class VmRimeLog : ViewModelBase
{
	public ObservableCollection<string> Lines => Di.GetRSvc<RimeLogBuffer>().Lines;
}

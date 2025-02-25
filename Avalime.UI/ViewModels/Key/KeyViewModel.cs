using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalime.ViewModels.Key;

public partial class KeyViewModel : ViewModelBase {
	[ObservableProperty]
	private string _greeting = "Welcome to Avalonia!";

	[ObservableProperty]
	private string _content = "";
}

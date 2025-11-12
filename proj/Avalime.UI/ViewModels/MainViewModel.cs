using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalime.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}

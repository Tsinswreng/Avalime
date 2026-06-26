using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalime.ViewModels;
using Avalime.UI.Views;
using Avalonia.Markup.Xaml;

namespace Avalime.UI;

public partial class App : Application
{
	public override void Initialize(){
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted(){
		if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop){
// #if DEBUG
// 			this.AttachDeveloperTools();
// #endif
			desktop.MainWindow = new MainWindow{
				DataContext = new MainViewModel(),
				Width = 1920/4,
				MinWidth = 0,
				Height = 1080/4,
				MinHeight = 0,
				ExtendClientAreaToDecorationsHint = true
			};
		}
		else if(ApplicationLifetime is IActivityApplicationLifetime activityLifetime){
			activityLifetime.MainViewFactory = () => new MainView{
				DataContext = new MainViewModel()
			};
		}
		else if(ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform){
			singleViewPlatform.MainView = new MainView{
				DataContext = new MainViewModel()
			};
		}
		base.OnFrameworkInitializationCompleted();
	}
}

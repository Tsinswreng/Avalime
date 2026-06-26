using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
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
			// Windows 桌面端保持標準系統窗口，保留標題欄與拖動能力。
			desktop.MainWindow = new MainWindow{
				DataContext = new MainViewModel(),
				Title = "Avalime",
				Width = 1920/4,
				MinWidth = 0,
				Height = 1080/4,
				MinHeight = 0,
				WindowDecorations = WindowDecorations.Full,
				ExtendClientAreaToDecorationsHint = false
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

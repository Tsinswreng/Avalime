using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Themes.Fluent;
using Avalime.ViewModels;
using Avalime.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Avalime.UI;

public partial class App : Application
{
	public static IServiceProvider SvcP{get; private set;} = null!;
	public static void SetSvcProvider(IServiceProvider SvcP){App.SvcP = SvcP;}

	public static T GetRSvc<T>() where T:class => SvcP.GetRequiredService<T>();

	public override void Initialize(){
		Styles.Add(new FluentTheme());
	}

	public override void OnFrameworkInitializationCompleted(){
#if DEBUG
		this.AttachDeveloperTools();
#endif
		if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop){
			desktop.MainWindow = new MainWindow{
				DataContext = new MainViewModel(),
				Width = 1920/4,
				MinWidth = 0,
				Height = 1080/4,
				MinHeight = 0,
				ExtendClientAreaToDecorationsHint = true
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

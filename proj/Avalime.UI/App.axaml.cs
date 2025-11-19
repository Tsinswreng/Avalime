using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Avalime.ViewModels;
using Avalime.UI.views;
using Avalonia.Controls;
using System;

namespace Avalime.UI;

public partial class App : Application {

	public static IServiceProvider ServiceProvider { get; private set; } = null!;
	public static void ConfigureServices(IServiceProvider serviceProvider){
		ServiceProvider = serviceProvider;
	}

	public override void Initialize() {
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted() {
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			// Avoid duplicate validations from both Avalonia and the CommunityToolkit.
			// More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
			DisableAvaloniaDataAnnotationValidation();
			desktop.MainWindow = new MainWindow {
				DataContext = new MainViewModel()
				,Width = 1920/4
				,MinWidth = 0
				//,MaxWidth = 1920/2

				,Height = 1080/4
				,MinHeight = 0
				//,MaxHeight = 1080/2
				//,SizeToContent = SizeToContent.WidthAndHeight // 自動調整大小
				,SizeToContent = SizeToContent.Manual
			};
		} else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
			singleViewPlatform.MainView = new MainView {
				DataContext = new MainViewModel()
			};
		}

		base.OnFrameworkInitializationCompleted();
	}

	private void DisableAvaloniaDataAnnotationValidation() {
		// Get an array of plugins to remove
		var dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		// remove each entry found
		foreach (var plugin in dataValidationPluginsToRemove) {
			BindingPlugins.DataValidators.Remove(plugin);
		}
	}
}


using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalime.UI.views.candidate;
using Avalime.UI.views.candidatesBar;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Avalime.UI.views;

public partial class MainView : UserControl {
	public MainView() {
		//InitializeComponent();
		Content = new Avalime.UI.views.KeyBoard.KeyBoard();
		//Content = new CandidateView();
		//Content = new CandidatesBar();
	}

	// ObservableCollection<object> ob = [];


	// void test(){

	// 	for(var i = 0; i < 100; i++){
	// 		ob.Add(new Button{Content = i});
	// 	}

	// 	var fenestra = new Window();
	// 	{

	// 	}
	// 	{{
	// 		var grid = new Grid();
	// 		fenestra.Content = grid;
	// 		{
	// 			var o = grid;
	// 			o.RowDefinitions.AddRange([
	// 				new RowDefinition(1, GridUnitType.Star)
	// 				,new RowDefinition(2, GridUnitType.Star)
	// 			]);
	// 		}
	// 		{{
	// 			var itemsControl = new ItemsControl();
	// 			grid.Children.Add(itemsControl);
	// 			{
	// 				var o = itemsControl;
	// 				o.ItemsPanel = new FuncTemplate<Panel?>(()=>{
	// 					return new StackPanel(){
	// 						Orientation = Avalonia.Layout.Orientation.Horizontal
	// 					};
	// 				});
	// 				o.ItemsSource = ob;
	// 			}
	// 			itemsControl.ItemTemplate = new FuncDataTemplate<object>((vm,b)=>{

	// 			});
	// 		}}//~grid
	// 	}}//~fenestra
	// }



}
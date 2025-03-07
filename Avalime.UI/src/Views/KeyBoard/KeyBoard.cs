using System;
using System.Collections.Generic;
using Avalime.UI.Ext;
using Avalime.ViewModels.key;
using Avalime.ViewModels.KeyBoard;
using Avalime.Views.Key;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Avalime.UI.Views.KeyBoard;

public partial class KeyBoard : UserControl{
	public KeyBoard(){
		_style();
		_render();

	}

	public KeyBoardVm? ctx{
		get{return DataContext as KeyBoardVm;}
		set{DataContext = value;}
	}


	protected zero _style(){
		var gridLine = new Style(x=>
			x.Is<Grid>()
		);
		Styles.Add(gridLine);
		{
			var o = gridLine;
			// o.set(
			// 	Grid.ShowGridLinesProperty
			// 	,true
			// );
		}
		return 0;
	}

	public static IList<RowDefinition> Rd_Auto(i32 n){
		var rd = new List<RowDefinition>();
		for(i32 i = 0; i < n; i++){
			rd.Add(new RowDefinition(){Height = GridLength.Star});
		}
		return rd;
	}

	public static IList<ColumnDefinition> Cd_Auto(i32 n){
		var rd = new List<ColumnDefinition>();
		for(i32 i = 0; i < n; i++){
			rd.Add(new ColumnDefinition(){Width = GridLength.Star});
		}
		return rd;
	}

	protected Key _key(str label){
		var k = new Key();
		k.DataContext = new KeyVm(){label = label};
		return k;
	}

	protected zero _render(){
		var container = new Grid();
		Content = container;
		{
			var o = container;
			o.RowDefinitions.AddRange([
				new RowDefinition(){Height = new GridLength(1, GridUnitType.Star)}
				,new RowDefinition(){Height = new GridLength(6, GridUnitType.Star)}
			]);
		}
		{{

			var topBar = new TopBar();
			//var topBar = new TextBlock(){Text = "TopBar"};
			container.Children.Add(topBar);
			{
				var o = topBar;
				Grid.SetRow(o, 0);
			}

			//
			var keysGrid = new Grid();
			container.Children.Add(keysGrid);
			{
				var o = keysGrid;
				Grid.SetRow(o, 1);
				o.RowDefinitions.AddRange(Rd_Auto(6));
			}
			{{//container:Grid
				var row0 = new Grid();
				keysGrid.Children.Add(row0);
				var idx_row0 = 0;
				{
					var o = row0;
					o.ColumnDefinitions.AddRange(Cd_Auto(12));
				}
				{{
					var k=(str label)=>{
						var keyView = _key(label);
						row0.Children.Add(keyView);
						Grid.SetColumn(keyView, idx_row0++);
					};

					k("1");
					k("2");
					k("3");
					k("4");
					k("5");
					k("6");
					k("7");
					k("8");
					k("9");
					k("0");
					k("`");
				}}
				var row1 = new Grid();
				keysGrid.Children.Add(row1);
				var idx_row1 = 0;
				{
					var o = row1;
					o.ColumnDefinitions.AddRange(Cd_Auto(12));
					Grid.SetRow(row1, 1);
				}
				{{//row1:Grid
					//
					var k=(str label)=>{
						var keyView = _key(label);
						row1.Children.Add(keyView);
						Grid.SetColumn(keyView, idx_row1++);
					};
					k("Q");
					k("W");
					k("E");
					k("R");
					k("T");
					k("\\");
					k("U");
					k("I");
					k("O");
					k("P");
					k("Y");
				}}//~row1:Grid
				var row2 = new Grid();
				keysGrid.Children.Add(row2);
				var idx_row2 = 0;
				{
					var o = row2;
					o.ColumnDefinitions.AddRange(Cd_Auto(12));
					Grid.SetRow(row2, 2);
				}
				{{//row2:Grid
					//
					var k=(str label)=>{
						var keyView = _key(label);
						row2.Children.Add(keyView);
						Grid.SetColumn(keyView, idx_row2++);
					};
					//
					k("A");
					k("S");
					k("D");
					k("F");
					k("G");
					k("H");
					k("J");
					k("K");
					k("L");
					k(";");
					k("'");
				}}//~row2:Grid
				var row3 = new Grid();
				keysGrid.Children.Add(row3);
				var idx_row3 = 0;
				{
					var o = row3;
					o.ColumnDefinitions.AddRange(Cd_Auto(12));
					Grid.SetRow(row3, 3);
				}
				{{//row3:Grid
					//
					var k=(str label)=>{
						var keyView = _key(label);
						row3.Children.Add(keyView);
						Grid.SetColumn(keyView, idx_row3++);
					};
					//
					k("Z");
					k("X");
					k("C");
					k("V");
					k("B");
					k("N");
					k("M");
					k(",");
					k(".");
					k("⌫");
					k("⌫");
				}}//~row3:Grid
				var row4 = new Grid();
				keysGrid.Children.Add(row4);
				var idx_row4 = 0;
				{
					var o = row4;
					o.ColumnDefinitions.AddRange(Cd_Auto(12));
					Grid.SetRow(row4, 4);
				}
				{{//row4:Grid
					//
					var k=(str label)=>{
						var keyView = _key(label);
						row4.Children.Add(keyView);
						Grid.SetColumn(keyView, idx_row4++);
					};
					//
					k("數");
					k("$");
					k("↑");
					k("⇪");
					k(" ");
					k(" ");
					k(" ");
					k(" ");
					k(" ");
					k("↲");
					k("↲");
				}}//~row4:Grid
				var row5 = new Grid();
				keysGrid.Children.Add(row5);
				var idx_row5 = 0;
				{
					var o = row5;
					o.ColumnDefinitions.AddRange(Cd_Auto(12));
					Grid.SetRow(row5, 5);
				}

				{{//row5:Grid
					var k=(str label)=>{
						var keyView = _key(label);
						row5.Children.Add(keyView);
						Grid.SetColumn(keyView, idx_row5++);
					};

					k("⌨");
					k("←");
					k("↓");
					k("→");
					k("\\t");
					k("\\t");
					k("⇤");
					k("⇥");
					k("/");
					k("\\");
					k("⇪");
				}}//row5:Grid
			}}//~keysGrid:Grid
		}}//~container:Grid
		return 0;
	}


	// protected Func<str, zero> _geneFn_addKey(str key, Panel panel){
	// 	return (str label)=>{
	// 		var keyView = _key(label);
	// 		panel.Children.Add(keyView);
	// 		Grid.SetColumn(keyView, idx_row0++);
	// 		return 0;
	// 	};
	// }


}
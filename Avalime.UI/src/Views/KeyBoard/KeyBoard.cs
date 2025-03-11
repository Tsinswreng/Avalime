using System;
using System.Collections.Generic;
using Avalime.Core.keys;
using Avalime.UI.Ext;
using Avalime.ViewModels.key;
using Avalime.ViewModels.KeyBoard;
using Avalime.Views.Key;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalime.UI.views.topBar;
using Avalime.UI.views.input;
namespace Avalime.UI.views.KeyBoard;

public partial class KeyBoard : UserControl{
	public KeyBoard(){
		DataContext = new KeyBoardVm();
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

	public i32 colCnt = 11;

	public List<Grid> rows{get;set;}=new List<Grid>(7);
	public i32 idx_keysGrid = 0;
	public i32 idx_container = 0;

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

	protected KeyView _key(str label){
		var k = new KeyView();
		k.DataContext = new KeyVm(){label = label};
		return k;
	}

	protected zero _render(){
		var container = new Grid();
		Content = container;
		{
			var o = container;
			o.RowDefinitions.AddRange([
				//Input:
				new RowDefinition(){Height = new GridLength(1, GridUnitType.Star)}
				//TopBar:
				,new RowDefinition(){Height = new GridLength(2.5, GridUnitType.Star)}
				//Keys:
				,new RowDefinition(){Height = new GridLength(12, GridUnitType.Star)}
			]);
		}
		{{
			var input = new Input();
			container.Children.Add(input);
			{
				var o = input;
				Grid.SetRow(o, idx_container++);
			}

			var topBar = new TopBar();
			//var topBar = new TextBlock(){Text = "TopBar"};
			container.Children.Add(topBar);
			{
				var o = topBar;
				Grid.SetRow(o, idx_container++);
			}

			//
			var keysGrid = new Grid();
			container.Children.Add(keysGrid);
			{
				var o = keysGrid;
				Grid.SetRow(o, idx_container++);
				o.RowDefinitions.AddRange(Rd_Auto(6));
			}
			{{//keysGrid:Grid
				//var row0 = new Grid();
				var row0 = new Grid();
				rows.Add(row0);
				keysGrid.Children.Add(row0);
				var idx_row0 = 0;
				{
					var o = row0;
					o.ColumnDefinitions.AddRange(Cd_Auto(colCnt));
					Grid.SetRow(row0, idx_keysGrid++);
				}
				{{
					var k = (I_KeyChar key)=>{
						var view = kView(key);
						row0.Children.Add(view);
						Grid.SetColumn(view, idx_row0++);
					};
					k(KeyChars.D1);
					k(KeyChars.D2);
					k(KeyChars.D3);
					k(KeyChars.D4);
					k(KeyChars.D5);
					k(KeyChars.D6);
					k(KeyChars.D7);
					k(KeyChars.D8);
					k(KeyChars.D9);
					k(KeyChars.D0);
					k(KeyChars.Grave);
				}}
				var row1 = new Grid();
				keysGrid.Children.Add(row1);
				var idx_row1 = 0;
				{
					var o = row1;
					o.ColumnDefinitions.AddRange(Cd_Auto(colCnt));
					Grid.SetRow(row1, idx_keysGrid++);
				}
				{{//row1:Grid
					//
					// var strK=(str label)=>{
					// 	var keyView = _key(label);
					// 	row1.Children.Add(keyView);
					// 	Grid.SetColumn(keyView, idx_row1++);
					// };
					var k = (I_KeyChar key)=>{
						var view = kView(key);
						row1.Children.Add(view);
						Grid.SetColumn(view, idx_row1++);
					};
					k(KeyChars.q);
					k(KeyChars.w);
					k(KeyChars.e);
					k(KeyChars.r);
					k(KeyChars.t);
					k(KeyChars.y);
					k(KeyChars.u);
					k(KeyChars.i);
					k(KeyChars.o);
					k(KeyChars.p);
					k(KeyChars.SquareBracket_L);
				}}//~row1:Grid
				var row2 = new Grid();
				keysGrid.Children.Add(row2);
				var idx_row2 = 0;
				{
					var o = row2;
					o.ColumnDefinitions.AddRange(Cd_Auto(colCnt));
					Grid.SetRow(row2, idx_keysGrid++);
				}
				{{//row2:Grid
					//
					var k = (I_KeyChar key)=>{
						var view = kView(key);
						row2.Children.Add(view);
						Grid.SetColumn(view, idx_row2++);
					};
					//
					k(KeyChars.a);
					k(KeyChars.s);
					k(KeyChars.d);
					k(KeyChars.f);
					k(KeyChars.g);
					k(KeyChars.h);
					k(KeyChars.j);
					k(KeyChars.k);
					k(KeyChars.l);
					k(KeyChars.Semicolon);
					k(KeyChars.Apostrophe);
				}}//~row2:Grid
				var row3 = new Grid();
				keysGrid.Children.Add(row3);
				var idx_row3 = 0;
				{
					var o = row3;
					o.ColumnDefinitions.AddRange(Cd_Auto(colCnt));
					Grid.SetRow(row3, idx_keysGrid++);
				}
				{{//row3:Grid
					//
					var k = (I_KeyChar key)=>{
						var view = kView(key);
						row3.Children.Add(view);
						Grid.SetColumn(view, idx_row3++);
					};

					k(KeyChars.z);
					k(KeyChars.x);
					k(KeyChars.c);
					k(KeyChars.v);
					k(KeyChars.b);
					k(KeyChars.n);
					k(KeyChars.m);
					k(KeyChars.Comma);
					k(KeyChars.Period);
					// k(KeyChars.Alt_R);
					// k(KeyChars.Alt_R);
					k(KeyChars.Backspace);
					k(KeyChars.Backspace);
				}}//~row3:Grid
				var row4 = new Grid();
				keysGrid.Children.Add(row4);
				var idx_row4 = 0;
				{
					var o = row4;
					o.ColumnDefinitions.AddRange(Cd_Auto(colCnt));
					Grid.SetRow(row4, idx_keysGrid++);
				}
				{{//row4:Grid
					//
					var kStr=(str label)=>{
						var keyView = _key(label);
						row4.Children.Add(keyView);
						Grid.SetColumn(keyView, idx_row4++);
					};
				var k = (I_KeyChar key)=>{
						var view = kView(key);
						row4.Children.Add(view);
						Grid.SetColumn(view, idx_row4++);
					};

					//
					kStr("數");
					k(KeyChars.Dollar);
					k(KeyChars.Up);
					k(KeyChars.Shift_L);
					k(KeyChars.Space);
					k(KeyChars.Space);
					k(KeyChars.Space);
					k(KeyChars.Space);
					k(KeyChars.Space);
					k(KeyChars.Enter);
					k(KeyChars.Enter);
				}}//~row4:Grid
				var row5 = new Grid();
				keysGrid.Children.Add(row5);
				var idx_row5 = 0;
				{
					var o = row5;
					o.ColumnDefinitions.AddRange(Cd_Auto(colCnt));
					Grid.SetRow(row5, idx_keysGrid++);
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

	/// <summary>
	/// 並設imeState等
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	protected KeyView kView(I_KeyChar key){
		var vm = new KeyVm();
		vm.key_click = key;
		vm.imeState = ctx!.imeState;
		var ans = new KeyView();
		ans.DataContext = vm;
		return ans;
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
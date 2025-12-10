using System;
using Avalime.Core.Keys;
using Avalonia.Input;
using Avalonia.Remote.Protocol.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using KS = Avalime.Core.Keys.KeyStates;
namespace Avalime.ViewModels.key;

public partial class KeyVm
	:ViewModelBase
	,IKeyViewModel
{

	public KeyVm(){
		Label = Key_Click?.Name??"";

		Click = () => {
			var state = ImeState as ImeState;//TODO temp
			try{
				state?.Input([
					new KeyEvent{
						Key = Key_Click
						,KeyState = KS.Down
					}
					,new KeyEvent{
						Key = Key_Click
						,KeyState = KS.Up
					}
				]);
				// state?.osKeyProcessor.OnKeyEventAsy(
				// 	new KeyEvent{
				// 		key = key_click,
				// 		keyState = KS.Down,
				// 	}
				// );

			}
			catch (System.Exception e){
				System.Console.WriteLine(e);//TODO
			}
			return 0;
		};
	}

	public Func<zero>? Click{get;set;}
	public Func<zero>? LongPress{get;set;}
	public Func<zero>? SwipeLeft{get;set;}
	public Func<zero>? SwipeDown{get;set;}
	public Func<zero>? SwipeUP{get;set;}
	public Func<zero>? SwipeRight{get;set;}

	public ImeState ImeState{get;set;}//TODO 改用接口

	public IKeyChar Key_Click{
		get{return field;}
		set{
			Label = value.Name??"";
			SetProperty(ref field, value);
		}
	}


	public str Label{
		get => field;
		set => SetProperty(ref field, value);
	}="";



}

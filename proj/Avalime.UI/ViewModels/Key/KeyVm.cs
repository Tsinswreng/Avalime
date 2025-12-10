using System;
using Avalime.Core.Keys;
using Avalonia.Input;
using Avalonia.Remote.Protocol.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using KS = Avalime.Core.Keys.KeyStates;
namespace Avalime.ViewModels.key;

public partial class KeyVm
	:ViewModelBase
	,I_KeyViewModel
{

	public KeyVm(){
		label = key_click?.Name??"";

		click = () => {
			var state = imeState as ImeState;//TODO temp
			try{
				state?.input([
					new KeyEvent{
						key = key_click
						,keyState = KS.Down
					}
					,new KeyEvent{
						key = key_click
						,keyState = KS.Up
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

	public Func<zero>? click{get;set;}
	public Func<zero>? longPress{get;set;}
	public Func<zero>? swipeLeft{get;set;}
	public Func<zero>? swipeDown{get;set;}
	public Func<zero>? swipeUP{get;set;}
	public Func<zero>? swipeRight{get;set;}

	public ImeState imeState{get;set;}//TODO 改用接口

	protected IKeyChar _key_click;
	public IKeyChar key_click{
		get{return _key_click;}
		set{
			label = value.Name??"";
			SetProperty(ref _key_click, value);
		}
	}


	protected str _label="";
	public str label{
		get => _label;
		set => SetProperty(ref _label, value);
	}



}

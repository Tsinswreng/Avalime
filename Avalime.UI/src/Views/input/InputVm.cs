using System.Collections.Generic;
using System.Linq;
using Avalime.Core.keys;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Avalime.UI.views.input;
public class InputVm
	:ViewModelBase
{
	protected str _text = "";
	public str text{
		get{return _text;}
		set{SetProperty(ref _text, value);}
	}
	public ImeState imeState{get;set;} = App.ServiceProvider.GetRequiredService<ImeState>();
	public InputVm(){
		imeState.onInput += (sender, args) => {
			var sb= new List<str>();
			foreach(var keyEvent in args){
				if(keyEvent.keyState.isKeyDown){
					sb.Add(keyEvent.key.name);
				}
			}
			var txt = str.Join("", sb);

			text += txt;
		};
	}

}
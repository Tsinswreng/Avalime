using System;
using Avalime.ViewModels;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Avalime.UI.Views.topBar;

public class VmTopBar : ViewModelBase
{
	public static VmTopBar Mk(){return new VmTopBar();}

	public RimeConnectionState RimeConnection{get;set;} = App.SvcP.GetRequiredService<RimeConnectionState>();

	public str StatusText{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	public VmTopBar(){
		StatusText = RimeConnection.StatusText;
		RimeConnection.PropertyChanged += (_, e) => {
			if(e.PropertyName == nameof(RimeConnectionState.StatusText)){
				StatusText = RimeConnection.StatusText;
			}
		};
	}

	public void ConnectRime(){
		try{
			RimeConnection.Connect();
			StatusText = RimeConnection.StatusText;
		}
		catch(Exception ex){
			var msg = ex.Message;
			RimeConnection.SetError("Rime 連接失敗: " + msg);
			StatusText = RimeConnection.StatusText;
			Dispatcher.UIThread.Post(() => HandleErr(ex));
		}
	}
}

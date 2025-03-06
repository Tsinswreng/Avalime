using System.Runtime.InteropServices;
using Rime.Api;
using static System.Runtime.InteropServices.NativeMemory;
using static System.Runtime.InteropServices.Marshal;
using static Shr.Interop.PtrUtil;
using System;
using Shr.Interop;
namespace Avalime.UI;



unsafe public class RimeSetup
	:IDisposable
{
	~RimeSetup(){
		Dispose(false);
	}
	protected bool _disposed = false;
	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	protected virtual void Dispose(bool disposing) {
		if(_disposed){
			return;
		}
		if(disposing){
			// dispose managed resources
		}
		// dispose unmanaged resources
		//FreeEtNull(&_traits);
		//FreeIfNotNull(_traits);_traits=null;
		//FreeEtNull(ref _traits);//TODO traitsʹ成員 有 byte*、恐未釋放
		_traits = null;
		_disposed = true;
	}

	protected RimeTraits* _traits;

	public RimeTraits* traits{
		get{return _traits;}
	}

	public DelegateRimeApiFn rime{get;protected set;}

	public RimeSetup(){setup();}

	public zero setup(){
		//traits = (RimeTraits*)AllocZeroed((nuint)SizeOf<RimeTraits>());
		_traits = New<RimeTraits>();
		traits->data_size = RimeUtil.dataSize<RimeTraits>();
		traits->user_data_dir = "D:/Program Files/Rime/User_Data".cStr();
		traits->app_name = "rime.avalime".cStr();
		//var rimeApi = RimeApiFn.rime_get_api();
		var rime_get_api = RimeDllLoader.loadFn_rime_get_api("rime");
		var rimeApi = rime_get_api();
		rime = new DelegateRimeApiFn(rimeApi);

		return 0;
	}

}


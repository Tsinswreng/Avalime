

using System.Runtime.InteropServices;
using Rime.Api;
using static System.Runtime.InteropServices.NativeMemory;
using static System.Runtime.InteropServices.Marshal;
using System;


#region RimeTypes
using Bool = System.Int32;
using size_t = nuint;
using RimeSessionId = nuint;
#endregion RimeTypes

using static Tsinswreng.CsInterop.Ptr;
using Tsinswreng.CsInterop;
namespace Avalime.Rime;


unsafe public class RimeSetup
	:IDisposable
{

	protected static RimeSetup? _inst = null;
	public static RimeSetup inst => _inst??= new RimeSetup();
	//TODO test
	public static str dllPath = "D:/ENV/Rime/weasel-0.15.0/rime.dll";
	public PtrMgr ptrMgr = new PtrMgr();


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
			ptrMgr.Dispose();
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

	protected RimeSessionId _rimeSessionId;
	public RimeSessionId rimeSessionId{
		get{return _rimeSessionId;}
	}

	public DelegateRimeApiFn apiFn{get;protected set;}

	public RimeSetup(){
		_setupRimeApi();
		_setupRimeTraits();
		_setupRimeSession();
	}

	protected zero _setupRimeApi(){
		//traits = (RimeTraits*)AllocZeroed((nuint)SizeOf<RimeTraits>());
		//var rimeApi = RimeApiFn.rime_get_api();

		var rime_get_api = RimeDllLoader.loadFn_rime_get_api(dllPath);
		var rimeApi = rime_get_api();
		apiFn = new DelegateRimeApiFn(rimeApi);
		return 0;
	}


	protected zero _setupRimeTraits(){
		_traits = New<RimeTraits>();
		traits->data_size = RimeUtil.dataSize<RimeTraits>();
		traits->user_data_dir = ptrMgr.Str("D:/Program Files/Rime/User_Data");
		traits->app_name = ptrMgr.Str("rime.avalime");
		return 0;
	}

	protected zero _freeRimeTraits(){
		//TODO
		return 0;
	}

	public static str? S(u8* cStr){
		return ToolCStr.ToCsStr(cStr);
	}


	public void on_message(
		void* context_object
		,RimeSessionId session_id
		,byte* message_type
		,byte* message_value
	){
		//var pth = "D:/Program Files/Rime/User_Data/TswG_log";
		Console.WriteLine
		(
			session_id
			+" "+S(message_type)
			+" "+S(message_value)
		);
		//略
	}

	public RimeNotificationHandler rimeNotificationHandler;

	protected zero _setupRimeSession(){
		apiFn.setup(traits);
		rimeNotificationHandler = on_message;
		apiFn.set_notification_handler(
			rimeNotificationHandler
			,null
		);
		apiFn.initialize(null);
		var full_check = RimeUtil.True;
		if(apiFn.start_maintenance(full_check) != RimeUtil.False){
			apiFn.join_maintenance_thread();
		}
		_rimeSessionId = apiFn.create_session();
		return 0;
	}

	//TODO rime.destroy_session(rimeSessionId); rime.finalize();

}



// class MyClass:IDisposable{
// 	bool _Disposed = false;
// 	public void Dispose() {
// 		if(_Disposed){
// 			return;
// 		}
// 		//dispose()
// 	}
// 	~MyClass(){
// 		Dispose();
// 	}
// }

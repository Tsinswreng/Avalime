
using System.Runtime.InteropServices;
using Rime.Api;
using static System.Runtime.InteropServices.NativeMemory;
using static System.Runtime.InteropServices.Marshal;
using System;
using System.Diagnostics;


#region RimeTypes
// using Bool = System.Int32;
// using size_t = nuint;
// using RimeSessionId = nuint;
#endregion RimeTypes

using static Tsinswreng.CsInterop.Ptr;
using Tsinswreng.CsInterop;
using Rime.Api.Types;
namespace Avalime.Rime;


unsafe public class RimeSetup
	:IDisposable
{
	static void LogInfo(str message){ Debug.WriteLine("[AvalimeRime] " + message); }

	public static RimeSetup Inst => field??= new RimeSetup();
	//TODO test
	public static str dllPath = "D:/ENV/Rime/weasel-0.15.0/rime.dll";
	public static str userDataDir = "D:/Program Files/Rime/User_Data";
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

	public RimeApi apiFn{get;protected set;}

	public RimeSetup(){
		LogInfo("RimeSetup ctor begin");
		_setupRimeApi();
		LogInfo("_setupRimeApi done");
		_setupRimeTraits();
		LogInfo("_setupRimeTraits done");
		_setupRimeSession();
		LogInfo("_setupRimeSession done");
	}

	protected zero _setupRimeApi(){
		//traits = (RimeTraits*)AllocZeroed((nuint)SizeOf<RimeTraits>());
		//var rimeApi = RimeApiFn.rime_get_api();
		LogInfo("_setupRimeApi: loading " + dllPath);
		var rime_get_api = RimeDllLoader.loadFn_rime_get_api(dllPath);
		LogInfo("_setupRimeApi: loadFn_rime_get_api done");
		var rimeApi = rime_get_api();
		LogInfo("_setupRimeApi: rime_get_api invoked");
		apiFn = *rimeApi;
		return 0;
	}


	protected zero _setupRimeTraits(){
		LogInfo("_setupRimeTraits: begin");
		_traits = New<RimeTraits>();
		traits->data_size = RimeUtil.DataSize<RimeTraits>();
		traits->user_data_dir = ptrMgr.Str(userDataDir);
		traits->app_name = ptrMgr.Str("rime.avalime");
		LogInfo("_setupRimeTraits: userDataDir=" + userDataDir);
		return 0;
	}

	protected zero _freeRimeTraits(){
		//TODO
		return 0;
	}

	public static str? S(u8* cStr){
		return ToolCStr.ToCsStr(cStr);
	}

	/// <summary>option 改變時觸發。參數: option_name, is_enabled</summary>
	public static event Action<string, bool>? OnOptionChanged;

	public static void on_message(
		void* context_object
		,RimeSessionId session_id
		,byte* message_type
		,byte* message_value
	){
		var type = S(message_type);
		var val = S(message_value);
		Console.WriteLine(session_id+" "+type+" "+val);

		// 檢測 ascii_mode 切換
		if(type == "option" && val is not null){
			if(val == "ascii_mode")
				OnOptionChanged?.Invoke("ascii_mode", true);
			else if(val == "!ascii_mode")
				OnOptionChanged?.Invoke("ascii_mode", false);
		}
	}

	public RimeNotificationHandler ManagedRimeNotificationHandler = on_message;

	protected zero _setupRimeSession(){
		LogInfo("_setupRimeSession: apiFn.setup");
		apiFn.setup(traits);
		ManagedRimeNotificationHandler = on_message;
		LogInfo("_setupRimeSession: set_notification_handlerManaged");
		apiFn.set_notification_handlerManaged(
			ManagedRimeNotificationHandler
			,null
		);
		LogInfo("_setupRimeSession: initialize");
		apiFn.initialize(null);
		var full_check = RimeUtil.True;
		LogInfo("_setupRimeSession: start_maintenance");
		if(apiFn.start_maintenance(full_check) != RimeUtil.False){
			LogInfo("_setupRimeSession: join_maintenance_thread");
			apiFn.join_maintenance_thread();
		}
		LogInfo("_setupRimeSession: create_session");
		_rimeSessionId = apiFn.create_session();
		LogInfo("_setupRimeSession: session=" + _rimeSessionId);
		return 0;
	}

	//TODO rime.destroy_session(rimeSessionId); rime.finalize();


}

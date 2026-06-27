using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalime.UI;

/// `Ime` 界面的共享 UI 狀態。由根 `VmIme` 與工具欄/剪貼板等子模塊共用。
public partial class ImeUiState : ObservableObject
{
	/// <summary>
	/// 用戶主動點擊工具欄後的日誌頁顯示狀態。
	/// </summary>
	bool _IsRimeLogPinnedVisible;
	public bool IsRimeLogPinnedVisible{
		get{return _IsRimeLogPinnedVisible;}
		set{
			if(SetProperty(ref _IsRimeLogPinnedVisible, value)){
				OnPropertyChanged(nameof(IsRimeLogVisible));
			}
		}
	}

	/// <summary>
	/// 系統在“尚未完成初始化”等場景下強制要求顯示日誌頁的狀態。
	/// </summary>
	bool _IsRimeLogForcedVisible;
	public bool IsRimeLogForcedVisible{
		get{return _IsRimeLogForcedVisible;}
		set{
			if(SetProperty(ref _IsRimeLogForcedVisible, value)){
				OnPropertyChanged(nameof(IsRimeLogVisible));
			}
		}
	}

	bool _IsCandidateCommentVisible;
	public bool IsCandidateCommentVisible{
		get{return _IsCandidateCommentVisible;}
		set{SetProperty(ref _IsCandidateCommentVisible, value);}
	}

	bool _IsClipboardVisible;
	public bool IsClipboardVisible{
		get{return _IsClipboardVisible;}
		set{SetProperty(ref _IsClipboardVisible, value);}
	}

	public bool IsRimeLogVisible{
		get{return IsRimeLogPinnedVisible || IsRimeLogForcedVisible;}
	}

	public void ToggleClipboard()
	{
		if(!IsClipboardVisible){
			IsRimeLogPinnedVisible = false;
		}
		IsClipboardVisible = !IsClipboardVisible;
	}

	public void ExitClipboard()
	{
		IsClipboardVisible = false;
	}

	public void ToggleRimeLog()
	{
		if(!IsRimeLogPinnedVisible){
			IsClipboardVisible = false;
		}
		IsRimeLogPinnedVisible = !IsRimeLogPinnedVisible;
	}

	public void ExitRimeLog()
	{
		IsRimeLogPinnedVisible = false;
		IsRimeLogForcedVisible = false;
	}

	public void SetForcedRimeLogVisible(bool Value)
	{
		IsRimeLogForcedVisible = Value;
	}

	public void ToggleCandidateComment()
	{
		IsCandidateCommentVisible = !IsCandidateCommentVisible;
	}
}

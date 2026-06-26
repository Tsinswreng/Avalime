using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalime.UI;

/// `Ime` 界面的共享 UI 狀態。由根 `VmIme` 與工具欄/剪貼板等子模塊共用。
public partial class ImeUiState : ObservableObject
{
	public bool IsCandidateCommentVisible{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool IsClipboardVisible{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool IsRimeLogVisible{
		get => field;
		set => SetProperty(ref field, value);
	}

	public void ToggleClipboard()
	{
		if(!IsClipboardVisible){
			IsRimeLogVisible = false;
		}
		IsClipboardVisible = !IsClipboardVisible;
	}

	public void ExitClipboard()
	{
		IsClipboardVisible = false;
	}

	public void ToggleRimeLog()
	{
		if(!IsRimeLogVisible){
			IsClipboardVisible = false;
		}
		IsRimeLogVisible = !IsRimeLogVisible;
	}

	public void ExitRimeLog()
	{
		IsRimeLogVisible = false;
	}

	public void ToggleCandidateComment()
	{
		IsCandidateCommentVisible = !IsCandidateCommentVisible;
	}
}

namespace Avalime.UI.Views.Key;
using Tsinswreng.CsCore;

[Doc(@$"輸入法觸摸鍵盤上的按鍵")]
public interface IViewKey{
	[Doc(@$"單擊")]
	public Task<nil> Click(CT Ct);

	[Doc(@$"長按")]
	public Task<nil> LongClick(CT Ct);


	[Doc(@$"上滑")]
	public Task<nil> SwipeUp(CT Ct);

	[Doc(@$"下滑")]
	public Task<nil> SwipeDown(CT Ct);

	[Doc(@$"左滑")]
	public Task<nil> SwipeLeft(CT Ct);

	[Doc(@$"右滑")]
	public Task<nil> SwipeRight(CT Ct);
}

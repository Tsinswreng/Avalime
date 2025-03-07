using System;

namespace Avalime.UI.controls.IF;

public interface I_LongPressBtn{
	public event EventHandler? LongPressed;
	public i64 longPressDurationMs{get;set;}
}
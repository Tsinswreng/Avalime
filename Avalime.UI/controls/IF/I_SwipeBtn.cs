using System;

namespace Avalime.UI.controls.IF;

public interface I_SwipeBtn{
	public event EventHandler<SwipeEventArgs> Swipe;
	public f64 SwipeThreshold{get;set;}

}


public class SwipeEventArgs : EventArgs {
	public SwipeDirection Direction { get; }

	public SwipeEventArgs(SwipeDirection direction) {
		Direction = direction;
	}
}

public enum SwipeDirection {
	Up,
	Down,
	Left,
	Right
}

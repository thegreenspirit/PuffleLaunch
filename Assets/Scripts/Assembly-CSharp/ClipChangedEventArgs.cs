using System;

public class ClipChangedEventArgs : EventArgs
{
	public ClipChangedEventArgs(SpriteClip previous, SpriteClip current)
	{
		this.previous = previous;
		this.current = current;
	}

	public SpriteClip previous;

	public SpriteClip current;
}

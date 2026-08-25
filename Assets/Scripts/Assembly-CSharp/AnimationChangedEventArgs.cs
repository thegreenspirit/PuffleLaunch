using System;

public class AnimationChangedEventArgs : EventArgs
{
	public AnimationChangedEventArgs(SpriteAnimation a)
	{
		this.anim = a;
	}

	public SpriteAnimation anim;
}

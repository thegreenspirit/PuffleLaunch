using System;

public class FrameChangedEventArgs : EventArgs
{
	public FrameChangedEventArgs(string n, int f)
	{
		this.name = n;
		this.frame = f;
	}

	public string name;

	public int frame;
}

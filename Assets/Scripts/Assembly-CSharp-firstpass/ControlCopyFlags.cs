using System;

public enum ControlCopyFlags
{
	Appearance = 1,
	Sound,
	Text = 4,
	Data = 8,
	State = 16,
	Transitions = 32,
	Invocation = 64,
	Settings = 128,
	DragDrop = 256,
	All = 65535
}

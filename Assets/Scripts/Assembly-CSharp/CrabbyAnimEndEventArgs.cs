using System;

public class CrabbyAnimEndEventArgs : EventArgs
{
	public CrabbyAnimEndEventArgs(CrabbyAnimController.CrabbyAnim aAnim)
	{
		this.anim = aAnim;
	}

	public CrabbyAnimController.CrabbyAnim anim;
}

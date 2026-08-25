using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Management/UI Manager")]
public class BHUIManager : UIManager
{
	public override void Awake()
	{
		this.pointerType = UIManager.POINTER_TYPE.TOUCHPAD;
		base.Awake();
	}
}

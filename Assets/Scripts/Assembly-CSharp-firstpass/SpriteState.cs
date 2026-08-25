using System;
using UnityEngine;

[Serializable]
public class SpriteState
{
	public SpriteState(string n, string p)
	{
		this.name = n;
		this.imgPath = p;
	}

	public string name;

	[HideInInspector]
	public string imgPath;

	[HideInInspector]
	public CSpriteFrame frameInfo;
}

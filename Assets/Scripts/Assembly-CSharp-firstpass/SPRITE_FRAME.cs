using System;
using UnityEngine;

[Serializable]
public struct SPRITE_FRAME
{
	public SPRITE_FRAME(float dummy)
	{
		this.uvs = new Rect(0f, 0f, 1f, 1f);
		this.scaleFactor = new Vector2(0.5f, 0.5f);
		this.topLeftOffset = new Vector2(-1f, 1f);
		this.bottomRightOffset = new Vector2(1f, -1f);
	}

	public void Copy(CSpriteFrame f)
	{
		this.uvs = f.uvs;
		this.scaleFactor = f.scaleFactor;
		this.topLeftOffset = f.topLeftOffset;
		this.bottomRightOffset = f.bottomRightOffset;
	}

	public Rect uvs;

	public Vector2 scaleFactor;

	public Vector2 topLeftOffset;

	public Vector2 bottomRightOffset;
}

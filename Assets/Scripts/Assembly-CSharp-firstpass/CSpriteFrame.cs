using System;
using UnityEngine;

[Serializable]
public class CSpriteFrame
{
	public CSpriteFrame()
	{
	}

	public CSpriteFrame(CSpriteFrame f)
	{
		this.Copy(f);
	}

	public CSpriteFrame(SPRITE_FRAME f)
	{
		this.Copy(f);
	}

	public void Copy(SPRITE_FRAME f)
	{
		this.uvs = f.uvs;
		this.scaleFactor = f.scaleFactor;
		this.topLeftOffset = f.topLeftOffset;
		this.bottomRightOffset = f.bottomRightOffset;
	}

	public void Copy(CSpriteFrame f)
	{
		this.uvs = f.uvs;
		this.scaleFactor = f.scaleFactor;
		this.topLeftOffset = f.topLeftOffset;
		this.bottomRightOffset = f.bottomRightOffset;
	}

	public void CopyToSmall(SPRITE_FRAME f)
	{
		this.uvsSmall = f.uvs;
		this.scaleFactorSmall = f.scaleFactor;
		this.topLeftOffsetSmall = f.topLeftOffset;
		this.bottomRightOffsetSmall = f.bottomRightOffset;
	}

	public void CopyFromSmall()
	{
		this.uvs = this.uvsSmall;
		this.scaleFactor = this.scaleFactorSmall;
		this.topLeftOffset = this.topLeftOffsetSmall;
		this.bottomRightOffset = this.bottomRightOffsetSmall;
	}

	public void CopyToLarge(SPRITE_FRAME f)
	{
		this.uvsLarge = f.uvs;
		this.scaleFactorLarge = f.scaleFactor;
		this.topLeftOffsetLarge = f.topLeftOffset;
		this.bottomRightOffsetLarge = f.bottomRightOffset;
	}

	public void CopyFromLarge()
	{
		this.uvs = this.uvsLarge;
		this.scaleFactor = this.scaleFactorLarge;
		this.topLeftOffset = this.topLeftOffsetLarge;
		this.bottomRightOffset = this.bottomRightOffsetLarge;
	}

	public SPRITE_FRAME ToStruct()
	{
		SPRITE_FRAME sprite_FRAME;
		sprite_FRAME.uvs = this.uvs;
		sprite_FRAME.scaleFactor = this.scaleFactor;
		sprite_FRAME.topLeftOffset = this.topLeftOffset;
		sprite_FRAME.bottomRightOffset = this.bottomRightOffset;
		return sprite_FRAME;
	}

	public Rect uvs;

	public Rect uvsSmall;

	public Rect uvsLarge;

	public Vector2 scaleFactor = new Vector2(0.5f, 0.5f);

	public Vector2 scaleFactorSmall = new Vector2(0.5f, 0.5f);

	public Vector2 scaleFactorLarge = new Vector2(0.5f, 0.5f);

	public Vector2 topLeftOffset = new Vector2(-1f, 1f);

	public Vector2 topLeftOffsetSmall = new Vector2(-1f, 1f);

	public Vector2 topLeftOffsetLarge = new Vector2(-1f, 1f);

	public Vector2 bottomRightOffset = new Vector2(1f, -1f);

	public Vector2 bottomRightOffsetSmall = new Vector2(1f, -1f);

	public Vector2 bottomRightOffsetLarge = new Vector2(1f, -1f);
}

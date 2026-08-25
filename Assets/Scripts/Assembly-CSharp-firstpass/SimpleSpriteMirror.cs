using System;
using UnityEngine;

public class SimpleSpriteMirror : SpriteRootMirror
{
	public override void Mirror(SpriteRoot s)
	{
		base.Mirror(s);
		this.lowerLeftPixel = ((SimpleSprite)s).lowerLeftPixel;
		this.pixelDimensions = ((SimpleSprite)s).pixelDimensions;
	}

	public override bool DidChange(SpriteRoot s)
	{
		if (base.DidChange(s))
		{
			return true;
		}
		if (((SimpleSprite)s).lowerLeftPixel != this.lowerLeftPixel)
		{
			s.uvsInitialized = false;
			return true;
		}
		if (((SimpleSprite)s).pixelDimensions != this.pixelDimensions)
		{
			s.uvsInitialized = false;
			return true;
		}
		return false;
	}

	public Vector2 lowerLeftPixel;

	public Vector2 pixelDimensions;
}

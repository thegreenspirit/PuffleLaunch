using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChar
{
	public float GetKerning(int prevChar)
	{
		if (this.kernings == null)
		{
			return 0f;
		}
		float num = 0f;
		this.kernings.TryGetValue(prevChar, out num);
		return num;
	}

	public int id;

	public Rect UVs;

	public float xOffset;

	public float yOffset;

	public float xAdvance;

	public Dictionary<int, float> kernings;

	public Dictionary<int, float> origKernings;
}

using System;
using UnityEngine;

[Serializable]
public class Keyframe
{
	public Keyframe(int aFrame, Vector3 aOffset, float aAngle, Vector3 aScale)
	{
		this.frame = aFrame;
		this.offset = aOffset;
		this.angle = aAngle;
		this.scale = aScale;
	}

	public int frame;

	public Vector3 offset;

	public float angle;

	public Vector3 scale = Vector3.one;
}

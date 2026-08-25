using System;
using UnityEngine;

public class CrabCinematic : MonoBehaviour
{
	public void Start()
	{
		this.mCurrentFrame = 0f;
	}

	public void Update()
	{
		int num = (int)this.mCurrentFrame;
		this.mCurrentFrame += Time.deltaTime * 12f;
		int num2 = (int)this.mCurrentFrame % this.animFrames.Length;
		if (num2 != num)
		{
			base.GetComponent<Renderer>().material.mainTexture = this.animFrames[num2];
		}
	}

	private const int kFrameRate = 12;

	public Texture[] animFrames;

	private float mCurrentFrame;
}

using System;
using UnityEngine;

public class CoinTransferProgressBar : MonoBehaviour
{
	private void Start()
	{
		this.mLeftEnd = base.transform.Find("LeftEnd");
		this.mRightEnd = base.transform.Find("RightEnd");
		this.ShowSection(this.mLeftEnd, false);
		this.ShowSection(this.mRightEnd, false);
		this.mChunks = new Transform[7];
		for (int i = 1; i < 8; i++)
		{
			this.mChunks[i - 1] = base.transform.Find("Chunk" + i);
			this.ShowSection(this.mChunks[i - 1], false);
		}
	}

	private void FixedUpdate()
	{
	}

	private void SetProgress(float aProgress)
	{
		this.ShowSection(this.mLeftEnd, false);
		this.ShowSection(this.mRightEnd, false);
		for (int i = 1; i < 8; i++)
		{
			this.ShowSection(this.mChunks[i - 1], false);
		}
		if (aProgress > 10f)
		{
			this.ShowSection(this.mLeftEnd, true);
		}
		if (aProgress > 20f)
		{
			this.ShowSection(this.mChunks[0], true);
		}
		if (aProgress > 30f)
		{
			this.ShowSection(this.mChunks[1], true);
		}
		if (aProgress > 40f)
		{
			this.ShowSection(this.mChunks[2], true);
		}
		if (aProgress > 50f)
		{
			this.ShowSection(this.mChunks[3], true);
		}
		if (aProgress > 60f)
		{
			this.ShowSection(this.mChunks[4], true);
		}
		if (aProgress > 70f)
		{
			this.ShowSection(this.mChunks[5], true);
		}
		if (aProgress > 80f)
		{
			this.ShowSection(this.mChunks[6], true);
		}
		if (aProgress > 90f)
		{
			this.ShowSection(this.mRightEnd, true);
		}
	}

	private void ShowSection(Transform aSection, bool aShow)
	{
		aSection.transform.position = new Vector3(aSection.transform.position.x, aSection.transform.position.y, (!aShow) ? 1f : (-1f));
	}

	private Transform mLeftEnd;

	private Transform mRightEnd;

	private Transform[] mChunks;
}

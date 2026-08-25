using System;
using UnityEngine;

[RequireComponent(typeof(Cannon))]
public class CannonRotator : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		this.mCannon = base.GetComponent<Cannon>();
		this.mStartAngle = this.mTransform.eulerAngles.z;
		this.mTime = 0f;
		this.mInPause = this.doPause;
		this.mCurrentStep = 0;
		this.mForward = true;
	}

	public void FixedUpdate()
	{
		if (this.alwaysActive || this.mCannon.IsPuffleInside())
		{
			this.mTime += TimeManager.Instance.DeltaTime;
			if (this.mTime > this.rotationTime)
			{
				this.mTime -= this.rotationTime;
				if (!this.mInPause)
				{
					if (this.mForward)
					{
						this.mCurrentStep++;
						if (this.mCurrentStep >= this.stepAmount)
						{
							if (this.pingPong)
							{
								this.mForward = false;
							}
							else
							{
								this.mCurrentStep = 0;
							}
						}
					}
					else
					{
						this.mCurrentStep--;
						if (this.mCurrentStep <= 0)
						{
							if (this.pingPong)
							{
								this.mForward = true;
							}
							else
							{
								this.mCurrentStep = this.stepAmount - 1;
							}
						}
					}
				}
				if (this.doPause)
				{
					this.mInPause = !this.mInPause;
				}
			}
			int num = this.mCurrentStep + ((!this.mForward) ? (-1) : 1);
			float num2 = this.mStartAngle + Mathf.LerpAngle(this.rotationStep * (float)this.mCurrentStep, this.rotationStep * (float)num, (!this.mInPause) ? (this.mTime / this.rotationTime) : 0f);
			this.mTransform.eulerAngles = new Vector3(0f, 0f, num2);
		}
	}

	public float rotationTime = 10f;

	public float rotationStep = 90f;

	public int stepAmount = 1;

	public bool pingPong = true;

	public bool alwaysActive;

	public bool doPause = true;

	private Transform mTransform;

	private Cannon mCannon;

	private float mStartAngle;

	private float mTime;

	private bool mInPause;

	private int mCurrentStep;

	private bool mForward;
}

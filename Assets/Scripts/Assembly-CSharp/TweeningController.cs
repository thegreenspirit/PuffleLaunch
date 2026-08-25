using System;
using UnityEngine;

public class TweeningController : MonoBehaviour
{
	public event TweeningEndEventHandler ForwardTweenEnd;

	public event TweeningEndEventHandler ReverseTweenEnd;

	public void Start()
	{
		this.mPlay = this.PlayOnStart;
		this.mForward = true;
		this.mTransform = base.transform;
		this.mFrame = 0f;
		this.mOriginalScale = this.mTransform.localScale;
		this.mOriginalPosition = this.mTransform.localPosition;
		if (this.keyframeOffsetInPixels)
		{
			for (int i = 0; i < this.keyframes.Length; i++)
			{
				this.keyframes[i].offset.x = this.keyframes[i].offset.x * ScaleItem.Instance.LevelScale;
				this.keyframes[i].offset.y = this.keyframes[i].offset.y * ScaleItem.Instance.LevelScale;
				this.keyframes[i].offset.z = this.keyframes[i].offset.z * ScaleItem.Instance.LevelScale;
			}
		}
		if (this.keyframes.Length > 0)
		{
			if (this.AffectOrientation)
			{
				this.mTransform.eulerAngles = new Vector3(0f, 0f, this.keyframes[0].angle);
			}
			this.mTransform.localScale = Vector3.Scale(this.mOriginalScale, this.keyframes[0].scale);
		}
		if (this.keyframes.Length < 2)
		{
			this.mTransform.position += this.keyframes[0].offset;
			base.enabled = false;
		}
	}

	public void FixedUpdate()
	{
		if (this.mPlay)
		{
			if (this.mForward)
			{
				this.mFrame += TimeManager.Instance.DeltaTime;
				if (this.mFrame > (float)this.keyframes[this.mCurrentKeyframe + 1].frame)
				{
					this.mCurrentKeyframe++;
					if (this.mCurrentKeyframe == this.keyframes.Length - 1)
					{
						if (this.Looping)
						{
							this.mFrame = 0f;
							this.mCurrentKeyframe = 0;
						}
						else
						{
							this.mPlay = false;
							if (this.ForwardTweenEnd != null)
							{
								this.ForwardTweenEnd(this, EventArgs.Empty);
							}
							this.mCurrentKeyframe--;
							this.mFrame = (float)this.keyframes[this.mCurrentKeyframe + 1].frame;
						}
					}
				}
				float num = (this.mFrame - (float)this.keyframes[this.mCurrentKeyframe].frame) / (float)(this.keyframes[this.mCurrentKeyframe + 1].frame - this.keyframes[this.mCurrentKeyframe].frame);
				if (this.EaseInEaseOut)
				{
					int num2 = this.keyframes[this.mCurrentKeyframe + 1].frame - this.keyframes[this.mCurrentKeyframe].frame;
					this.EaseInEaseOut3D(ref this.mOffsetVector, num2 - (this.keyframes[this.mCurrentKeyframe + 1].frame - (int)this.mFrame), this.keyframes[this.mCurrentKeyframe].offset, this.keyframes[this.mCurrentKeyframe + 1].offset - this.keyframes[this.mCurrentKeyframe].offset, num2);
				}
				else
				{
					this.mOffsetVector = Vector3.Lerp(this.keyframes[this.mCurrentKeyframe].offset, this.keyframes[this.mCurrentKeyframe + 1].offset, num);
				}
				float num3 = Mathf.Lerp(this.keyframes[this.mCurrentKeyframe].angle, this.keyframes[this.mCurrentKeyframe + 1].angle, num);
				this.ApplyFiltering(ref this.mOffsetVector, ref num3);
				this.mTransform.position -= this.mPreviousOffset;
				this.mOffsetVector = Quaternion.Euler(0f, 0f, num3) * this.mOffsetVector;
				this.mTransform.position += this.mOffsetVector;
				this.mPreviousOffset = this.mOffsetVector;
				this.mWorkingVector.x = 0f;
				this.mWorkingVector.y = 0f;
				this.mWorkingVector.z = num3;
				this.mTransform.eulerAngles = this.mWorkingVector;
				this.mTransform.localScale = Vector3.Scale(this.mOriginalScale, Vector3.Lerp(this.keyframes[this.mCurrentKeyframe].scale, this.keyframes[this.mCurrentKeyframe + 1].scale, num));
			}
			else
			{
				this.mFrame -= TimeManager.Instance.DeltaTime;
				if (this.mFrame < (float)this.keyframes[this.mCurrentKeyframe - 1].frame)
				{
					this.mCurrentKeyframe--;
					if (this.mCurrentKeyframe == 0)
					{
						if (this.Looping)
						{
							this.Reset(false);
							this.mPlay = true;
						}
						else
						{
							this.mPlay = false;
							if (this.ReverseTweenEnd != null)
							{
								this.ReverseTweenEnd(this, EventArgs.Empty);
							}
							this.mCurrentKeyframe++;
							this.mFrame = (float)this.keyframes[0].frame;
						}
					}
				}
				float num = ((float)this.keyframes[this.mCurrentKeyframe].frame - this.mFrame) / (float)(this.keyframes[this.mCurrentKeyframe].frame - this.keyframes[this.mCurrentKeyframe - 1].frame);
				if (this.EaseInEaseOut)
				{
					int num4 = this.keyframes[this.mCurrentKeyframe].frame - this.keyframes[this.mCurrentKeyframe - 1].frame;
					this.EaseInEaseOut3D(ref this.mOffsetVector, this.keyframes[this.mCurrentKeyframe].frame - (int)this.mFrame, this.keyframes[this.mCurrentKeyframe - 1].offset, this.keyframes[this.mCurrentKeyframe].offset - this.keyframes[this.mCurrentKeyframe - 1].offset, num4);
				}
				else
				{
					this.mOffsetVector = Vector3.Lerp(this.keyframes[this.mCurrentKeyframe].offset, this.keyframes[this.mCurrentKeyframe - 1].offset, num);
				}
				float num5 = Mathf.Lerp(this.keyframes[this.mCurrentKeyframe].angle, this.keyframes[this.mCurrentKeyframe - 1].angle, num);
				this.ApplyFiltering(ref this.mOffsetVector, ref num5);
				this.mTransform.position -= this.mPreviousOffset;
				this.mOffsetVector = Quaternion.Euler(0f, 0f, num5) * this.mOffsetVector;
				this.mTransform.position += this.mOffsetVector;
				this.mPreviousOffset = this.mOffsetVector;
				this.mWorkingVector.x = 0f;
				this.mWorkingVector.y = 0f;
				this.mWorkingVector.z = num5;
				this.mTransform.eulerAngles = this.mWorkingVector;
				this.mTransform.localScale = Vector3.Scale(this.mOriginalScale, Vector3.Lerp(this.keyframes[this.mCurrentKeyframe].scale, this.keyframes[this.mCurrentKeyframe - 1].scale, num));
			}
		}
	}

	public void Play(bool aForward)
	{
		this.Reset(false);
		this.mPlay = true;
		if (aForward)
		{
			this.mForward = true;
		}
		else
		{
			this.mForward = false;
		}
	}

	public void Reset(bool aResetPosAndScale)
	{
		if (aResetPosAndScale)
		{
			base.transform.localPosition = this.mOriginalPosition;
			base.transform.localScale = this.mOriginalScale;
		}
		this.mPlay = false;
		if (this.mForward)
		{
			this.mFrame = 0f;
			this.mCurrentKeyframe = 0;
		}
		else
		{
			this.mFrame = (float)this.keyframes[this.keyframes.Length - 1].frame;
			this.mCurrentKeyframe = this.keyframes.Length - 1;
		}
	}

	private void ApplyFiltering(ref Vector3 offset, ref float angle)
	{
		if (!this.AffectX)
		{
			offset.x = 0f;
		}
		if (!this.AffectY)
		{
			offset.y = 0f;
		}
		if (!this.AffectZ)
		{
			offset.z = 0f;
		}
		if (!this.AffectOrientation)
		{
			angle = this.mTransform.eulerAngles.z;
		}
	}

	private void EaseInEaseOut3D(ref Vector3 outVector, int aTime, Vector3 aBegin, Vector3 aChange, int aDuration)
	{
		outVector.x = this.EaseInEaseOut1D((float)aTime, aBegin.x, aChange.x, (float)aDuration);
		outVector.y = this.EaseInEaseOut1D((float)aTime, aBegin.y, aChange.y, (float)aDuration);
		outVector.z = this.EaseInEaseOut1D((float)aTime, aBegin.z, aChange.z, (float)aDuration);
	}

	private float EaseInEaseOut1D(float aTime, float aBegin, float aChange, float aDuration)
	{
		if ((aTime /= 0.5f * aDuration) < 1f)
		{
			return 0.5f * aChange * aTime * aTime + aBegin;
		}
		return -(0.5f * aChange) * ((aTime -= 1f) * (aTime - 2f) - 1f) + aBegin;
	}

	public global::Keyframe[] Keyframes
	{
		get
		{
			return this.keyframes;
		}
		set
		{
			if (this.keyframeOffsetInPixels)
			{
				for (int i = 0; i < value.Length; i++)
				{
					value[i].offset.x = value[i].offset.x * ScaleItem.Instance.LevelScale;
					value[i].offset.y = value[i].offset.y * ScaleItem.Instance.LevelScale;
					value[i].offset.z = value[i].offset.z * ScaleItem.Instance.LevelScale;
				}
			}
			this.keyframes = value;
		}
	}

	public bool PlayOnStart = true;

	public bool Looping = true;

	public bool keyframeOffsetInPixels;

	public bool keyframeOffsetInWorldSpace = true;

	public global::Keyframe[] keyframes;

	public bool EaseInEaseOut;

	public bool AffectX = true;

	public bool AffectY = true;

	public bool AffectZ = true;

	public bool AffectOrientation = true;

	private Transform mTransform;

	private Vector3 mPreviousOffset;

	private Vector3 mOriginalScale;

	private Vector3 mOriginalPosition;

	private int mCurrentKeyframe;

	private float mFrame;

	private bool mForward;

	private bool mPlay;

	private Vector3 mOffsetVector = default(Vector3);

	private Vector3 mWorkingVector = default(Vector3);
}

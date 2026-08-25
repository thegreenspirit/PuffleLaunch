using System;
using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
	public virtual void Start()
	{
		this.mTransform = base.transform;
		this.mStartPosition = this.mTransform.position;
		this.mWaitTime = 0f;
		Vector3 vector = this.movementOffset.normalized * this.velocity * ScaleItem.Instance.LevelScale;
		if (this.accelerate)
		{
			this.mAcceleration = vector;
		}
		else
		{
			this.mVelocity = vector;
		}
	}

	public void FixedUpdate()
	{
		this.UpdateTransform();
	}

	protected virtual void UpdateTransform()
	{
		if (this.mWaitTime == 0f)
		{
			Vector3 vector = this.mVelocity;
			if (this.accelerate)
			{
				vector = this.mAcceleration;
			}
			Vector3 vector2 = this.mStartPosition;
			if (Vector3.Dot(vector, this.movementOffset) > 0f)
			{
				vector2 += this.movementOffset * ScaleItem.Instance.LevelScale;
			}
			this.mVelocity += this.mAcceleration * TimeManager.Instance.DeltaTime;
			Vector3 vector3 = this.mTransform.position + this.mVelocity * TimeManager.Instance.DeltaTime;
			Vector3 vector4 = vector2 - vector3;
			if (Vector3.Dot(vector4, vector) < -0.001f)
			{
				this.mWaitTime = this.waitFrames;
				if (!this.accelerate)
				{
					this.mVelocity = Vector3.zero;
					this.mTransform.position = vector2;
				}
				else
				{
					this.mAcceleration = -this.mAcceleration;
				}
			}
		}
		else
		{
			this.mWaitTime = Mathf.Max(this.mWaitTime - TimeManager.Instance.DeltaTime, 0f);
		}
		this.mVelocity *= 1f - this.friction * TimeManager.Instance.DeltaTime;
		this.mTransform.position += this.mVelocity * TimeManager.Instance.DeltaTime;
		if (!this.accelerate && this.mVelocity == Vector3.zero && this.mWaitTime == 0f)
		{
			this.mVelocity = -this.movementOffset.normalized * this.velocity * ScaleItem.Instance.LevelScale;
			if (this.mTransform.position == this.mStartPosition)
			{
				this.mVelocity *= -1f;
			}
		}
	}

	public Vector3 movementOffset;

	public float velocity;

	public float friction;

	public float waitFrames;

	public bool accelerate;

	protected Vector3 mVelocity;

	protected Vector3 mAcceleration;

	protected Transform mTransform;

	protected Vector3 mStartPosition;

	protected float mWaitTime;
}

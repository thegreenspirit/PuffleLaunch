using System;
using UnityEngine;

public class ElasticMovement : MonoBehaviour
{
	public Vector3 Velocity
	{
		get
		{
			return this.mVelocity;
		}
		set
		{
			this.mVelocity = value;
			this.mSleeping = false;
		}
	}

	public Vector3 TargetPosition
	{
		get
		{
			return this.mTargetPosition;
		}
		set
		{
			this.mTargetPosition = value;
			this.mTargetOverride = true;
			this.mSleeping = false;
		}
	}

	public void Awake()
	{
		this.mTargetOverride = false;
	}

	public void Start()
	{
		this.mTransform = base.transform;
		if (!this.mTargetOverride)
		{
			this.mTargetPosition = this.mTransform.position;
		}
		this.mElasticMultiplierDefault = this.elasticMultiplier;
		this.mSleeping = false;
	}

	public void FixedUpdate()
	{
		if (!this.mSleeping)
		{
			this.UpdateTransform(TimeManager.Instance.DeltaTime);
		}
		if (this.restoreElasticity)
		{
			if (this.elasticMultiplier < this.mElasticMultiplierDefault)
			{
				float num = 100f - this.elasticMultiplier / this.mElasticMultiplierDefault * 100f;
				if (num > 10f)
				{
					this.elasticMultiplier *= 1.05f;
				}
			}
			else if (this.elasticMultiplier > this.mElasticMultiplierDefault)
			{
				this.elasticMultiplier = this.mElasticMultiplierDefault;
			}
		}
	}

	public void UpdateTransform(float aDeltaTime)
	{
		if (LevelLoader.Instance != null)
		{
			float levelScale = ScaleItem.Instance.LevelScale;
			Vector3 vector = this.mTargetPosition - this.mTransform.position;
			if (Mathf.Abs(vector.x) <= levelScale)
			{
				vector.x = 0f;
			}
			if (Mathf.Abs(vector.y) <= levelScale)
			{
				vector.y = 0f;
			}
			this.mVelocity += vector * this.elasticMultiplier * aDeltaTime;
			this.mVelocity *= 1f - this.friction * aDeltaTime;
			this.mTransform.position += this.mVelocity * aDeltaTime;
			if (this.mVelocity.sqrMagnitude < 0.0001f && vector == Vector3.zero)
			{
				this.mVelocity = Vector3.zero;
				this.mSleeping = true;
			}
		}
	}

	public float elasticMultiplier = 0.01f;

	public float friction = 0.1f;

	public bool restoreElasticity;

	private Transform mTransform;

	private Vector3 mVelocity;

	private Vector3 mTargetPosition;

	private float mElasticMultiplierDefault;

	private bool mTargetOverride;

	private bool mSleeping;
}

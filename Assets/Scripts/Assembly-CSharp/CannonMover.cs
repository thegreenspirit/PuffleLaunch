using System;
using UnityEngine;

[RequireComponent(typeof(Cannon))]
public class CannonMover : ObstacleMover
{
	public override void Start()
	{
		base.Start();
		this.mCannon = base.GetComponent<Cannon>();
		this.mCollider = (SphereCollider)base.GetComponent<Collider>();
	}

	protected override void UpdateTransform()
	{
		Vector3 position = this.mTransform.position;
		if (this.alwaysActive || this.mCannon.IsPuffleInside())
		{
			base.UpdateTransform();
		}
		else if (this.mTransform.position != this.mStartPosition)
		{
			if (Vector3.Dot(this.mVelocity, this.movementOffset) > 0f)
			{
				this.mVelocity = -this.movementOffset.normalized * this.velocity * ScaleItem.Instance.LevelScale;
			}
			base.UpdateTransform();
		}
		Vector3 vector = this.mTransform.position - position;
		this.mCollider.center = this.mTransform.rotation * vector;
	}

	public bool alwaysActive;

	private Cannon mCannon;

	private SphereCollider mCollider;
}

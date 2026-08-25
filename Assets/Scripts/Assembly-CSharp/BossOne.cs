using System;
using UnityEngine;

public class BossOne : MonoBehaviour
{
	public void Start()
	{
		this.mElasticMovement = base.GetComponent<ElasticMovement>();
		this.mBossController = base.GetComponent<BossController>();
		this.mStartPosition = base.transform.position;
		this.mMoveTimeout = 0;
	}

	public void FixedUpdate()
	{
		if (this.mBossController.IsAlive)
		{
			if (this.mMoveTimeout > 0)
			{
				this.mMoveTimeout--;
			}
			else
			{
				this.mMoveTimeout = 40;
				if (this.mElasticMovement.TargetPosition.x > this.mStartPosition.x)
				{
					this.mElasticMovement.TargetPosition = new Vector3(this.mStartPosition.x - 500f * ScaleItem.Instance.LevelScale, this.mStartPosition.y, 0f);
				}
				else
				{
					this.mElasticMovement.TargetPosition = new Vector3(this.mStartPosition.x + 500f * ScaleItem.Instance.LevelScale, this.mStartPosition.y, 0f);
				}
			}
		}
	}

	private ElasticMovement mElasticMovement;

	private BossController mBossController;

	private Vector3 mStartPosition;

	private int mMoveTimeout;
}

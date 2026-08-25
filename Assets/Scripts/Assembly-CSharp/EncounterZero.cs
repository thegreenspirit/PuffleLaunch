using System;
using UnityEngine;

public class EncounterZero : MonoBehaviour
{
	public void Start()
	{
		this.mElasticMovement = base.GetComponent<ElasticMovement>();
		this.mCrabbyAnimController = base.GetComponentInChildren<CrabbyAnimController>();
		this.mBossController = base.GetComponent<BossController>();
		this.mIsLeaving = false;
		this.mStartPosition = base.transform.position;
		this.mMoveTimeout = 0;
	}

	public void FixedUpdate()
	{
		if (this.mBossController.IsAlive)
		{
			if (this.mIsLeaving)
			{
				this.mElasticMovement.Velocity += new Vector3(-0.2f, 0.2f, 0f) * ScaleItem.Instance.LevelScale;
				if (!this.mCrabbyAnimController.IsAnimPlaying)
				{
					this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLeaving);
				}
			}
			else if (this.mMoveTimeout > 0)
			{
				this.mMoveTimeout--;
			}
			else
			{
				this.mMoveTimeout = 40;
				if (this.mElasticMovement.TargetPosition.x > this.mStartPosition.x)
				{
					this.mElasticMovement.TargetPosition = new Vector3(this.mStartPosition.x - 100f * ScaleItem.Instance.LevelScale, this.mStartPosition.y, 0f);
				}
				else
				{
					this.mElasticMovement.TargetPosition = new Vector3(this.mStartPosition.x + 100f * ScaleItem.Instance.LevelScale, this.mStartPosition.y, 0f);
				}
			}
		}
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (aOther.tag == "Player")
		{
			this.mIsLeaving = true;
			this.mElasticMovement.elasticMultiplier = 0f;
		}
	}

	public void OnGiantPuffleOCollect()
	{
		this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLose);
	}

	private ElasticMovement mElasticMovement;

	private CrabbyAnimController mCrabbyAnimController;

	private BossController mBossController;

	private bool mIsLeaving;

	private Vector3 mStartPosition;

	private int mMoveTimeout;
}

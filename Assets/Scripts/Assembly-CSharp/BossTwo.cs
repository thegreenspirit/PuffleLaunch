using System;
using UnityEngine;

[RequireComponent(typeof(ElasticMovement))]
[RequireComponent(typeof(BossController))]
[RequireComponent(typeof(PathFollower))]
public class BossTwo : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		this.mElasticMovement = base.GetComponent<ElasticMovement>();
		this.mBossController = base.GetComponent<BossController>();
		this.mBossController.onRecovery += this.RecoveryEventHandler;
		this.mPathFollower = base.GetComponent<PathFollower>();
		this.mArm = base.transform.Find("Magnet");
		this.mPlayer = Puffle.Instance.transform;
		Puffle.Instance.puffleDeath += this.PlayerRespawnHandler;
		this.mIsChasing = false;
		this.mIsAttacking = true;
		this.mResetOnLoop = false;
		this.mArmAngle = 0f;
		this.mArmAngularVelocity = 0f;
		this.mFrameCount = 0U;
		this.mInCutscene = true;
		GameManager.Instance.DuringCutscene = true;
		Puffle.Instance.DisableInput = true;
		this.mPathBackup = new Vector3[this.mPathFollower.pathNodes.Length];
		this.mPathFollower.pathNodes.CopyTo(this.mPathBackup, 0);
		this.mPathFollower.pathNodes = new Vector3[]
		{
			new Vector3(653f, 1540f),
			new Vector3(-665f, 1603f),
			new Vector3(-675.05f, 1325f)
		};
		this.mPathFollower.CurrentNode = 0;
		AudioManager.Instance.PlayMusic(AudioManager.MusicTrack.eMusic_Boss);
	}

	public void FixedUpdate()
	{
		this.mFrameCount += 1U;
		if (this.mFrameCount == 72U)
		{
			this.mRestoreSlowMo = TimeManager.Instance.SlowmoOverride;
			GameManager.Instance.StartCutscene(true);
		}
		else if (this.mFrameCount == 96U)
		{
			AudioManager.Instance.PlayMusic(AudioManager.MusicTrack.eMusic_Boss);
		}
		else if (this.mFrameCount == 240U)
		{
			Puffle.Instance.DisableInput = false;
			GameManager.Instance.EndCutscene();
			if (this.mRestoreSlowMo)
			{
				GameManager.Instance.ActivatePlayerSlowMo();
				GameFlowManager.Instance.GUIManager.HudManager.InGameHud.SetSlowmoButtonState(this.mRestoreSlowMo);
			}
		}
		else if (this.mFrameCount == 336U)
		{
			this.mPathFollower.pathNodes = new Vector3[this.mPathBackup.Length];
			this.mPathBackup.CopyTo(this.mPathFollower.pathNodes, 0);
			this.mPathFollower.CurrentNode = 0;
			this.mElasticMovement.TargetPosition = this.mPathFollower.pathNodes[0] * ScaleItem.Instance.LevelScale;
			this.mIsAttacking = false;
			this.mInCutscene = false;
			GameManager.Instance.DuringCutscene = false;
		}
		if (this.mBossController.IsAlive)
		{
			float sqrMagnitude = (this.mTransform.position - this.mPlayer.position).sqrMagnitude;
			if (!this.mInCutscene)
			{
				this.AdjustSpeed(sqrMagnitude);
				if (this.mIsChasing)
				{
					this.mChaseTimer -= TimeManager.Instance.DeltaTime;
					if (this.mChaseTimer <= 0f)
					{
						this.mIsChasing = false;
					}
					this.ChasePlayer();
				}
			}
			if (this.mIsAttacking)
			{
				if (this.mResetOnLoop)
				{
					if (this.mPathFollower.CurrentNode == this.mAttackStartNode)
					{
						this.mIsAttacking = false;
						this.mResetOnLoop = false;
					}
				}
				else if (this.mPathFollower.CurrentNode != this.mAttackStartNode)
				{
					this.mResetOnLoop = true;
				}
			}
		}
		this.SwingArm();
	}

	public void RecoveryEventHandler(object sender, EventArgs e)
	{
		this.mIsChasing = true;
		this.mChaseTimer = 120f;
		this.mIsAttacking = true;
		this.mAttackStartNode = this.mPathFollower.CurrentNode;
	}

	public void PlayerRespawnHandler(object sender, EventArgs e)
	{
		this.mIsAttacking = true;
		this.mAttackStartNode = this.mPathFollower.CurrentNode;
	}

	private void AdjustSpeed(float aDistance)
	{
		if (aDistance > Mathf.Pow(1500f * ScaleItem.Instance.LevelScale, 2f))
		{
			this.mElasticMovement.elasticMultiplier = 0.02f;
		}
		else if (aDistance > Mathf.Pow(1000f * ScaleItem.Instance.LevelScale, 2f))
		{
			this.mElasticMovement.elasticMultiplier = 0.01f;
		}
		else if (aDistance > Mathf.Pow(600f * ScaleItem.Instance.LevelScale, 2f))
		{
			this.mElasticMovement.elasticMultiplier = 0.006f;
		}
		else
		{
			this.mElasticMovement.elasticMultiplier = 0.003f;
		}
	}

	private void ChasePlayer()
	{
		int num = this.mPathFollower.CurrentNode + 1;
		if (num == this.mPathFollower.pathNodes.Length)
		{
			num = 0;
		}
		float sqrMagnitude = (this.mPlayer.position - this.mPathFollower.pathNodes[num]).sqrMagnitude;
		int num2 = this.mPathFollower.CurrentNode - 1;
		if (num2 == -1)
		{
			num2 = this.mPathFollower.pathNodes.Length - 1;
		}
		float sqrMagnitude2 = (this.mPlayer.position - this.mPathFollower.pathNodes[num2]).sqrMagnitude;
		this.mPathFollower.reversed = sqrMagnitude2 > sqrMagnitude;
	}

	private void SwingArm()
	{
		float num = ((!this.mInCutscene) ? TimeManager.Instance.DeltaTime : 1f);
		float num2 = -this.mElasticMovement.Velocity.x / ScaleItem.Instance.LevelScale;
		if (this.mIsAttacking)
		{
			this.mArmAngularVelocity += 4f * Mathf.Sign(num2) * num;
		}
		this.mArmAngularVelocity += num2 * 0.1f * num;
		if (this.mArmAngle != 0f && !this.mIsAttacking)
		{
			this.mArmAngularVelocity -= this.mArmAngle * 0.1f * num;
		}
		this.mArmAngularVelocity *= 0.9f * num;
		this.mArmAngle += this.mArmAngularVelocity * num;
		if (this.mArmAngle > 360f)
		{
			this.mArmAngle -= 360f;
			if (!AudioManager.Instance.Muted)
			{
				this.mArm.GetComponent<AudioSource>().Play();
			}
		}
		else if (this.mArmAngle < -360f)
		{
			this.mArmAngle += 360f;
			if (!AudioManager.Instance.Muted)
			{
				this.mArm.GetComponent<AudioSource>().Play();
			}
		}
		this.mArm.localEulerAngles = new Vector3(0f, 0f, this.mArmAngle);
	}

	public bool IsAttacking
	{
		get
		{
			return this.mIsAttacking;
		}
	}

	private Transform mTransform;

	private ElasticMovement mElasticMovement;

	private BossController mBossController;

	private PathFollower mPathFollower;

	private Transform mArm;

	private Transform mPlayer;

	private bool mIsChasing;

	private float mChaseTimer;

	private bool mIsAttacking;

	private int mAttackStartNode;

	private bool mResetOnLoop;

	private float mArmAngle;

	private float mArmAngularVelocity;

	private uint mFrameCount;

	private Vector3[] mPathBackup;

	private bool mInCutscene;

	private bool mRestoreSlowMo;
}

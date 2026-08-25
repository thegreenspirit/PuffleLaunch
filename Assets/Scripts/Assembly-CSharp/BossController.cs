using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
	public event HitEventHandler onHit;

	public event RecoveryEventHandler onRecovery;

	public void Start()
	{
		this.mTransform = base.transform;
		this.mElasticMovement = base.GetComponent<ElasticMovement>();
		this.mCrabbyAnimController = base.GetComponentInChildren<CrabbyAnimController>();
		this.mCrabbyAnimController.animationEnd += this.CrabbyAnimEndEventHandler;
		this.mTweeningController = base.GetComponent<TweeningController>();
		this.mSpriteManager = base.GetComponent<SpriteManager>();
		this.mAudioSource = base.GetComponent<AudioSource>();
		this.mAudioSource.mute = AudioManager.Instance.Muted;
		this.mStartPosition = this.mTransform.position;
		this.mHealth = 8;
		this.mIsAlive = true;
		this.mIsCollidable = true;
	}

	public void Update()
	{
		this.mAudioSource.mute = AudioManager.Instance.Muted;
	}

	public void FixedUpdate()
	{
		if (!this.mIsAlive)
		{
			if (this.mCrabbyAnimController.CurrentAnim != CrabbyAnimController.CrabbyAnim.eFreefall)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eFreefall);
				if (this.spawnGiantPuffleO)
				{
					this.SpawnGiantPuffleO();
				}
			}
			this.mElasticMovement.Velocity -= Vector3.up * 0.4f * ScaleItem.Instance.LevelScale;
			if (Random.Range(0, 15) == 0)
			{
				Vector3 vector = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), -1f);
				global::UnityEngine.Object.Instantiate(this.hitFX, this.mTransform.position + vector, default(Quaternion));
				if (!AudioManager.Instance.Muted)
				{
					this.mAudioSource.Play();
				}
			}
		}
		else if (this.mHitCollisionTimeout > 0f)
		{
			this.mHitCollisionTimeout = Mathf.Max(this.mHitCollisionTimeout - TimeManager.Instance.DeltaTime, 0f);
			if (this.mHitCollisionTimeout == 0f)
			{
				this.mIsCollidable = true;
			}
		}
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (this.mIsCollidable && aOther.tag == "Player")
		{
			Puffle component = aOther.GetComponent<Puffle>();
			if (component.State == Puffle.PuffleState.eFlying)
			{
				Vector3 vector = this.mTransform.position - component.transform.position;
				float num = Mathf.Round(Mathf.Atan2(vector.y, vector.x) * 57.29578f);
				Vector3 vector2 = new Vector3(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f), 0f);
				vector2 *= 20f * ScaleItem.Instance.LevelScale;
				this.mElasticMovement.Velocity = vector2 * 1.5f;
				this.ReboundPlayer(component, vector2);
				this.TakeDamage();
				TimeManager.Instance.StopSlowmo();
				if (this.onHit != null)
				{
					this.onHit(this, EventArgs.Empty);
				}
				if (this.impactSound != null)
				{
					AudioManager.Instance.PlayObstacleSound(this.impactSound);
				}
			}
		}
	}

	public void OnPuffleOCollect()
	{
		if (!this.mCrabbyAnimController.IsAnimPlaying)
		{
			this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
		}
	}

	public void CrabbyAnimEndEventHandler(object sender, CrabbyAnimEndEventArgs e)
	{
		if (e.anim == CrabbyAnimController.CrabbyAnim.eHit)
		{
			if (this.mIsAlive)
			{
				if (this.mTweeningController)
				{
					this.mTweeningController.enabled = true;
				}
				if (this.onRecovery != null)
				{
					this.onRecovery(this, EventArgs.Empty);
				}
			}
			if (this.mSpriteManager.current.name == "Ship2")
			{
				this.mSpriteManager.current.framerate = 0;
				this.mSpriteManager.Seek(0);
			}
		}
	}

	private void ReboundPlayer(Puffle aPuffle, Vector3 aPush)
	{
		Vector3 vector = new Vector3(aPush.x * -0.2f, -aPush.y, 0f);
		if (aPuffle.Velocity.y <= 0f)
		{
			vector.y = (Mathf.Abs(aPush.y) + Mathf.Abs(aPush.x)) * 0.5f;
		}
		aPuffle.Velocity = vector;
		aPuffle.AngularVelocity = (Mathf.Abs(aPush.x) + Mathf.Abs(aPush.y)) / ScaleItem.Instance.LevelScale;
	}

	private void TakeDamage()
	{
		if (this.mHealth > 0)
		{
			global::UnityEngine.Object.Instantiate(this.hitFX, this.mTransform.position - Vector3.forward, default(Quaternion));
			if (this.mTweeningController)
			{
				this.mTweeningController.enabled = false;
			}
			this.mTransform.eulerAngles = Vector3.zero;
			if (--this.mHealth == 0)
			{
				this.mIsAlive = false;
				this.mIsCollidable = false;
				this.mElasticMovement.elasticMultiplier = 0f;
				this.mAudioSource.Stop();
				this.mAudioSource.clip = this.explosionSound;
				this.mAudioSource.loop = false;
				Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					global::UnityEngine.Object.Destroy(collider);
				}
				AudioManager.Instance.PlayMusic(AudioManager.MusicTrack.eMusic_Win);
			}
			this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eHit);
			if (this.mSpriteManager.current.name == "Ship2")
			{
				this.mSpriteManager.current.framerate = 24;
				this.mSpriteManager.Seek(1);
			}
			else if (this.mSpriteManager.current.name == "Ship3")
			{
			}
		}
	}

	private void SpawnGiantPuffleO()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("Finish");
		if (gameObject)
		{
			gameObject.transform.position = this.mTransform.position;
			gameObject.GetComponent<ElasticMovement>().TargetPosition = this.mStartPosition;
		}
	}

	public bool IsAlive
	{
		get
		{
			return this.mIsAlive;
		}
	}

	public bool IsCollidable
	{
		get
		{
			return this.mIsCollidable;
		}
	}

	private const int mkCollisionTimeoutFrames = 15;

	public bool spawnGiantPuffleO;

	public GameObject hitFX;

	public AudioClip impactSound;

	public AudioClip explosionSound;

	private Transform mTransform;

	private ElasticMovement mElasticMovement;

	private CrabbyAnimController mCrabbyAnimController;

	private TweeningController mTweeningController;

	private SpriteManager mSpriteManager;

	private AudioSource mAudioSource;

	private Vector3 mStartPosition;

	private int mHealth;

	private bool mIsAlive;

	private bool mIsCollidable;

	private float mHitCollisionTimeout;
}

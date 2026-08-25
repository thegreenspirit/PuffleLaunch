using System;
using UnityEngine;

public class PuffleO : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		this.mProgressBar = GameObject.Find("ProgressBar").GetComponent<ProgressBar>();
		this.mMagnet = null;
		this.mStartPosition = base.transform.position;
		this.mSleeping = true;
	}

	public void FixedUpdate()
	{
		if (!this.mSleeping)
		{
			if (this.mMagnet)
			{
				Vector3 vector = this.mMagnet.position + this.mMagnetOffset - this.mTransform.position;
				float magnitude = vector.magnitude;
				this.mVelocity += 0.01f * (this.mMagnetRadius - magnitude) * vector / magnitude;
			}
			this.mVelocity *= 0.9f * TimeManager.Instance.DeltaTime;
			this.mTransform.position += this.mVelocity;
			if (this.mMagnet == null && this.mVelocity.sqrMagnitude < 0.0001f)
			{
				this.mVelocity = Vector3.zero;
				this.mSleeping = true;
			}
		}
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (aOther.tag == "Player")
		{
			Puffle component = aOther.GetComponent<Puffle>();
			if (component.State == Puffle.PuffleState.eFlying)
			{
				this.OnCollect();
				this.mProgressBar.CollectPuffleO();
				GameManager.smCurrentLevelRingCount++;
			}
		}
		else if (aOther.tag == "Magnet")
		{
			if (this.mMagnet)
			{
				this.mMagnet.root.GetComponent<BossController>().OnPuffleOCollect();
				this.OnCollect();
			}
			else
			{
				this.mMagnet = aOther.transform;
				this.mMagnetOffset = ((SphereCollider)aOther).center;
				this.mMagnetRadius = ((SphereCollider)aOther).radius;
				this.mSleeping = false;
			}
		}
	}

	public void OnTriggerExit(Collider aOther)
	{
		if (aOther.tag == "Magnet")
		{
			this.mMagnet = null;
		}
	}

	public void EffectEndEventHandler(object sender, AnimationChangedEventArgs e)
	{
		global::UnityEngine.Object.Destroy(((SpriteManager)sender).gameObject);
		GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/PuffleOBreadcrumb", typeof(GameObject)), this.mStartPosition, default(Quaternion)) as GameObject;
		gameObject.transform.localScale *= ScaleItem.Instance.BillboardScale;
		if (GameManager.Instance.CurrentWorld == GameManager.World.eWorld_BonusWorld)
		{
			string text = "Textures/PuffleOBreadcrumb/PuffleOBreadcrumbBonusLevel_texture_01";
			if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres)
			{
				text = "Textures/PuffleOBreadcrumb/PuffleOBreadcrumbBonusLevel_texture_01";
			}
			Texture texture = Resources.Load(text, typeof(Texture)) as Texture;
			gameObject.GetComponent<MeshRenderer>().materials[0].mainTexture = texture;
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnCollect()
	{
		AudioManager.Instance.PlayPuffleOSound(this.mPuffleOCollectSound);
		this.mMagnet = null;
		this.mVelocity = Vector3.zero;
		global::UnityEngine.Object.Destroy(base.GetComponent<Collider>());
		SpriteManager spriteManager = (SpriteManager)global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/PuffleOEffect", typeof(SpriteManager)) as SpriteManager, this.mTransform.position, default(Quaternion));
		spriteManager.animationend += this.EffectEndEventHandler;
		spriteManager.transform.localScale *= ScaleItem.Instance.BillboardScale;
		base.GetComponent<Renderer>().enabled = false;
	}

	public AudioClip mPuffleOCollectSound;

	private Transform mTransform;

	private ProgressBar mProgressBar;

	private Transform mMagnet;

	private Vector3 mMagnetOffset;

	private float mMagnetRadius;

	private Vector3 mStartPosition;

	private Vector3 mVelocity;

	private bool mSleeping;
}

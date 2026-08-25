using System;
using UnityEngine;

[RequireComponent(typeof(SpriteManager))]
public class Splash : MonoBehaviour
{
	public void Start()
	{
		this.mSpriteManager = base.GetComponent<SpriteManager>();
		this.mSpriteManager.animationend += this.OnAnimationEnd;
		this.mRenderer = base.GetComponent<MeshRenderer>();
		base.transform.localScale *= ScaleItem.Instance.BillboardScale;
	}

	public void FixedUpdate()
	{
		if (this.mAnimEndReached && --this.mRespawnTimer == 0)
		{
			this.mRenderer.enabled = false;
			this.mSpriteManager.enabled = false;
			this.mAnimEndReached = false;
			this.mRespawnTimer = 6;
			base.gameObject.active = false;
			this.mPuffle.Respawn();
		}
	}

	public void OnAnimationEnd(object sender, AnimationChangedEventArgs e)
	{
		this.mRenderer.enabled = false;
		this.mAnimEndReached = true;
	}

	public Puffle Puffle
	{
		get
		{
			return this.mPuffle;
		}
		set
		{
			this.mPuffle = value;
		}
	}

	public void Reset()
	{
		base.gameObject.active = true;
		if (!this.mRenderer)
		{
			this.mRenderer = base.GetComponent<MeshRenderer>();
		}
		this.mRenderer.enabled = true;
		if (!this.mSpriteManager)
		{
			this.mSpriteManager = base.GetComponent<SpriteManager>();
		}
		this.mSpriteManager.enabled = true;
		this.mSpriteManager.Reset();
		this.mSpriteManager.Play(0);
	}

	private SpriteManager mSpriteManager;

	private MeshRenderer mRenderer;

	private Puffle mPuffle;

	private bool mAnimEndReached;

	private int mRespawnTimer = 6;
}

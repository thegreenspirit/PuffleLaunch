using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteManager))]
public class CrabbyAnimController : MonoBehaviour
{
	public event CrabbyAnimEndEventHandler animationEnd;

	public void Start()
	{
		this.mSpriteManager = base.GetComponent<SpriteManager>();
		this.mSpriteManager.animationend += this.OnAnimationEnd;
		this.InitIdles();
		this.InitAnims();
	}

	public void FixedUpdate()
	{
		if (!this.mIsAnimPlaying && Random.Range(0, 10) == 0)
		{
			if (this.reachingIdle)
			{
				this.mSpriteManager.Play(this.mIdleAnimsReaching[Random.Range(0, this.mIdleAnimsReaching.Length)]);
			}
			else
			{
				this.mSpriteManager.Play(this.mIdleAnimsStill[Random.Range(0, this.mIdleAnimsStill.Length)]);
			}
			this.mIsAnimPlaying = true;
			this.mAnimPlaying = CrabbyAnimController.CrabbyAnim.eIdle;
		}
	}

	public void Play(CrabbyAnimController.CrabbyAnim aAnim)
	{
		this.mSpriteManager.Play(this.mAnimNames[(int)aAnim]);
		this.mAnimPlaying = aAnim;
		this.mIsAnimPlaying = true;
	}

	public void OnAnimationEnd(object sender, AnimationChangedEventArgs e)
	{
		if (this.animationEnd != null)
		{
			this.animationEnd(this, new CrabbyAnimEndEventArgs(this.mAnimPlaying));
		}
		this.mAnimPlaying = CrabbyAnimController.CrabbyAnim.eIdle;
		this.mIsAnimPlaying = false;
	}

	public bool IsAnimPlaying
	{
		get
		{
			return this.mIsAnimPlaying;
		}
	}

	public CrabbyAnimController.CrabbyAnim CurrentAnim
	{
		get
		{
			return this.mAnimPlaying;
		}
	}

	private void InitIdles()
	{
		if (this.reachingIdle)
		{
			this.mSpriteManager.defaultAnimation = 2;
		}
		else
		{
			this.mSpriteManager.defaultAnimation = 5;
		}
		this.mIdleAnimsStill = new string[] { "Still_ArmL", "Still_ArmR" };
		this.mIdleAnimsReaching = new string[] { "Reaching_ArmL", "Reaching_ArmR", "Blink" };
	}

	private void InitAnims()
	{
		this.mIsAnimPlaying = false;
		this.mAnimNames = new string[] { "Leaving", "Laugh", "Freefall", "Hit", "Lose" };
	}

	public bool reachingIdle;

	private SpriteManager mSpriteManager;

	private bool mIsAnimPlaying;

	private CrabbyAnimController.CrabbyAnim mAnimPlaying;

	private string[] mIdleAnimsStill;

	private string[] mIdleAnimsReaching;

	private string[] mAnimNames;

	public enum CrabbyAnim
	{
		eLeaving,
		eLaugh,
		eFreefall,
		eHit,
		eLose,
		eIdle
	}
}

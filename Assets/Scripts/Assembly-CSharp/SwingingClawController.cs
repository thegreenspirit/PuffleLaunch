using System;
using UnityEngine;

public class SwingingClawController : MonoBehaviour
{
	private void Start()
	{
		this.mAudioSource = base.GetComponent<AudioSource>();
		this.mSpriteManager = base.GetComponent<SpriteManager>();
		if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres)
		{
			base.transform.localPosition *= 2f;
		}
	}

	private void Update()
	{
		this.mAudioSource.mute = AudioManager.Instance.Muted;
	}

	private void FixedUpdate()
	{
		if (this.mFrameCountDelay == 0)
		{
			if (this.mSpriteManager.current.frame == 8)
			{
				this.mSpriteManager.Pause(true);
				this.mFrameCountDelay = 18;
			}
			else if (this.mSpriteManager.current.frame == 9)
			{
				this.mAudioSource.PlayOneShot(this.retractClawSound);
			}
			else if (this.mSpriteManager.current.frame == 18)
			{
				this.mSpriteManager.Pause(true);
				this.mFrameCountDelay = 18;
			}
			else if (this.mSpriteManager.current.frame == 19)
			{
				this.mAudioSource.PlayOneShot(this.extendClawSound);
			}
		}
		else
		{
			this.mFrameCountDelay--;
			if (this.mFrameCountDelay == 0)
			{
				this.mSpriteManager.Pause(false);
			}
		}
	}

	public AudioClip retractClawSound;

	public AudioClip extendClawSound;

	private int mFrameCount;

	private int mFrameCountDelay;

	private AudioSource mAudioSource;

	private SpriteManager mSpriteManager;
}

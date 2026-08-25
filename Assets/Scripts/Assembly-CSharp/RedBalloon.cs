using System;
using UnityEngine;

public class RedBalloon : Balloon
{
	public override void Start()
	{
		base.Start();
		this.mSpriteManager = base.GetComponent<SpriteManager>();
		this.mSpriteManager.animationend += this.OnAnimationEnd;
	}

	protected override void ReboundPlayer(Puffle aPuffle, Vector3 aPush)
	{
		Vector3 vector = new Vector3(aPush.x * -0.5f, aPush.y * -1.5f, 0f);
		aPuffle.Velocity = vector;
		aPuffle.AngularVelocity = (Mathf.Abs(aPush.x) + Mathf.Abs(aPush.y)) / ScaleItem.Instance.LevelScale;
		this.mLoop = 10;
		this.mSpriteManager.animations[0].framerate = 24;
		this.mSpriteManager.Seek(1);
	}

	public void OnAnimationEnd(object sender, AnimationChangedEventArgs e)
	{
		if (--this.mLoop > 0)
		{
			this.mSpriteManager.Seek(1);
		}
		else
		{
			this.mSpriteManager.Seek(0);
			this.mSpriteManager.animations[0].framerate = 0;
		}
	}

	private const int mkLoopCount = 10;

	private SpriteManager mSpriteManager;

	private int mLoop;
}

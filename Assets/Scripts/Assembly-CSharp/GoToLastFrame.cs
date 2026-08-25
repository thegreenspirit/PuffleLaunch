using System;
using UnityEngine;

public class GoToLastFrame : MonoBehaviour
{
	private void Start()
	{
		this.mSpriteManager = base.GetComponent<SpriteManager>();
		this.mSpriteManager.Play("Fire");
		this.mSpriteManager.GoToLastFrame();
	}

	private SpriteManager mSpriteManager;
}

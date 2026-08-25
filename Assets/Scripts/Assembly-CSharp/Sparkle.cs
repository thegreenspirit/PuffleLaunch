using System;
using UnityEngine;

public class Sparkle : MonoBehaviour
{
	private void Start()
	{
		base.GetComponent<SpriteManager>().animationend += this.FrameChangedEventHandler;
	}

	public void FrameChangedEventHandler(object sender, AnimationChangedEventArgs e)
	{
		if (e.anim.name == "PuffleOEffect")
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}

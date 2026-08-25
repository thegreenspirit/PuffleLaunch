using System;
using UnityEngine;

[RequireComponent(typeof(SpriteManager))]
public class PoofyCloud : MonoBehaviour
{
	public void Start()
	{
		this.mSpriteManager = base.GetComponent<SpriteManager>();
		this.mSpriteManager.animationend += this.OnAnimationEnd;
		Vector3 vector = Vector3.forward * 0.1f;
		base.transform.position -= vector;
		base.GetComponent<SphereCollider>().center += vector;
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (aOther.tag == "Player")
		{
			global::UnityEngine.Object.Destroy(base.GetComponent<SharedSpriteManager>());
			this.mSpriteManager.sharedMaterial = false;
			this.mSpriteManager.enabled = true;
			if (this.impactSound != null)
			{
				AudioManager.Instance.PlayObstacleSound(this.impactSound);
			}
		}
	}

	public void OnAnimationEnd(object sender, AnimationChangedEventArgs e)
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public AudioClip impactSound;

	private SpriteManager mSpriteManager;
}

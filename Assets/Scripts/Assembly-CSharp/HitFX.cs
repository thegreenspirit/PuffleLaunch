using System;
using UnityEngine;

public class HitFX : MonoBehaviour
{
	private void Start()
	{
		if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres)
		{
			base.transform.localScale *= 0.5f;
		}
		this.mNumEmitters = base.GetComponentsInChildren<ParticleEmitter>().Length;
		this.mDestroyedEmitters = 0;
		base.GetComponent<SpriteManager>().animationend += this.OnAnimationEnd;
	}

	private void Update()
	{
		if (this.mDestroyedEmitters == this.mNumEmitters)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void OnAnimationEnd(object sender, AnimationChangedEventArgs args)
	{
		base.GetComponent<MeshRenderer>().enabled = false;
		base.GetComponent<SpriteManager>().enabled = false;
	}

	public void OnEmitterEnd()
	{
		this.mDestroyedEmitters++;
	}

	private int mNumEmitters;

	private int mDestroyedEmitters;
}

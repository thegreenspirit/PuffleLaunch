using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	private bool mActiveCheckpoint;
	private MeshRenderer mFlagRenderer;
	private SpriteManager mFlagSpriteManager;
	private Transform mTransform;

	public void Start()
	{
		this.mTransform = base.transform;
		Transform transform = this.mTransform.Find("Flag");
		this.mFlagRenderer = transform.GetComponent<MeshRenderer>();
		this.mFlagSpriteManager = transform.GetComponent<SpriteManager>();
		this.mFlagRenderer.enabled = false;
		switch (ResolutionManager.Instance.AssetResolution)
		{
			case ResolutionManager.eAssetResolution.eLowres:
				transform.localPosition = new Vector3(-10f, 5.8f, 0f);
				break;
			case ResolutionManager.eAssetResolution.eIPad:
				break;
			default:
				transform.localPosition = new Vector3(-5f, 2.9f, 0f);
				break;
		}
	}

	public void Update()
	{
		if (this.mActiveCheckpoint && !Puffle.Instance.spawnPoint.Equals(this.mTransform.position))
		{
			this.mActiveCheckpoint = false;
			this.mFlagRenderer.enabled = false;
			this.mFlagSpriteManager.Seek(0);
		}
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (!this.mActiveCheckpoint && aOther.tag == "Player")
		{
			this.mActiveCheckpoint = true;
			this.mFlagRenderer.enabled = true;
			this.mFlagSpriteManager.Play("Flag");
			Puffle.Instance.spawnPoint = base.transform.position;
		}
	}
}

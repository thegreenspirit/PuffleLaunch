using System;
using UnityEngine;

public class GiantPuffleO : MonoBehaviour
{
	public void Start()
	{
		this.mSpriteManager = base.GetComponent<SpriteManager>();
		this.mSpriteManager.Play("GiantPuffleO");
		this.mSpriteManager.animationend += this.FrameChangedEventHandler;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Boss");
			if (gameObject)
			{
				EncounterZero component = gameObject.GetComponent<EncounterZero>();
				if (component)
				{
					component.OnGiantPuffleOCollect();
				}
			}
			other.transform.parent = base.transform;
			other.gameObject.SetActive(false);
			AudioManager.Instance.PlayObstacleSound(this.mReachedSound);
			if (LevelSelect.SelectedLevel - 1 == 23)
			{
				this.mSpriteManager.Play("GiantPuffleOBox");
			}
			else
			{
				this.mSpriteManager.Play("GiantPuffleOReach");
			}
		}
	}

	public void FrameChangedEventHandler(object sender, AnimationChangedEventArgs e)
	{
		if (!GameManager.Instance.EnableTiming)
		{
			return;
		}
		if (e.anim.name == "GiantPuffleOReach" || e.anim.name == "GiantPuffleOBox")
		{
			GameManager.Instance.EnableTiming = false;
			Puffle.Instance.transform.parent = null;
			GameManager.Instance.ShowEndLevelScreens();
		}
	}

	public AudioClip mReachedSound;

	private SpriteManager mSpriteManager;
}

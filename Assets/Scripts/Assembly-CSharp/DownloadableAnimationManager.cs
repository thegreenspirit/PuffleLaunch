using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadableAnimationManager : MonoBehaviour
{
	public void Attach(SpriteManager sprite)
	{
		sprite.animationstart += this.QueueAnimation;
	}

	public void AttachExtra(SpriteManager sprite)
	{
	}

	private void Start()
	{
		base.StartCoroutine(this.ProcessRequests());
	}

	public void QueueAnimation(object sender, AnimationChangedEventArgs e)
	{
		if (!e.anim.loaded)
		{
			this.QueueRequests(sender as SpriteManager, e.anim);
		}
	}

	public void UnloadAll()
	{
		base.StopAllCoroutines();
	}

	private void AddRequest(SpriteAnimation aAnim, SpriteManager aSprite)
	{
		SpriteManager extraSprite = aSprite.extraSprite;
		if (extraSprite != null)
		{
			SpriteAnimation animation = extraSprite.GetAnimation(aAnim.name);
			if (animation != null)
			{
				animation.extra.HookExtra(aSprite, aAnim);
			}
		}
	}

	private StreamingRequest FirstRequest()
	{
		if (this.m_Requests.Count == 0)
		{
			return null;
		}
		return this.m_Requests[0];
	}

	private void QueueRequests(SpriteManager aSprite, SpriteAnimation aAnim)
	{
		this.AddRequest(aAnim, aSprite);
	}

	public void PrefetchAnimation(SpriteManager aSprite, SpriteAnimation aAnim)
	{
		this.QueueRequests(aSprite, aAnim);
	}

	public IEnumerator ProcessRequests()
	{
		for (;;)
		{
			if (this.m_Requests.Count == 0)
			{
				yield return null;
			}
			else
			{
				StreamingRequest req = this.m_Requests[0];
				IEnumerator e = req.process();
				if (e != null)
				{
					while (e.MoveNext())
					{
						object obj = e.Current;
						yield return obj;
					}
				}
				this.m_Requests.RemoveAt(0);
			}
		}
		yield break;
	}

	public string BaseUrl;

	public bool debug;

	private List<StreamingRequest> m_Requests = new List<StreamingRequest>();
}

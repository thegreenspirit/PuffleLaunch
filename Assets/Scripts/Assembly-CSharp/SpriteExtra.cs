using System;
using UnityEngine;

[Serializable]
public class SpriteExtra
{
	public bool background;
	public int[] frames;
	public SpriteManager sprite;
	private SpriteManager m_ParentSprite;
	private SpriteAnimation m_Anim;
	private int m_LastFrame;

	public SpriteExtra(SpriteManager aSprite)
	{
		this.sprite = aSprite;
	}

	public void HookExtra(SpriteManager aSprite, SpriteAnimation aAnim)
	{
		this.m_Anim = aAnim;
		this.m_ParentSprite = aSprite;
		this.m_ParentSprite.framechanged += this.FrameChanged;
		this.m_ParentSprite.animationend += this.AnimationEnd;
		this.sprite.transform.parent = aSprite.transform;
		this.sprite.transform.localScale = new Vector3(1f, 1f, 1f);
		if (this.background)
		{
			this.sprite.transform.localPosition = new Vector3(0f, 0f, 0.2f);
		}
		else
		{
			this.sprite.transform.localPosition = new Vector3(0f, 0f, -0.2f);
		}
		this.m_LastFrame = 0;
	}

	public void AnimationEnd(object sender, AnimationChangedEventArgs e)
	{
		if (e.anim == this.m_Anim)
		{
			this.m_ParentSprite.framechanged -= this.FrameChanged;
			this.m_ParentSprite.animationend -= this.AnimationEnd;
			this.sprite.ClipApplied(null);
		}
	}

	public void FrameChanged(object sender, FrameChangedEventArgs e)
	{
		if (e.frame == this.m_LastFrame)
		{
			return;
		}
		if (e.name == this.m_Anim.name)
		{
			for (int i = 0; i < this.frames.Length; i++)
			{
				if (this.m_LastFrame < this.frames[i] && e.frame >= this.frames[i])
				{
					if (i == 0)
					{
						this.sprite.Play(this.m_Anim.name);
						this.sprite.Pause(true);
					}
					this.sprite.Seek(i);
					break;
				}
			}
			this.m_LastFrame = e.frame;
		}
	}
}

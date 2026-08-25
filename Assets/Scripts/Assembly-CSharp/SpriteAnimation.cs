using System;
using UnityEngine;

[Serializable]
public class SpriteAnimation
{
	public bool loaded
	{
		get { return this.current.loaded; }
	}

	public int frame
	{
		get { return this.m_CurrentFrame; }
	}

	protected SpriteClip current
	{
		get
		{
			if (this.m_CurrentClip == -1 || this.clips == null || this.m_CurrentClip >= this.clips.Length)
			{
				return null;
			}
			return this.clips[this.m_CurrentClip];
		}
	}

	public void SetIgnore(bool[] aIgnore)
	{
		foreach (SpriteClip spriteClip in this.clips)
		{
			spriteClip.SetIgnore(aIgnore);
		}
	}

	public void Reset()
	{
		this.m_CurrentClip = 0;
		this.m_CurrentFrame = 0;
		this.m_CurrentTime = 0f;
		this.current.Reset();
	}

	public void Seek(int aFrame)
	{
		this.Reset();
		this.m_CurrentFrame = aFrame;
		this.m_CurrentTime = (float)aFrame / (float)this.framerate;
		while (aFrame >= this.current.total)
		{
			if (this.m_CurrentClip == this.clips.Length)
			{
				break;
			}
			aFrame -= this.current.total;
			this.m_CurrentClip++;
		}
		if (this.m_CurrentClip != this.clips.Length)
		{
			this.current.Reset();
			this.current.Update(aFrame);
		}
		else
		{
			this.Reset();
		}
	}

	public void GoToLastFrame()
	{
		this.Seek(this.GetTotalNumFrame() - 1);
	}

	public int GetTotalNumFrame()
	{
		int num = 0;
		for (int i = 0; i < this.clips.Length; i++)
		{
			num += this.clips[i].total;
		}
		return num;
	}

	public void Apply(SpriteManager aManager, Material[] aMaterials)
	{
		if (this.current != null)
		{
			this.current.Apply(aManager, aMaterials);
		}
	}

	public float Update(SpriteManager aManager, float aDeltaTime)
	{
		if (this.framerate == 0)
		{
			return 0f;
		}
		this.m_CurrentTime += aDeltaTime;
		int num = (int)(this.m_CurrentTime * (float)this.framerate + 0.5f) - this.m_CurrentFrame;
		this.m_CurrentFrame += num;
		while (num != 0)
		{
			num = this.current.Update(num);
			if (num > 0)
			{
				this.m_CurrentClip++;
				if (this.m_CurrentClip == this.clips.Length)
				{
					this.Reset();
					if (!this.looping)
					{
						return (float)num / (float)this.framerate;
					}
				}
				num--;
				this.current.Reset();
			}
		}
		aManager.FrameChanged(this.name, this.m_CurrentFrame);
		return 0f;
	}

	public void Preload()
	{
		int num = 0;
		foreach (SpriteClip spriteClip in this.clips)
		{
			num += spriteClip.stringTiles.Length;
		}
		this.m_CachedClips = new Texture2D[num];
		int num2 = 0;
		foreach (SpriteClip spriteClip2 in this.clips)
		{
			foreach (string text in spriteClip2.stringTiles)
			{
				if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres && !spriteClip2.useHighRes)
				{
					this.m_CachedClips[num2] = Resources.Load(string.Format("{0}_lowres", text), typeof(Texture2D)) as Texture2D;
					Utilities.AssertMsg(this.m_CachedClips[num2] != null, string.Format("Low-res sprite sheet not found: {0}", text));
				}
				else
				{
					this.m_CachedClips[num2] = Resources.Load(text, typeof(Texture2D)) as Texture2D;
					Utilities.AssertMsg(this.m_CachedClips[num2] != null, string.Format("Sprite sheet not found: {0}", text));
				}
				num2++;
			}
		}
	}

	public string name;

	public string stream;

	public int framerate;

	public bool looping;

	public bool preload;

	public SpriteClip[] clips;

	public SpriteExtra extra;

	private int m_CurrentClip;

	private int m_CurrentFrame;

	private float m_CurrentTime;

	private Texture2D[] m_CachedClips;
}

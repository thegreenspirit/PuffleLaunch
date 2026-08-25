using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpriteManager : MonoBehaviour
{
	public event FrameChangedEventHandler framechanged;

	public event AnimationChangedEventHandler animationstart;

	public event AnimationChangedEventHandler animationend;

	public event AnimationChangedEventHandler animationfetched;

	public event ClipChangedEventHandler clipchanged;

	public SpriteManager extraSprite
	{
		get
		{
			return this.m_ExtraSprite;
		}
		set
		{
			this.m_ExtraSprite = value;
		}
	}

	public SpriteAnimation current
	{
		get
		{
			if (this.m_CurrentAnim == -1 || this.m_CurrentAnim >= this.animations.Length)
			{
				return null;
			}
			return this.animations[this.m_CurrentAnim];
		}
	}

	private void Awake()
	{
		this.m_MyTransform = base.transform;
		foreach (SpriteAnimation spriteAnimation in this.animations)
		{
			foreach (SpriteClip spriteClip in spriteAnimation.clips)
			{
				if (spriteClip.stringTiles.Length == 0)
				{
					Debug.LogWarning("missing path, please fix: " + base.gameObject.name);
				}
				foreach (string text in spriteClip.stringTiles)
				{
					if (text == string.Empty || text == null)
					{
						Debug.LogWarning("empty asset in path, please fix: " + base.gameObject.name);
					}
					if (text.Contains(".png"))
					{
						Debug.LogWarning("asset contains .png in path, please remove: " + text + ", " + base.gameObject.name);
					}
				}
			}
		}
		foreach (SpriteAnimation spriteAnimation2 in this.animations)
		{
			if (spriteAnimation2.preload)
			{
				spriteAnimation2.Preload();
			}
		}
		this.m_MeshGen = new SpriteMeshGenerator(base.GetComponent<MeshFilter>());
		this.clipchanged = (ClipChangedEventHandler)Delegate.Combine(this.clipchanged, new ClipChangedEventHandler(this.m_MeshGen.Generate));
		if (this.manager != null)
		{
			this.manager.Attach(this);
			if (this.m_ExtraSprite != null)
			{
				this.manager.AttachExtra(this.m_ExtraSprite);
			}
		}
		if (this.defaultOnStart)
		{
			this.m_CurrentAnim = this.defaultAnimation;
			if (this.current != null)
			{
				this.PlayInternal();
			}
		}
		MeshRenderer component = base.GetComponent<MeshRenderer>();
		if (!this.sharedMaterial)
		{
			for (int m = 0; m < component.materials.Length; m++)
			{
				component.materials[m] = new Material(component.materials[m]);
				component.materials[m].mainTexture = null;
			}
		}
		this.m_MeshRenderer = component;
	}

	private void Update()
	{
		if (StartOfGameDelay.Instance != null)
		{
			this.m_MyPos = this.m_MyTransform.position;
			this.m_MyTransform.position = this.m_MyPos;
		}
		if (this.current != null && this.current.loaded && !this.m_Paused)
		{
			if (!base.gameObject.GetComponent<Renderer>().isVisible && !this.sharedMaterial && StartOfGameDelay.Instance == null)
			{
				return;
			}
			float num = this.current.Update(this, Time.deltaTime);
			if (num > 0f)
			{
				this.AnimationEnd(this.current);
				this.m_CurrentAnim = this.defaultAnimation;
				if (this.current == null)
				{
					return;
				}
				this.PlayInternal();
			}
			if (this.current.loaded)
			{
				if (this.sharedMaterial)
				{
					this.current.Apply(this, this.m_MeshRenderer.sharedMaterials);
				}
				else
				{
					this.current.Apply(this, this.m_MeshRenderer.materials);
				}
			}
		}
	}

	public void ClipApplied(SpriteClip aNewClip)
	{
		if (aNewClip != this.m_CurrentClip)
		{
			if (this.clipchanged != null)
			{
				this.clipchanged(this, new ClipChangedEventArgs(this.m_CurrentClip, aNewClip));
			}
			this.m_CurrentClip = aNewClip;
		}
	}

	public void FrameChanged(string aName, int aFrame)
	{
		if (this.framechanged != null)
		{
			this.framechanged(this, new FrameChangedEventArgs(aName, aFrame));
		}
	}

	public void AnimationEnd(SpriteAnimation aAnimation)
	{
		if (this.animationend != null)
		{
			this.animationend(this, new AnimationChangedEventArgs(aAnimation));
		}
	}

	public void AnimationStart(SpriteAnimation aAnimation)
	{
		if (this.animationstart != null)
		{
			this.animationstart(this, new AnimationChangedEventArgs(aAnimation));
		}
	}

	public void AnimationFetched(SpriteAnimation aAnimation)
	{
		if (this.animationfetched != null)
		{
			this.animationfetched(this, new AnimationChangedEventArgs(aAnimation));
		}
	}

	public void Reset()
	{
		this.m_CurrentAnim = this.defaultAnimation;
	}

	public SpriteAnimation GetExtraAnimation(string aName)
	{
		if (this.extraSprite != null)
		{
			return this.extraSprite.GetAnimation(aName);
		}
		return null;
	}

	public SpriteAnimation GetAnimation(string aName)
	{
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (aName == this.animations[i].name)
			{
				return this.animations[i];
			}
		}
		return null;
	}

	public void GoToLastFrame()
	{
		if (this.current != null)
		{
			this.current.GoToLastFrame();
			MeshRenderer component = base.GetComponent<MeshRenderer>();
			if (this.sharedMaterial)
			{
				this.current.Apply(this, component.sharedMaterials);
			}
			else
			{
				this.current.Apply(this, component.materials);
			}
		}
	}

	public int GetCurrAnimTotalFrames()
	{
		return this.current.GetTotalNumFrame();
	}

	public bool Contains(string aName)
	{
		return this.GetAnimation(aName) != null;
	}

	private void PlayInternal()
	{
		this.current.Reset();
		this.m_Paused = false;
		this.AnimationStart(this.current);
	}

	public void Pause(bool aPause)
	{
		this.m_Paused = aPause;
	}

	public bool Play(string aName)
	{
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (aName == this.animations[i].name)
			{
				if (this.current != null)
				{
					this.AnimationEnd(this.current);
				}
				this.m_CurrentAnim = i;
				this.PlayInternal();
				return true;
			}
		}
		return false;
	}

	public bool Play(int index)
	{
		if (index < this.animations.Length)
		{
			if (this.current != null)
			{
				this.AnimationEnd(this.current);
			}
			this.m_CurrentAnim = index;
			this.PlayInternal();
			return true;
		}
		return false;
	}

	public bool Prefetch(string aName)
	{
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (aName == this.animations[i].name && this.manager != null)
			{
				this.manager.PrefetchAnimation(this, this.animations[i]);
				return true;
			}
		}
		return false;
	}

	public string AnimationPlaying()
	{
		return this.current.name;
	}

	public int CurrentAnimation()
	{
		return this.m_CurrentAnim;
	}

	public void Seek(int aFrame)
	{
		if (this.current != null)
		{
			this.current.Seek(aFrame);
			if (this.current.loaded)
			{
				MeshRenderer component = base.GetComponent<MeshRenderer>();
				if (this.sharedMaterial)
				{
					this.current.Apply(this, component.sharedMaterials);
				}
				else
				{
					this.current.Apply(this, component.materials);
				}
			}
		}
	}

	public void SetIgnore(bool[] aIgnore)
	{
		foreach (SpriteAnimation spriteAnimation in this.animations)
		{
			spriteAnimation.SetIgnore(aIgnore);
		}
	}

	public void MergeInto(SpriteManager other)
	{
		if (other == null)
		{
			return;
		}
		this.MergeInto(other.animations);
	}

	public void MergeInto(SpriteAnimation[] other)
	{
		if (other == null)
		{
			return;
		}
		SpriteAnimation[] array = this.animations;
		this.animations = new SpriteAnimation[((array == null) ? 0 : array.Length) + ((other == null) ? 0 : other.Length)];
		int num = 0;
		if (array != null)
		{
			foreach (SpriteAnimation spriteAnimation in array)
			{
				this.animations[num++] = spriteAnimation;
			}
		}
		if (other != null)
		{
			foreach (SpriteAnimation spriteAnimation2 in other)
			{
				if (spriteAnimation2.extra != null)
				{
					spriteAnimation2.extra.sprite = this;
				}
				this.animations[num++] = spriteAnimation2;
			}
		}
	}

	public void MergeInto(SpriteAnimation other)
	{
		if (other == null)
		{
			return;
		}
		this.MergeInto(new SpriteAnimation[] { other });
	}

	public int defaultAnimation;

	public bool defaultOnStart = true;

	public DownloadableAnimationManager manager;

	public SpriteAnimation[] animations;

	public bool sharedMaterial;

	public bool zoomInvariant;

	private SpriteManager m_ExtraSprite;

	private int m_CurrentAnim = -1;

	private SpriteMeshGenerator m_MeshGen;

	private MeshRenderer m_MeshRenderer;

	private SpriteClip m_CurrentClip;

	private bool m_Paused;

	private Transform m_MyTransform;

	private Vector3 m_MyPos;
}

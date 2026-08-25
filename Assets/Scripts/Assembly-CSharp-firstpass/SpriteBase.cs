using System;
using UnityEngine;

[ExecuteInEditMode]
public abstract class SpriteBase : SpriteRoot, ISpriteAnimatable
{
	protected override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		if (this.m_spriteMesh != null)
		{
			this.m_spriteMesh.UseUV2 = this.crossfadeFrames;
		}
	}

	public override void Clear()
	{
		base.Clear();
		this.animCompleteDelegate = null;
	}

	public override void Delete()
	{
		if (this.animating)
		{
			this.RemoveFromAnimatedList();
			this.animating = true;
		}
		base.Delete();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (this.animating)
		{
			this.RemoveFromAnimatedList();
			this.animating = true;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.animating)
		{
			this.animating = false;
			this.AddToAnimatedList();
		}
	}

	public override void Copy(SpriteRoot s)
	{
		base.Copy(s);
		if (!(s is SpriteBase))
		{
			return;
		}
		SpriteBase spriteBase = (SpriteBase)s;
		this.defaultAnim = spriteBase.defaultAnim;
		this.playAnimOnStart = spriteBase.playAnimOnStart;
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);
		if (tf)
		{
			this.PauseAnim();
		}
	}

	public void SetAnimCompleteDelegate(SpriteBase.AnimCompleteDelegate del)
	{
		this.animCompleteDelegate = del;
	}

	public void SetAnimFrameDelegate(SpriteBase.AnimFrameDelegate del)
	{
		this.animFrameDelegate = del;
	}

	public void SetSpriteResizedDelegate(SpriteRoot.SpriteResizedDelegate del)
	{
		this.resizedDelegate = del;
	}

	public void AddSpriteResizedDelegate(SpriteRoot.SpriteResizedDelegate del)
	{
		this.resizedDelegate = (SpriteRoot.SpriteResizedDelegate)Delegate.Combine(this.resizedDelegate, del);
	}

	public void RemoveSpriteresizedDelegate(SpriteRoot.SpriteResizedDelegate del)
	{
		this.resizedDelegate = (SpriteRoot.SpriteResizedDelegate)Delegate.Remove(this.resizedDelegate, del);
	}

	public virtual bool StepAnim(float time)
	{
		return false;
	}

	public virtual void PlayAnim(int index)
	{
	}

	public virtual void PlayAnim(string name)
	{
	}

	public virtual void PlayAnimInReverse(int index)
	{
	}

	public virtual void PlayAnimInReverse(string name)
	{
	}

	public void SetFramerate(float fps)
	{
		this.timeBetweenAnimFrames = 1f / fps;
	}

	public void PauseAnim()
	{
		if (this.animating)
		{
			this.RemoveFromAnimatedList();
		}
	}

	public virtual void StopAnim()
	{
	}

	public void RevertToStatic()
	{
		if (this.animating)
		{
			this.StopAnim();
		}
		this.InitUVs();
		base.SetBleedCompensation();
		if (this.autoResize || this.pixelPerfect)
		{
			base.CalcSize();
		}
	}

	protected abstract void AddToAnimatedList();

	protected abstract void RemoveFromAnimatedList();

	public bool IsAnimating()
	{
		return this.animating;
	}

	public bool Animating
	{
		get
		{
			return this.animating;
		}
		set
		{
			if (value)
			{
				this.PlayAnim(this.curAnimIndex);
			}
		}
	}

	public int CurAnimIndex
	{
		get
		{
			return this.curAnimIndex;
		}
		set
		{
			this.curAnimIndex = value;
		}
	}

	public bool playAnimOnStart;

	[HideInInspector]
	public bool crossfadeFrames;

	public int defaultAnim;

	protected int curAnimIndex;

	protected SpriteBase.AnimCompleteDelegate animCompleteDelegate;

	protected SpriteBase.AnimFrameDelegate animFrameDelegate;

	protected float timeSinceLastFrame;

	protected float timeBetweenAnimFrames;

	protected float framesToAdvance;

	protected bool animating;

	protected SPRITE_FRAME nextFrameInfo = new SPRITE_FRAME(0f);

	public delegate void AnimCompleteDelegate(SpriteBase sprite);

	public delegate void AnimFrameDelegate(SpriteBase sprite, int frame);
}

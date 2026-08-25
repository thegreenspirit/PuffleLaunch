using System;
using System.Collections;
using UnityEngine;

public abstract class AutoSpriteBase : SpriteBase, ISpriteAggregator, ISpritePackable
{
	public abstract TextureAnim[] States { get; set; }

	public virtual CSpriteFrame DefaultFrame
	{
		get
		{
			if (this.States[0].spriteFrames.Length != 0)
			{
				return this.States[0].spriteFrames[0];
			}
			return null;
		}
	}

	public virtual TextureAnim DefaultState
	{
		get
		{
			if (this.States != null && this.States.Length != 0)
			{
				return this.States[0];
			}
			return null;
		}
	}

	public override Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader)
	{
		TextureAnim defaultState = this.DefaultState;
		CSpriteFrame defaultFrame = this.DefaultFrame;
		if (defaultState == null)
		{
			return Vector2.zero;
		}
		if (defaultState.frameGUIDs == null)
		{
			return Vector2.zero;
		}
		if (defaultState.frameGUIDs.Length == 0)
		{
			return Vector2.zero;
		}
		if (defaultFrame == null)
		{
			Debug.LogWarning("Sprite \"" + base.name + "\" does not seem to have been built to an atlas yet.");
			return Vector2.zero;
		}
		Vector2 zero = Vector2.zero;
		Texture2D texture2D = (Texture2D)loader(guid2Path(defaultState.frameGUIDs[0]), typeof(Texture2D));
		if (texture2D == null)
		{
			if (base.spriteMesh != null)
			{
				texture2D = (Texture2D)base.spriteMesh.material.GetTexture("_MainTex");
				zero = new Vector2(defaultFrame.uvs.width * (float)texture2D.width, defaultFrame.uvs.height * (float)texture2D.height);
			}
		}
		else
		{
			zero = new Vector2((float)texture2D.width * (1f / (defaultFrame.scaleFactor.x * 2f)), (float)texture2D.height * (1f / (defaultFrame.scaleFactor.y * 2f)));
		}
		return zero;
	}

	protected override void Awake()
	{
		base.Awake();
		this.animations = new UVAnimation[this.States.Length];
		for (int i = 0; i < this.States.Length; i++)
		{
			this.animations[i] = new UVAnimation();
			this.animations[i].SetAnim(this.States[i], i);
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (this.curAnim != null)
		{
			base.PauseAnim();
			this.curAnim = null;
		}
	}

	public void Setup(float w, float h)
	{
		this.Setup(w, h, this.m_spriteMesh.material);
	}

	public void Setup(float w, float h, Material material)
	{
		this.width = w;
		this.height = h;
		if (!this.managed)
		{
			((SpriteMesh)this.m_spriteMesh).material = material;
		}
		this.Init();
	}

	public override void Copy(SpriteRoot s)
	{
		base.Copy(s);
		if (!(s is AutoSpriteBase))
		{
			return;
		}
		AutoSpriteBase autoSpriteBase = (AutoSpriteBase)s;
		if (autoSpriteBase.spriteMesh != null)
		{
			if (autoSpriteBase.animations.Length > 0)
			{
				this.animations = new UVAnimation[autoSpriteBase.animations.Length];
				for (int i = 0; i < this.animations.Length; i++)
				{
					this.animations[i] = autoSpriteBase.animations[i].Clone();
				}
			}
		}
		else if (this.States != null)
		{
			this.animations = new UVAnimation[autoSpriteBase.States.Length];
			for (int j = 0; j < autoSpriteBase.States.Length; j++)
			{
				this.animations[j] = new UVAnimation();
				this.animations[j].SetAnim(autoSpriteBase.States[j], j);
			}
		}
	}

	public virtual void CopyAll(SpriteRoot s)
	{
		base.Copy(s);
		if (!(s is AutoSpriteBase))
		{
			return;
		}
		AutoSpriteBase autoSpriteBase = (AutoSpriteBase)s;
		this.States = new TextureAnim[autoSpriteBase.States.Length];
		for (int i = 0; i < this.States.Length; i++)
		{
			this.States[i] = new TextureAnim();
			this.States[i].Copy(autoSpriteBase.States[i]);
		}
		this.animations = new UVAnimation[this.States.Length];
		for (int j = 0; j < this.States.Length; j++)
		{
			this.animations[j] = new UVAnimation();
			this.animations[j].SetAnim(this.States[j], j);
		}
		this.doNotTrimImages = autoSpriteBase.doNotTrimImages;
	}

	public override bool StepAnim(float time)
	{
		if (this.curAnim == null)
		{
			return false;
		}
		this.timeSinceLastFrame += Mathf.Max(0f, time);
		this.framesToAdvance = this.timeSinceLastFrame / this.timeBetweenAnimFrames;
		if (this.framesToAdvance < 1f)
		{
			if (this.crossfadeFrames)
			{
				this.SetColor(new Color(1f, 1f, 1f, 1f - this.framesToAdvance));
			}
			return true;
		}
		while (this.framesToAdvance >= 1f)
		{
			if (!this.curAnim.GetNextFrame(ref this.frameInfo))
			{
				if (this.crossfadeFrames)
				{
					this.SetColor(Color.white);
				}
				switch (this.curAnim.onAnimEnd)
				{
				case UVAnimation.ANIM_END_ACTION.Do_Nothing:
					base.PauseAnim();
					this.uvRect = this.frameInfo.uvs;
					base.SetBleedCompensation();
					if (this.autoResize || this.pixelPerfect)
					{
						base.CalcSize();
					}
					break;
				case UVAnimation.ANIM_END_ACTION.Revert_To_Static:
					base.RevertToStatic();
					break;
				case UVAnimation.ANIM_END_ACTION.Play_Default_Anim:
					if (this.animCompleteDelegate != null)
					{
						this.animCompleteDelegate(this);
					}
					this.PlayAnim(this.defaultAnim);
					return false;
				case UVAnimation.ANIM_END_ACTION.Hide:
					this.Hide(true);
					break;
				case UVAnimation.ANIM_END_ACTION.Deactivate:
					base.gameObject.active = false;
					break;
				case UVAnimation.ANIM_END_ACTION.Destroy:
					if (this.animCompleteDelegate != null)
					{
						this.animCompleteDelegate(this);
					}
					this.Delete();
					global::UnityEngine.Object.Destroy(base.gameObject);
					break;
				}
				if (this.animCompleteDelegate != null)
				{
					this.animCompleteDelegate(this);
				}
				if (!this.animating)
				{
					this.curAnim = null;
				}
				return false;
			}
			this.framesToAdvance -= 1f;
			this.timeSinceLastFrame -= this.timeBetweenAnimFrames;
		}
		if (this.crossfadeFrames)
		{
			int curPosition = this.curAnim.GetCurPosition();
			int stepDirection = this.curAnim.StepDirection;
			this.curAnim.GetNextFrame(ref this.nextFrameInfo);
			Vector2[] uvs = this.m_spriteMesh.uvs2;
			Rect uvs2 = this.nextFrameInfo.uvs;
			uvs[0].x = uvs2.xMin;
			uvs[0].y = uvs2.yMax;
			uvs[1].x = uvs2.xMin;
			uvs[1].y = uvs2.yMin;
			uvs[2].x = uvs2.xMax;
			uvs[2].y = uvs2.yMin;
			uvs[3].x = uvs2.xMax;
			uvs[3].y = uvs2.yMax;
			this.curAnim.SetCurrentFrame(curPosition);
			this.curAnim.StepDirection = stepDirection;
			this.SetColor(new Color(1f, 1f, 1f, 1f - this.framesToAdvance));
		}
		this.uvRect = this.frameInfo.uvs;
		base.SetBleedCompensation();
		if (this.autoResize || this.pixelPerfect)
		{
			base.CalcSize();
		}
		else if (this.anchor == SpriteRoot.ANCHOR_METHOD.TEXTURE_OFFSET)
		{
			this.SetSize(this.width, this.height);
		}
		return true;
	}

	public void PlayAnim(UVAnimation anim, int frame)
	{
		if (this.deleted || !base.gameObject.active)
		{
			return;
		}
		if (!this.m_started)
		{
			this.Start();
		}
		this.curAnim = anim;
		this.curAnimIndex = this.curAnim.index;
		this.curAnim.Reset();
		this.curAnim.SetCurrentFrame(frame - 1);
		if (anim.framerate != 0f)
		{
			this.timeBetweenAnimFrames = 1f / anim.framerate;
		}
		else
		{
			this.timeBetweenAnimFrames = 1f;
		}
		this.timeSinceLastFrame = this.timeBetweenAnimFrames;
		if ((anim.GetFrameCount() > 1 || anim.onAnimEnd != UVAnimation.ANIM_END_ACTION.Do_Nothing) && anim.framerate != 0f)
		{
			this.StepAnim(0f);
			if (!this.animating)
			{
				this.AddToAnimatedList();
			}
		}
		else
		{
			base.PauseAnim();
			if (this.animCompleteDelegate != null)
			{
				this.animCompleteDelegate(this);
			}
			this.StepAnim(0f);
		}
	}

	public void PlayAnim(UVAnimation anim)
	{
		this.PlayAnim(anim, 0);
	}

	public void PlayAnim(int index, int frame)
	{
		if (index >= this.animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
			return;
		}
		this.PlayAnim(this.animations[index], frame);
	}

	public override void PlayAnim(int index)
	{
		if (index >= this.animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
			return;
		}
		this.PlayAnim(this.animations[index], 0);
	}

	public void PlayAnim(string name, int frame)
	{
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (this.animations[i].name == name)
			{
				this.PlayAnim(this.animations[i], frame);
				return;
			}
		}
		Debug.LogError("ERROR: Animation \"" + name + "\" not found!");
	}

	public override void PlayAnim(string name)
	{
		this.PlayAnim(name, 0);
	}

	public void PlayAnimInReverse(UVAnimation anim)
	{
		if (this.deleted || !base.gameObject.active)
		{
			return;
		}
		this.curAnim = anim;
		this.curAnim.Reset();
		this.curAnim.PlayInReverse();
		if (anim.framerate != 0f)
		{
			this.timeBetweenAnimFrames = 1f / anim.framerate;
		}
		else
		{
			this.timeBetweenAnimFrames = 1f;
		}
		this.timeSinceLastFrame = this.timeBetweenAnimFrames;
		if ((anim.GetFrameCount() > 1 || anim.onAnimEnd != UVAnimation.ANIM_END_ACTION.Do_Nothing) && anim.framerate != 0f)
		{
			this.StepAnim(0f);
			if (!this.animating)
			{
				this.AddToAnimatedList();
			}
		}
		else
		{
			base.PauseAnim();
			if (this.animCompleteDelegate != null)
			{
				this.animCompleteDelegate(this);
			}
			this.StepAnim(0f);
		}
	}

	public void PlayAnimInReverse(UVAnimation anim, int frame)
	{
		if (this.deleted || !base.gameObject.active)
		{
			return;
		}
		if (!this.m_started)
		{
			this.Start();
		}
		this.curAnim = anim;
		this.curAnim.Reset();
		this.curAnim.PlayInReverse();
		this.curAnim.SetCurrentFrame(frame + 1);
		anim.framerate = Mathf.Max(0.0001f, anim.framerate);
		this.timeBetweenAnimFrames = 1f / anim.framerate;
		this.timeSinceLastFrame = this.timeBetweenAnimFrames;
		if (anim.GetFrameCount() > 1)
		{
			this.StepAnim(0f);
			if (!this.animating)
			{
				this.AddToAnimatedList();
			}
		}
		else
		{
			if (this.animCompleteDelegate != null)
			{
				this.animCompleteDelegate(this);
			}
			this.StepAnim(0f);
		}
	}

	public override void PlayAnimInReverse(int index)
	{
		if (index >= this.animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
			return;
		}
		this.PlayAnimInReverse(this.animations[index]);
	}

	public void PlayAnimInReverse(int index, int frame)
	{
		if (index >= this.animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
			return;
		}
		this.PlayAnimInReverse(this.animations[index], frame);
	}

	public override void PlayAnimInReverse(string name)
	{
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (this.animations[i].name == name)
			{
				this.animations[i].PlayInReverse();
				this.PlayAnimInReverse(this.animations[i]);
				return;
			}
		}
		Debug.LogError("ERROR: Animation \"" + name + "\" not found!");
	}

	public void PlayAnimInReverse(string name, int frame)
	{
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (this.animations[i].name == name)
			{
				this.animations[i].PlayInReverse();
				this.PlayAnimInReverse(this.animations[i], frame);
				return;
			}
		}
		Debug.LogError("ERROR: Animation \"" + name + "\" not found!");
	}

	public void DoAnim(int index)
	{
		if (this.curAnim == null)
		{
			this.PlayAnim(index);
		}
		else if (this.curAnim.index != index || !this.animating)
		{
			this.PlayAnim(index);
		}
	}

	public void DoAnim(string name)
	{
		if (this.curAnim == null)
		{
			this.PlayAnim(name);
		}
		else if (this.curAnim.name != name || !this.animating)
		{
			this.PlayAnim(name);
		}
	}

	public void DoAnim(UVAnimation anim)
	{
		if (this.curAnim != anim || !this.animating)
		{
			this.PlayAnim(anim);
		}
	}

	public void SetCurFrame(int index)
	{
		if (this.curAnim == null)
		{
			return;
		}
		if (!this.m_started)
		{
			this.Start();
		}
		this.curAnim.SetCurrentFrame(index - this.curAnim.StepDirection);
		this.timeSinceLastFrame = this.timeBetweenAnimFrames;
		this.StepAnim(0f);
	}

	public override void StopAnim()
	{
		this.RemoveFromAnimatedList();
		if (this.curAnim != null)
		{
			this.curAnim.Reset();
		}
		base.RevertToStatic();
	}

	public void UnpauseAnim()
	{
		if (this.curAnim == null)
		{
			return;
		}
		this.AddToAnimatedList();
	}

	protected override void AddToAnimatedList()
	{
		if (this.animating || !Application.isPlaying || !base.gameObject.active)
		{
			return;
		}
		this.animating = true;
		SpriteAnimationPump.Add(this);
	}

	protected override void RemoveFromAnimatedList()
	{
		SpriteAnimationPump.Remove(this);
		this.animating = false;
	}

	public UVAnimation GetCurAnim()
	{
		return this.curAnim;
	}

	public UVAnimation GetAnim(string name)
	{
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (this.animations[i].name == name)
			{
				return this.animations[i];
			}
		}
		return null;
	}

	public override int GetStateIndex(string stateName)
	{
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (string.Equals(this.animations[i].name, stateName, StringComparison.CurrentCultureIgnoreCase))
			{
				return i;
			}
		}
		return -1;
	}

	public override void SetState(int index)
	{
		this.PlayAnim(index);
	}

	public virtual bool SupportsArbitraryAnimations
	{
		get
		{
			return false;
		}
	}

	public virtual Material GetPackedMaterial(out string errString)
	{
		errString = "Sprite \"" + base.name + "\" has not been assigned a material, and can therefore not be included in the atlas build.";
		if (base.spriteMesh == null)
		{
			if (!this.managed)
			{
				return base.GetComponent<Renderer>().sharedMaterial;
			}
			if (this.manager != null)
			{
				return this.manager.GetComponent<Renderer>().sharedMaterial;
			}
			errString = "Sprite \"" + base.name + "\" is not associated with a SpriteManager, and can therefore not be included in the atlas build.";
			return null;
		}
		else
		{
			if (!this.managed)
			{
				return base.spriteMesh.material;
			}
			if (this.manager != null)
			{
				return this.manager.GetComponent<Renderer>().sharedMaterial;
			}
			errString = "Sprite \"" + base.name + "\" is not associated with a SpriteManager, and can therefore not be included in the atlas build.";
			return null;
		}
	}

	public virtual bool DoNotTrimImages
	{
		get
		{
			return this.doNotTrimImages;
		}
		set
		{
			this.doNotTrimImages = value;
		}
	}

	public virtual void Aggregate(PathFromGUIDDelegate guid2Path, LoadAssetDelegate load, GUIDFromPathDelegate path2Guid)
	{
		ArrayList arrayList = new ArrayList();
		ArrayList arrayList2 = new ArrayList();
		for (int i = 0; i < this.States.Length; i++)
		{
			this.States[i].Allocate();
			if (this.States[i].frameGUIDs.Length >= this.States[i].framePaths.Length)
			{
				for (int j = 0; j < this.States[i].frameGUIDs.Length; j++)
				{
					string text = guid2Path(this.States[i].frameGUIDs[j]);
					arrayList.Add(load(text, typeof(Texture2D)));
					arrayList2.Add(this.States[i].spriteFrames[j]);
				}
				this.States[i].framePaths = new string[0];
			}
			else
			{
				this.States[i].frameGUIDs = new string[this.States[i].framePaths.Length];
				this.States[i].spriteFrames = new CSpriteFrame[this.States[i].framePaths.Length];
				for (int k = 0; k < this.States[i].spriteFrames.Length; k++)
				{
					this.States[i].spriteFrames[k] = new CSpriteFrame();
				}
				for (int l = 0; l < this.States[i].framePaths.Length; l++)
				{
					this.States[i].frameGUIDs[l] = path2Guid(this.States[i].framePaths[l]);
					arrayList.Add(load(this.States[i].framePaths[l], typeof(Texture2D)));
					arrayList2.Add(this.States[i].spriteFrames[l]);
				}
			}
		}
		this.sourceTextures = (Texture2D[])arrayList.ToArray(typeof(Texture2D));
		this.spriteFrames = (CSpriteFrame[])arrayList2.ToArray(typeof(CSpriteFrame));
	}

	public Texture2D[] SourceTextures
	{
		get
		{
			return this.sourceTextures;
		}
	}

	public CSpriteFrame[] SpriteFrames
	{
		get
		{
			return this.spriteFrames;
		}
	}

	void ISpriteAggregator.SetUVs(Rect uvs)
	{
		base.SetUVs(uvs);
	}

	GameObject ISpriteAggregator.gameObject
	{
		get { return base.gameObject; }
	}

	GameObject ISpritePackable.gameObject
	{
		get { return base.gameObject; }
	}

	SpriteRoot.ANCHOR_METHOD ISpritePackable.Anchor
	{
		get { return base.Anchor; }
	}

	Color ISpritePackable.Color
	{
		get { return base.Color; }
		set { base.Color = value; }
	}

	protected Texture2D[] sourceTextures;

	protected CSpriteFrame[] spriteFrames;

	public bool doNotTrimImages;

	[HideInInspector]
	public UVAnimation[] animations;

	protected UVAnimation curAnim;
}

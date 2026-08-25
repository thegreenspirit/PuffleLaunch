using System;
using UnityEngine;

[ExecuteInEditMode]
public class SimpleSprite : SpriteRoot
{
	public override Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader)
	{
		return this.pixelDimensions;
	}

	protected override void Awake()
	{
		base.Awake();
		this.Init();
	}

	protected override void Init()
	{
		this.nullCamera = this.renderCamera == null;
		base.Init();
	}

	public override void Start()
	{
		base.Start();
		if (UIManager.Exists() && this.nullCamera && UIManager.instance.uiCameras.Length > 0)
		{
			this.SetCamera(UIManager.instance.uiCameras[0].camera);
		}
	}

	public override void Clear()
	{
		base.Clear();
	}

	public void Setup(float w, float h, Vector2 lowerleftPixel, Vector2 pixeldimensions)
	{
		this.Setup(w, h, lowerleftPixel, pixeldimensions, this.m_spriteMesh.material);
	}

	public void Setup(float w, float h, Vector2 lowerleftPixel, Vector2 pixeldimensions, Material material)
	{
		this.width = w;
		this.height = h;
		this.lowerLeftPixel = lowerleftPixel;
		this.pixelDimensions = pixeldimensions;
		this.uvsInitialized = false;
		if (!this.managed)
		{
			((SpriteMesh)this.m_spriteMesh).material = material;
		}
		this.Init();
	}

	public override void Copy(SpriteRoot s)
	{
		base.Copy(s);
		if (!(s is SimpleSprite))
		{
			return;
		}
		this.lowerLeftPixel = ((SimpleSprite)s).lowerLeftPixel;
		this.pixelDimensions = ((SimpleSprite)s).pixelDimensions;
		this.InitUVs();
		base.SetBleedCompensation(s.bleedCompensation);
		if (this.autoResize || this.pixelPerfect)
		{
			base.CalcSize();
		}
		else
		{
			this.SetSize(s.width, s.height);
		}
	}

	public override void InitUVs()
	{
		this.tempUV = base.PixelCoordToUVCoord(this.lowerLeftPixel);
		this.uvRect.x = this.tempUV.x;
		this.uvRect.y = this.tempUV.y;
		this.tempUV = base.PixelSpaceToUVSpace(this.pixelDimensions);
		this.uvRect.xMax = this.uvRect.x + this.tempUV.x;
		this.uvRect.yMax = this.uvRect.y + this.tempUV.y;
		this.frameInfo.uvs = this.uvRect;
		base.InitUVs();
	}

	public void SetLowerLeftPixel(Vector2 lowerLeft)
	{
		this.lowerLeftPixel = lowerLeft;
		this.tempUV = base.PixelCoordToUVCoord(this.lowerLeftPixel);
		this.uvRect.x = this.tempUV.x;
		this.uvRect.y = this.tempUV.y;
		this.tempUV = base.PixelSpaceToUVSpace(this.pixelDimensions);
		this.uvRect.xMax = this.uvRect.x + this.tempUV.x;
		this.uvRect.yMax = this.uvRect.y + this.tempUV.y;
		this.frameInfo.uvs = this.uvRect;
		base.SetBleedCompensation(this.bleedCompensation);
		if (this.autoResize || this.pixelPerfect)
		{
			base.CalcSize();
		}
	}

	public void SetLowerLeftPixel(int x, int y)
	{
		this.SetLowerLeftPixel(new Vector2((float)x, (float)y));
	}

	public void SetPixelDimensions(Vector2 size)
	{
		this.pixelDimensions = size;
		this.tempUV = base.PixelSpaceToUVSpace(this.pixelDimensions);
		this.uvRect.xMax = this.uvRect.x + this.tempUV.x;
		this.uvRect.yMax = this.uvRect.y + this.tempUV.y;
		this.frameInfo.uvs = this.uvRect;
		if (this.autoResize || this.pixelPerfect)
		{
			base.CalcSize();
		}
	}

	public void SetPixelDimensions(int x, int y)
	{
		this.SetPixelDimensions(new Vector2((float)x, (float)y));
	}

	public override int GetStateIndex(string stateName)
	{
		return -1;
	}

	public override void SetState(int index)
	{
	}

	public static SimpleSprite Create(string name, Vector3 pos)
	{
		return (SimpleSprite)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(SimpleSprite));
	}

	public static SimpleSprite Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (SimpleSprite)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(SimpleSprite));
	}

	public override void DoMirror()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (this.screenSize.x == 0f || this.screenSize.y == 0f)
		{
			base.Start();
		}
		if (this.mirror == null)
		{
			this.mirror = new SimpleSpriteMirror();
			this.mirror.Mirror(this);
		}
		this.mirror.Validate(this);
		if (this.mirror.DidChange(this))
		{
			this.Init();
			this.mirror.Mirror(this);
		}
	}

	public Vector2 lowerLeftPixel;

	public Vector2 pixelDimensions;

	protected bool nullCamera;
}

using System;
using UnityEngine;

[ExecuteInEditMode]
public abstract class SpriteRoot : MonoBehaviour, IEZLinkedListItem<ISpriteAnimatable>, IUseCamera
{
	protected virtual void Awake()
	{
		this.screenSize.x = 0f;
		this.screenSize.y = 0f;
		if (base.name.EndsWith("(Clone)"))
		{
			this.isClone = true;
		}
		if (!this.managed)
		{
			MeshFilter meshFilter = (MeshFilter)base.GetComponent(typeof(MeshFilter));
			if (meshFilter != null)
			{
				this.oldMesh = meshFilter.sharedMesh;
				meshFilter.sharedMesh = null;
			}
			this.AddMesh();
		}
		else if (this.manager != null)
		{
			this.manager.AddSprite(this);
		}
		else
		{
			Debug.LogError("Managed sprite \"" + base.name + "\" has not been assigned to a SpriteManager!");
		}
	}

	public virtual void Start()
	{
		this.m_started = true;
		if (!this.managed)
		{
			if (Application.isPlaying)
			{
				if (!this.isClone)
				{
					global::UnityEngine.Object.Destroy(this.oldMesh);
				}
				this.oldMesh = null;
			}
		}
		else if (this.m_spriteMesh != null)
		{
			this.Init();
		}
		if (this.persistent)
		{
			global::UnityEngine.Object.DontDestroyOnLoad(this);
			if (this.m_spriteMesh is SpriteMesh)
			{
				((SpriteMesh)this.m_spriteMesh).SetPersistent();
			}
		}
		if (this.m_spriteMesh == null && !this.managed)
		{
			this.AddMesh();
		}
		this.CalcSizeUnitsPerUV();
		if (this.m_spriteMesh != null && this.m_spriteMesh.texture != null)
		{
			this.SetPixelToUV(this.m_spriteMesh.texture);
		}
		if (this.renderCamera == null)
		{
			this.renderCamera = Camera.main;
		}
		this.SetCamera(this.renderCamera);
		if (this.clipped)
		{
			this.UpdateUVs();
		}
		if (this.hideAtStart)
		{
			this.Hide(true);
		}
	}

	protected void CalcSizeUnitsPerUV()
	{
		Rect uvs = this.frameInfo.uvs;
		if (uvs.width == 0f || uvs.height == 0f)
		{
			this.sizeUnitsPerUV = Vector2.zero;
			return;
		}
		this.sizeUnitsPerUV.x = this.width / uvs.width;
		this.sizeUnitsPerUV.y = this.height / uvs.height;
	}

	protected virtual void Init()
	{
		this.screenPlacer = (EZScreenPlacement)base.GetComponent(typeof(EZScreenPlacement));
		if (!Application.isPlaying && this.screenPlacer != null)
		{
			this.screenPlacer.SetCamera(this.renderCamera);
		}
		if (this.m_spriteMesh != null)
		{
			if (this.persistent && !this.managed)
			{
				global::UnityEngine.Object.DontDestroyOnLoad(((SpriteMesh)this.m_spriteMesh).mesh);
			}
			if (this.m_spriteMesh.texture != null)
			{
				this.SetPixelToUV(this.m_spriteMesh.texture);
			}
			this.m_spriteMesh.Init();
		}
		if (!Application.isPlaying)
		{
			this.CalcSizeUnitsPerUV();
		}
	}

	public virtual void Clear()
	{
		this.billboarded = false;
		this.SetColor(Color.white);
		this.offset = Vector3.zero;
	}

	public virtual void Copy(SpriteRoot s)
	{
		if (!this.managed)
		{
			if (this.m_spriteMesh != null && s.spriteMesh != null)
			{
				((SpriteMesh)this.m_spriteMesh).material = s.spriteMesh.material;
			}
			else if (!s.managed)
			{
				base.GetComponent<Renderer>().sharedMaterial = s.GetComponent<Renderer>().sharedMaterial;
			}
		}
		this.drawLayer = s.drawLayer;
		if (s.renderCamera != null)
		{
			this.SetCamera(s.renderCamera);
		}
		if (this.renderCamera == null)
		{
			this.renderCamera = Camera.main;
		}
		if (this.m_spriteMesh != null)
		{
			if (this.m_spriteMesh.texture != null)
			{
				this.SetPixelToUV(this.m_spriteMesh.texture);
			}
			else if (!this.managed)
			{
				((SpriteMesh)this.m_spriteMesh).material = base.GetComponent<Renderer>().sharedMaterial;
				this.SetPixelToUV(this.m_spriteMesh.texture);
			}
		}
		this.plane = s.plane;
		this.winding = s.winding;
		this.offset = s.offset;
		this.anchor = s.anchor;
		this.bleedCompensation = s.bleedCompensation;
		this.autoResize = s.autoResize;
		this.pixelPerfect = s.pixelPerfect;
		this.ignoreClipping = s.ignoreClipping;
		this.uvRect = s.uvRect;
		this.scaleFactor = s.scaleFactor;
		this.topLeftOffset = s.topLeftOffset;
		this.bottomRightOffset = s.bottomRightOffset;
		this.width = s.width;
		this.height = s.height;
		this.SetColor(s.color);
	}

	public virtual void InitUVs()
	{
		this.uvRect = this.frameInfo.uvs;
	}

	public virtual void Delete()
	{
		this.deleted = true;
		if (!this.managed && Application.isPlaying)
		{
			global::UnityEngine.Object.Destroy(((SpriteMesh)this.spriteMesh).mesh);
			((SpriteMesh)this.spriteMesh).mesh = null;
		}
	}

	protected virtual void OnEnable()
	{
		if (this.managed && this.manager != null && this.m_started)
		{
			SPRITE_FRAME sprite_FRAME = this.frameInfo;
			this.manager.AddSprite(this);
			this.frameInfo = sprite_FRAME;
			this.uvRect = this.frameInfo.uvs;
			this.SetBleedCompensation();
		}
		else if (this.savedManager != null)
		{
			this.savedManager.AddSprite(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (this.managed && this.manager != null)
		{
			this.savedManager = this.manager;
			this.manager.RemoveSprite(this);
		}
	}

	public virtual void OnDestroy()
	{
		this.Delete();
	}

	public void CalcEdges()
	{
		switch (this.anchor)
		{
		case SpriteRoot.ANCHOR_METHOD.UPPER_LEFT:
			this.topLeft.x = 0f;
			this.topLeft.y = 0f;
			this.bottomRight.x = this.width;
			this.bottomRight.y = -this.height;
			break;
		case SpriteRoot.ANCHOR_METHOD.UPPER_CENTER:
			this.topLeft.x = this.width * -0.5f;
			this.topLeft.y = 0f;
			this.bottomRight.x = this.width * 0.5f;
			this.bottomRight.y = -this.height;
			break;
		case SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT:
			this.topLeft.x = -this.width;
			this.topLeft.y = 0f;
			this.bottomRight.x = 0f;
			this.bottomRight.y = -this.height;
			break;
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT:
			this.topLeft.x = 0f;
			this.topLeft.y = this.height * 0.5f;
			this.bottomRight.x = this.width;
			this.bottomRight.y = this.height * -0.5f;
			break;
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER:
			this.topLeft.x = this.width * -0.5f;
			this.topLeft.y = this.height * 0.5f;
			this.bottomRight.x = this.width * 0.5f;
			this.bottomRight.y = this.height * -0.5f;
			break;
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT:
			this.topLeft.x = -this.width;
			this.topLeft.y = this.height * 0.5f;
			this.bottomRight.x = 0f;
			this.bottomRight.y = this.height * -0.5f;
			break;
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT:
			this.topLeft.x = 0f;
			this.topLeft.y = this.height;
			this.bottomRight.x = this.width;
			this.bottomRight.y = 0f;
			break;
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER:
			this.topLeft.x = this.width * -0.5f;
			this.topLeft.y = this.height;
			this.bottomRight.x = this.width * 0.5f;
			this.bottomRight.y = 0f;
			break;
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT:
			this.topLeft.x = -this.width;
			this.topLeft.y = this.height;
			this.bottomRight.x = 0f;
			this.bottomRight.y = 0f;
			break;
		case SpriteRoot.ANCHOR_METHOD.TEXTURE_OFFSET:
		{
			Vector2 vector;
			vector.x = this.width * this.scaleFactor.x;
			vector.y = this.height * this.scaleFactor.y;
			this.topLeft.x = vector.x * this.topLeftOffset.x;
			this.topLeft.y = vector.y * this.topLeftOffset.y;
			this.bottomRight.x = vector.x * this.bottomRightOffset.x;
			this.bottomRight.y = vector.y * this.bottomRightOffset.y;
			break;
		}
		}
		this.unclippedTopLeft = this.topLeft + this.offset;
		this.unclippedBottomRight = this.bottomRight + this.offset;
		if (this.truncated)
		{
			this.topLeft.x = this.bottomRight.x - (this.bottomRight.x - this.topLeft.x) * this.tlTruncate.x;
			this.topLeft.y = this.bottomRight.y - (this.bottomRight.y - this.topLeft.y) * this.tlTruncate.y;
			this.bottomRight.x = this.topLeft.x - (this.topLeft.x - this.bottomRight.x) * this.brTruncate.x;
			this.bottomRight.y = this.topLeft.y - (this.topLeft.y - this.bottomRight.y) * this.brTruncate.y;
		}
		if (this.clipped && this.bottomRight.x - this.topLeft.x != 0f && this.topLeft.y - this.bottomRight.y != 0f)
		{
			Vector3 vector2 = this.topLeft;
			Vector3 vector3 = this.bottomRight;
			Rect rect = this.localClipRect;
			rect.x -= this.offset.x;
			rect.y -= this.offset.y;
			if (this.topLeft.x < rect.x)
			{
				this.leftClipPct = 1f - (rect.x - vector2.x) / (vector3.x - vector2.x);
				this.topLeft.x = Mathf.Clamp(rect.x, vector2.x, vector3.x);
				if (this.leftClipPct <= 0f)
				{
					this.topLeft.x = (this.bottomRight.x = rect.x);
				}
			}
			else
			{
				this.leftClipPct = 1f;
			}
			if (this.bottomRight.x > rect.xMax)
			{
				this.rightClipPct = (rect.xMax - vector2.x) / (vector3.x - vector2.x);
				this.bottomRight.x = Mathf.Clamp(rect.xMax, vector2.x, vector3.x);
				if (this.rightClipPct <= 0f)
				{
					this.bottomRight.x = (this.topLeft.x = rect.xMax);
				}
			}
			else
			{
				this.rightClipPct = 1f;
			}
			if (this.topLeft.y > rect.yMax)
			{
				this.topClipPct = (rect.yMax - vector3.y) / (vector2.y - vector3.y);
				this.topLeft.y = Mathf.Clamp(rect.yMax, vector3.y, vector2.y);
				if (this.topClipPct <= 0f)
				{
					this.topLeft.y = (this.bottomRight.y = rect.yMax);
				}
			}
			else
			{
				this.topClipPct = 1f;
			}
			if (this.bottomRight.y < rect.y)
			{
				this.bottomClipPct = 1f - (rect.y - vector3.y) / (vector2.y - vector3.y);
				this.bottomRight.y = Mathf.Clamp(rect.y, vector3.y, vector2.y);
				if (this.bottomClipPct <= 0f)
				{
					this.bottomRight.y = (this.topLeft.y = rect.y);
				}
			}
			else
			{
				this.bottomClipPct = 1f;
			}
		}
		if (this.winding == SpriteRoot.WINDING_ORDER.CCW)
		{
			this.topLeft.x = this.topLeft.x * -1f;
			this.bottomRight.x = this.bottomRight.x * -1f;
		}
	}

	public void CalcSize()
	{
		if (this.uvRect.width == 0f)
		{
			this.uvRect.width = 1E-07f;
		}
		if (this.uvRect.height == 0f)
		{
			this.uvRect.height = 1E-07f;
		}
		if (this.pixelPerfect)
		{
			this.width = this.worldUnitsPerScreenPixel * this.frameInfo.uvs.width * this.pixelsPerUV.x;
			this.height = this.worldUnitsPerScreenPixel * this.frameInfo.uvs.height * this.pixelsPerUV.y;
		}
		else if (this.autoResize && this.sizeUnitsPerUV.x != 0f && this.sizeUnitsPerUV.y != 0f)
		{
			this.width = this.frameInfo.uvs.width * this.sizeUnitsPerUV.x;
			this.height = this.frameInfo.uvs.height * this.sizeUnitsPerUV.y;
		}
		this.SetSize(this.width, this.height);
	}

	public virtual void SetSize(float w, float h)
	{
		if (this.m_spriteMesh == null)
		{
			return;
		}
		this.width = w;
		this.height = h;
		this.CalcSizeUnitsPerUV();
		switch (this.plane)
		{
		case SpriteRoot.SPRITE_PLANE.XY:
			this.SetSizeXY(this.width, this.height);
			break;
		case SpriteRoot.SPRITE_PLANE.XZ:
			this.SetSizeXZ(this.width, this.height);
			break;
		case SpriteRoot.SPRITE_PLANE.YZ:
			this.SetSizeYZ(this.width, this.height);
			break;
		}
		if (this.resizedDelegate != null)
		{
			this.resizedDelegate(this.width, this.height, this);
		}
	}

	protected void SetSizeXY(float w, float h)
	{
		this.CalcEdges();
		Vector3[] vertices = this.m_spriteMesh.vertices;
		if (this.winding == SpriteRoot.WINDING_ORDER.CW)
		{
			vertices[0].x = this.offset.x + this.topLeft.x;
			vertices[0].y = this.offset.y + this.topLeft.y;
			vertices[0].z = this.offset.z;
			vertices[1].x = this.offset.x + this.topLeft.x;
			vertices[1].y = this.offset.y + this.bottomRight.y;
			vertices[1].z = this.offset.z;
			vertices[2].x = this.offset.x + this.bottomRight.x;
			vertices[2].y = this.offset.y + this.bottomRight.y;
			vertices[2].z = this.offset.z;
			vertices[3].x = this.offset.x + this.bottomRight.x;
			vertices[3].y = this.offset.y + this.topLeft.y;
			vertices[3].z = this.offset.z;
		}
		else
		{
			vertices[0].x = this.offset.x + this.topLeft.x;
			vertices[0].y = this.offset.y + this.topLeft.y;
			vertices[0].z = this.offset.z;
			vertices[1].x = this.offset.x + this.topLeft.x;
			vertices[1].y = this.offset.y + this.bottomRight.y;
			vertices[1].z = this.offset.z;
			vertices[2].x = this.offset.x + this.bottomRight.x;
			vertices[2].y = this.offset.y + this.bottomRight.y;
			vertices[2].z = this.offset.z;
			vertices[3].x = this.offset.x + this.bottomRight.x;
			vertices[3].y = this.offset.y + this.topLeft.y;
			vertices[3].z = this.offset.z;
		}
		this.m_spriteMesh.UpdateVerts();
	}

	protected void SetSizeXZ(float w, float h)
	{
		this.CalcEdges();
		Vector3[] vertices = this.m_spriteMesh.vertices;
		vertices[0].x = this.offset.x + this.topLeft.x;
		vertices[0].y = this.offset.y;
		vertices[0].z = this.offset.z + this.topLeft.y;
		vertices[1].x = this.offset.x + this.topLeft.x;
		vertices[1].y = this.offset.y;
		vertices[1].z = this.offset.z + this.bottomRight.y;
		vertices[2].x = this.offset.x + this.bottomRight.x;
		vertices[2].y = this.offset.y;
		vertices[2].z = this.offset.z + this.bottomRight.y;
		vertices[3].x = this.offset.x + this.bottomRight.x;
		vertices[3].y = this.offset.y;
		vertices[3].z = this.offset.z + this.topLeft.y;
		this.m_spriteMesh.UpdateVerts();
	}

	protected void SetSizeYZ(float w, float h)
	{
		this.CalcEdges();
		Vector3[] vertices = this.m_spriteMesh.vertices;
		vertices[0].x = this.offset.x;
		vertices[0].y = this.offset.y + this.topLeft.y;
		vertices[0].z = this.offset.z + this.topLeft.x;
		vertices[1].x = this.offset.x;
		vertices[1].y = this.offset.y + this.bottomRight.y;
		vertices[1].z = this.offset.z + this.topLeft.x;
		vertices[2].x = this.offset.x;
		vertices[2].y = this.offset.y + this.bottomRight.y;
		vertices[2].z = this.offset.z + this.bottomRight.x;
		vertices[3].x = this.offset.x;
		vertices[3].y = this.offset.y + this.topLeft.y;
		vertices[3].z = this.offset.z + this.bottomRight.x;
		this.m_spriteMesh.UpdateVerts();
	}

	public virtual void TruncateRight(float pct)
	{
		this.tlTruncate.x = 1f;
		this.brTruncate.x = Mathf.Clamp01(pct);
		if (this.brTruncate.x < 1f || this.tlTruncate.y < 1f || this.brTruncate.y < 1f)
		{
			this.truncated = true;
			this.UpdateUVs();
			this.CalcSize();
			return;
		}
		this.Untruncate();
	}

	public virtual void TruncateLeft(float pct)
	{
		this.tlTruncate.x = Mathf.Clamp01(pct);
		this.brTruncate.x = 1f;
		if (this.tlTruncate.x < 1f || this.tlTruncate.y < 1f || this.brTruncate.y < 1f)
		{
			this.truncated = true;
			this.UpdateUVs();
			this.CalcSize();
			return;
		}
		this.Untruncate();
	}

	public virtual void TruncateTop(float pct)
	{
		this.tlTruncate.y = Mathf.Clamp01(pct);
		this.brTruncate.y = 1f;
		if (this.tlTruncate.y < 1f || this.tlTruncate.x < 1f || this.brTruncate.x < 1f)
		{
			this.truncated = true;
			this.UpdateUVs();
			this.CalcSize();
			return;
		}
		this.Untruncate();
	}

	public virtual void TruncateBottom(float pct)
	{
		this.tlTruncate.y = 1f;
		this.brTruncate.y = Mathf.Clamp01(pct);
		if (this.brTruncate.y < 1f || this.tlTruncate.x < 1f || this.brTruncate.x < 1f)
		{
			this.truncated = true;
			this.UpdateUVs();
			this.CalcSize();
			return;
		}
		this.Untruncate();
	}

	public virtual void Untruncate()
	{
		this.tlTruncate.x = 1f;
		this.tlTruncate.y = 1f;
		this.brTruncate.x = 1f;
		this.brTruncate.y = 1f;
		this.truncated = false;
		this.uvRect = this.frameInfo.uvs;
		this.SetBleedCompensation();
		this.CalcSize();
	}

	public virtual void Unclip()
	{
		if (this.ignoreClipping)
		{
			return;
		}
		this.leftClipPct = 1f;
		this.rightClipPct = 1f;
		this.topClipPct = 1f;
		this.bottomClipPct = 1f;
		this.clipped = false;
		this.uvRect = this.frameInfo.uvs;
		this.SetBleedCompensation();
		this.CalcSize();
	}

	public virtual void UpdateUVs()
	{
		this.scaleFactor = this.frameInfo.scaleFactor;
		this.topLeftOffset = this.frameInfo.topLeftOffset;
		this.bottomRightOffset = this.frameInfo.bottomRightOffset;
		if (this.truncated)
		{
			this.uvRect.x = this.frameInfo.uvs.xMax + this.bleedCompensationUV.x - this.frameInfo.uvs.width * this.tlTruncate.x * this.leftClipPct;
			this.uvRect.y = this.frameInfo.uvs.yMax + this.bleedCompensationUV.y - this.frameInfo.uvs.height * this.brTruncate.y * this.bottomClipPct;
			this.uvRect.xMax = this.frameInfo.uvs.x + this.bleedCompensationUVMax.x + this.frameInfo.uvs.width * this.brTruncate.x * this.rightClipPct;
			this.uvRect.yMax = this.frameInfo.uvs.y + this.bleedCompensationUVMax.y + this.frameInfo.uvs.height * this.tlTruncate.y * this.topClipPct;
		}
		else if (this.clipped)
		{
			Rect rect = Rect.MinMaxRect(this.frameInfo.uvs.x + this.bleedCompensationUV.x, this.frameInfo.uvs.y + this.bleedCompensationUV.y, this.frameInfo.uvs.xMax + this.bleedCompensationUVMax.x, this.frameInfo.uvs.yMax + this.bleedCompensationUVMax.y);
			this.uvRect.x = Mathf.Lerp(rect.xMax, rect.x, this.leftClipPct);
			this.uvRect.y = Mathf.Lerp(rect.yMax, rect.y, this.bottomClipPct);
			this.uvRect.xMax = Mathf.Lerp(rect.x, rect.xMax, this.rightClipPct);
			this.uvRect.yMax = Mathf.Lerp(rect.y, rect.yMax, this.topClipPct);
		}
		if (this.m_spriteMesh == null)
		{
			return;
		}
		Vector2[] uvs = this.m_spriteMesh.uvs;
		uvs[0].x = this.uvRect.x;
		uvs[0].y = this.uvRect.yMax;
		uvs[1].x = this.uvRect.x;
		uvs[1].y = this.uvRect.y;
		uvs[2].x = this.uvRect.xMax;
		uvs[2].y = this.uvRect.y;
		uvs[3].x = this.uvRect.xMax;
		uvs[3].y = this.uvRect.yMax;
		this.m_spriteMesh.UpdateUVs();
	}

	public void TransformBillboarded(Transform t)
	{
	}

	public virtual void SetColor(Color c)
	{
		this.color = c;
		if (this.m_spriteMesh != null)
		{
			this.m_spriteMesh.UpdateColors(this.color);
		}
	}

	public Color Color
	{
		get
		{
			return this.color;
		}
		set
		{
			this.SetColor(value);
		}
	}

	public void SetPixelToUV(int texWidth, int texHeight)
	{
		Vector2 vector = this.pixelsPerUV;
		this.pixelsPerUV.x = (float)texWidth;
		this.pixelsPerUV.y = (float)texHeight;
		Rect uvs = this.frameInfo.uvs;
		if (uvs.width == 0f || uvs.height == 0f || vector.x == 0f || vector.y == 0f)
		{
			return;
		}
		Vector2 vector2 = new Vector2(this.width / (uvs.width * vector.x), this.height / (uvs.height * vector.y));
		this.sizeUnitsPerUV.x = vector2.x * this.pixelsPerUV.x;
		this.sizeUnitsPerUV.y = vector2.y * this.pixelsPerUV.y;
	}

	public void SetPixelToUV(Texture tex)
	{
		this.SetPixelToUV(tex.width, tex.height);
	}

	public void CalcPixelToUV()
	{
		if (this.managed)
		{
			if (this.spriteMesh != null && this.spriteMesh.material != null && this.spriteMesh.material.mainTexture != null)
			{
				this.SetPixelToUV(this.spriteMesh.material.mainTexture);
			}
		}
		else if (base.GetComponent<Renderer>() != null && base.GetComponent<Renderer>().sharedMaterial != null && base.GetComponent<Renderer>().sharedMaterial.mainTexture != null)
		{
			this.SetPixelToUV(base.GetComponent<Renderer>().sharedMaterial.mainTexture);
		}
	}

	public void SetTexture(Texture2D tex)
	{
		if (this.managed || base.GetComponent<Renderer>() == null)
		{
			return;
		}
		base.GetComponent<Renderer>().material.mainTexture = tex;
		this.SetPixelToUV(tex);
		this.SetCamera();
	}

	public void SetMaterial(Material mat)
	{
		if (this.managed || base.GetComponent<Renderer>() == null)
		{
			return;
		}
		base.GetComponent<Renderer>().sharedMaterial = mat;
		this.SetPixelToUV(mat.mainTexture);
		this.SetCamera();
	}

	public virtual Camera RenderCamera
	{
		get
		{
			return this.renderCamera;
		}
		set
		{
			this.renderCamera = value;
			this.SetCamera(value);
		}
	}

	public void UpdateCamera()
	{
		this.SetCamera(this.renderCamera);
	}

	public void SetCamera()
	{
		this.SetCamera(this.renderCamera);
	}

	public virtual void SetCamera(Camera c)
	{
		if (c == null || !this.m_started)
		{
			return;
		}
		Plane plane = new Plane(c.transform.forward, c.transform.position);
		float num;
		if (Application.isPlaying)
		{
			this.renderCamera = c;
			this.screenSize.x = c.pixelWidth;
			this.screenSize.y = c.pixelHeight;
			if (this.screenPlacer != null)
			{
				this.screenPlacer.SetCamera(this.renderCamera);
			}
			num = plane.GetDistanceToPoint(base.transform.position);
			this.worldUnitsPerScreenPixel = Vector3.Distance(c.ScreenToWorldPoint(new Vector3(0f, 1f, num)), c.ScreenToWorldPoint(new Vector3(0f, 0f, num)));
			this.CalcSize();
			return;
		}
		this.screenSize.x = c.pixelWidth;
		this.screenSize.y = c.pixelHeight;
		if (this.screenSize.x == 0f)
		{
			return;
		}
		this.renderCamera = c;
		if (this.screenPlacer != null)
		{
			this.screenPlacer.SetCamera(this.renderCamera);
		}
		num = plane.GetDistanceToPoint(base.transform.position);
		this.worldUnitsPerScreenPixel = Vector3.Distance(c.ScreenToWorldPoint(new Vector3(0f, 1f, num)), c.ScreenToWorldPoint(new Vector3(0f, 0f, num)));
		if (!this.hideAtStart)
		{
			this.CalcSize();
		}
	}

	public virtual void Hide(bool tf)
	{
		if (this.m_spriteMesh != null)
		{
			this.m_spriteMesh.Hide(tf);
		}
		this.m_hidden = tf;
	}

	public bool IsHidden()
	{
		return this.m_hidden;
	}

	public Vector2 PixelSize
	{
		get
		{
			return new Vector2(this.width * this.worldUnitsPerScreenPixel, this.height * this.worldUnitsPerScreenPixel);
		}
		set
		{
			this.SetSize(value.x * this.worldUnitsPerScreenPixel, value.y * this.worldUnitsPerScreenPixel);
		}
	}

	public Vector2 ImageSize
	{
		get
		{
			return new Vector2(this.uvRect.width * this.pixelsPerUV.x, this.uvRect.height * this.pixelsPerUV.y);
		}
	}

	public bool Managed
	{
		get
		{
			return this.managed;
		}
		set
		{
			if (value)
			{
				if (!this.managed)
				{
					this.DestroyMesh();
				}
				this.managed = value;
			}
			else
			{
				if (this.managed)
				{
					if (this.manager != null)
					{
						this.manager.RemoveSprite(this);
					}
					this.manager = null;
				}
				this.managed = value;
				if (this.m_spriteMesh == null)
				{
					this.AddMesh();
				}
				else if (!(this.m_spriteMesh is SpriteMesh))
				{
					this.AddMesh();
				}
			}
		}
	}

	public bool Started
	{
		get
		{
			return this.m_started;
		}
	}

	protected void DestroyMesh()
	{
		if (this.m_spriteMesh != null)
		{
			this.m_spriteMesh.sprite = null;
		}
		this.m_spriteMesh = null;
		if (base.GetComponent<Renderer>() != null)
		{
			global::UnityEngine.Object.DestroyImmediate(base.GetComponent<Renderer>());
		}
		global::UnityEngine.Object component = base.gameObject.GetComponent(typeof(MeshFilter));
		if (component != null)
		{
			global::UnityEngine.Object.DestroyImmediate(component);
		}
	}

	protected void AddMesh()
	{
		this.m_spriteMesh = new SpriteMesh();
		this.m_spriteMesh.sprite = this;
	}

	public void SetBleedCompensation()
	{
		this.SetBleedCompensation(this.bleedCompensation);
	}

	public void SetBleedCompensation(float x, float y)
	{
		this.SetBleedCompensation(new Vector2(x, y));
	}

	public void SetBleedCompensation(Vector2 xy)
	{
		this.bleedCompensation = xy;
		this.bleedCompensationUV = this.PixelSpaceToUVSpace(this.bleedCompensation);
		this.bleedCompensationUVMax = this.bleedCompensationUV * -2f;
		this.uvRect.x = this.uvRect.x + this.bleedCompensationUV.x;
		this.uvRect.y = this.uvRect.y + this.bleedCompensationUV.y;
		this.uvRect.xMax = this.uvRect.xMax + this.bleedCompensationUVMax.x;
		this.uvRect.yMax = this.uvRect.yMax + this.bleedCompensationUVMax.y;
		this.UpdateUVs();
	}

	public void SetPlane(SpriteRoot.SPRITE_PLANE p)
	{
		this.plane = p;
		this.SetSize(this.width, this.height);
	}

	public void SetWindingOrder(SpriteRoot.WINDING_ORDER order)
	{
		this.winding = order;
		if (!this.managed && this.m_spriteMesh != null)
		{
			((SpriteMesh)this.m_spriteMesh).SetWindingOrder(order);
		}
	}

	public void SetDrawLayer(int layer)
	{
		if (!this.managed)
		{
			return;
		}
		this.drawLayer = layer;
		((SpriteMesh_Managed)this.m_spriteMesh).drawLayer = layer;
		if (this.manager != null)
		{
			this.manager.SortDrawingOrder();
		}
	}

	public void SetFrameInfo(SPRITE_FRAME fInfo)
	{
		this.frameInfo = fInfo;
		this.uvRect = fInfo.uvs;
		this.SetBleedCompensation();
		if (this.autoResize || this.pixelPerfect)
		{
			this.CalcSize();
		}
	}

	public void SetUVs(Rect uv)
	{
		this.frameInfo.uvs = uv;
		this.uvRect = uv;
		this.SetBleedCompensation();
		if (!Application.isPlaying)
		{
			this.CalcSizeUnitsPerUV();
		}
		if (this.autoResize || this.pixelPerfect)
		{
			this.CalcSize();
		}
	}

	public void SetUVsFromPixelCoords(Rect pxCoords)
	{
		this.tempUV = this.PixelCoordToUVCoord((int)pxCoords.x, (int)pxCoords.yMax);
		this.uvRect.x = this.tempUV.x;
		this.uvRect.y = this.tempUV.y;
		this.tempUV = this.PixelCoordToUVCoord((int)pxCoords.xMax, (int)pxCoords.y);
		this.uvRect.xMax = this.tempUV.x;
		this.uvRect.yMax = this.tempUV.y;
		this.frameInfo.uvs = this.uvRect;
		this.SetBleedCompensation();
		if (this.autoResize || this.pixelPerfect)
		{
			this.CalcSize();
		}
	}

	public Rect GetUVs()
	{
		return this.uvRect;
	}

	public Vector3[] GetVertices()
	{
		if (!this.managed)
		{
			return ((SpriteMesh)this.m_spriteMesh).mesh.vertices;
		}
		return this.m_spriteMesh.vertices;
	}

	public Vector3 GetCenterPoint()
	{
		if (this.m_spriteMesh == null)
		{
			return Vector3.zero;
		}
		Vector3[] vertices = this.m_spriteMesh.vertices;
		switch (this.plane)
		{
		case SpriteRoot.SPRITE_PLANE.XY:
			return new Vector3(vertices[0].x + 0.5f * (vertices[2].x - vertices[0].x), vertices[0].y - 0.5f * (vertices[0].y - vertices[2].y), this.offset.z);
		case SpriteRoot.SPRITE_PLANE.XZ:
			return new Vector3(vertices[0].x + 0.5f * (vertices[2].x - vertices[0].x), this.offset.y, vertices[0].z - 0.5f * (vertices[0].z - vertices[2].z));
		case SpriteRoot.SPRITE_PLANE.YZ:
			return new Vector3(this.offset.x, vertices[0].y - 0.5f * (vertices[0].y - vertices[2].y), vertices[0].z - 0.5f * (vertices[0].z - vertices[2].z));
		default:
			return new Vector3(vertices[0].x + 0.5f * (vertices[2].x - vertices[0].x), vertices[0].y - 0.5f * (vertices[0].y - vertices[2].y), this.offset.z);
		}
	}

	public virtual Rect3D ClippingRect
	{
		get
		{
			return this.clippingRect;
		}
		set
		{
			if (this.ignoreClipping)
			{
				return;
			}
			this.clippingRect = value;
			this.localClipRect = Rect3D.MultFast(this.clippingRect, base.transform.worldToLocalMatrix).GetRect();
			this.clipped = true;
			this.CalcSize();
			this.UpdateUVs();
		}
	}

	public virtual bool Clipped
	{
		get
		{
			return this.clipped;
		}
		set
		{
			if (this.ignoreClipping)
			{
				return;
			}
			if (value && !this.clipped)
			{
				this.clipped = true;
				this.CalcSize();
			}
			else if (this.clipped)
			{
				this.Unclip();
			}
		}
	}

	public void SetAnchor(SpriteRoot.ANCHOR_METHOD a)
	{
		this.anchor = a;
		this.SetSize(this.width, this.height);
	}

	public SpriteRoot.ANCHOR_METHOD Anchor
	{
		get
		{
			return this.anchor;
		}
		set
		{
			this.SetAnchor(value);
		}
	}

	public void SetOffset(Vector3 o)
	{
		this.offset = o;
		this.SetSize(this.width, this.height);
	}

	public Vector3 UnclippedTopLeft
	{
		get
		{
			if (!this.m_started)
			{
				this.Start();
			}
			return this.unclippedTopLeft;
		}
	}

	public Vector3 UnclippedBottomRight
	{
		get
		{
			if (!this.m_started)
			{
				this.Start();
			}
			return this.unclippedBottomRight;
		}
	}

	public Vector3 TopLeft
	{
		get
		{
			if (this.m_spriteMesh != null)
			{
				return this.m_spriteMesh.vertices[0];
			}
			return Vector3.zero;
		}
	}

	public Vector3 BottomRight
	{
		get
		{
			if (this.m_spriteMesh != null)
			{
				return this.m_spriteMesh.vertices[2];
			}
			return Vector3.zero;
		}
	}

	public ISpriteMesh spriteMesh
	{
		get
		{
			return this.m_spriteMesh;
		}
		set
		{
			this.m_spriteMesh = value;
			if (this.m_spriteMesh != null)
			{
				if (this.m_spriteMesh.sprite != this)
				{
					this.m_spriteMesh.sprite = this;
				}
				if (this.managed)
				{
					this.manager = ((SpriteMesh_Managed)this.m_spriteMesh).manager;
				}
				return;
			}
		}
	}

	public bool AddedToManager
	{
		get
		{
			return this.addedToManager;
		}
		set
		{
			this.addedToManager = value;
		}
	}

	public abstract Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader);

	public Vector2 PixelSpaceToUVSpace(Vector2 xy)
	{
		if (this.pixelsPerUV.x == 0f || this.pixelsPerUV.y == 0f)
		{
			return Vector2.zero;
		}
		return new Vector2(xy.x / this.pixelsPerUV.x, xy.y / this.pixelsPerUV.y);
	}

	public Vector2 PixelSpaceToUVSpace(int x, int y)
	{
		return this.PixelSpaceToUVSpace(new Vector2((float)x, (float)y));
	}

	public Vector2 PixelCoordToUVCoord(Vector2 xy)
	{
		if (this.pixelsPerUV.x == 0f || this.pixelsPerUV.y == 0f)
		{
			return Vector2.zero;
		}
		return new Vector2(xy.x / (this.pixelsPerUV.x - 1f), 1f - xy.y / (this.pixelsPerUV.y - 1f));
	}

	public Vector2 PixelCoordToUVCoord(int x, int y)
	{
		return this.PixelCoordToUVCoord(new Vector2((float)x, (float)y));
	}

	public abstract int GetStateIndex(string stateName);

	public abstract void SetState(int index);

	public ISpriteAnimatable prev
	{
		get
		{
			return this.m_prev;
		}
		set
		{
			this.m_prev = value;
		}
	}

	public ISpriteAnimatable next
	{
		get
		{
			return this.m_next;
		}
		set
		{
			this.m_next = value;
		}
	}

	public virtual void DoMirror()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (this.screenSize.x == 0f || this.screenSize.y == 0f)
		{
			this.Start();
		}
		if (this.mirror == null)
		{
			this.mirror = new SpriteRootMirror();
			this.mirror.Mirror(this);
		}
		this.mirror.Validate(this);
		if (this.mirror.DidChange(this))
		{
			this.Init();
			this.mirror.Mirror(this);
		}
	}

	public virtual void OnDrawGizmosSelected()
	{
		this.DoMirror();
	}

	public virtual void OnDrawGizmos()
	{
		this.DoMirror();
	}

	public bool managed;

	public SpriteManagerEZ manager;

	protected bool addedToManager;

	public int drawLayer;

	public bool persistent;

	public SpriteRoot.SPRITE_PLANE plane;

	public SpriteRoot.WINDING_ORDER winding = SpriteRoot.WINDING_ORDER.CW;

	public float width;

	public float height;

	public Vector2 bleedCompensation;

	public SpriteRoot.ANCHOR_METHOD anchor = SpriteRoot.ANCHOR_METHOD.TEXTURE_OFFSET;

	public bool pixelPerfect;

	public bool autoResize;

	protected Vector2 bleedCompensationUV;
	protected Vector2 bleedCompensationUVMax;

	protected SPRITE_FRAME frameInfo = new SPRITE_FRAME(0f);

	protected Rect uvRect;

	protected Vector2 scaleFactor = new Vector2(0.5f, 0.5f);
	protected Vector2 topLeftOffset = new Vector2(-1f, 1f);
	protected Vector2 bottomRightOffset = new Vector2(1f, -1f);
	protected Vector3 topLeft;
	protected Vector3 bottomRight;
	protected Vector3 unclippedTopLeft;
	protected Vector3 unclippedBottomRight;

	protected Vector2 tlTruncate = new Vector2(1f, 1f);
	protected Vector2 brTruncate = new Vector2(1f, 1f);

	protected bool truncated;
	protected Rect3D clippingRect;
	protected Rect localClipRect;
	protected float leftClipPct = 1f;
	protected float rightClipPct = 1f;
	protected float topClipPct = 1f;
	protected float bottomClipPct = 1f;
	protected bool clipped;

	public bool billboarded;
	public bool isClone;
	public bool uvsInitialized;

	protected bool m_started;
	protected bool deleted;

	public Vector3 offset = default(Vector3);

	public Color color = Color.white;

	protected ISpriteMesh m_spriteMesh;

	protected ISpriteAnimatable m_prev;

	protected ISpriteAnimatable m_next;

	protected Vector2 screenSize;

	public Camera renderCamera;

	protected Vector2 sizeUnitsPerUV;

	[HideInInspector]
	public Vector2 pixelsPerUV;

	protected float worldUnitsPerScreenPixel;

	protected SpriteRoot.SpriteResizedDelegate resizedDelegate;

	protected EZScreenPlacement screenPlacer;

	public bool hideAtStart;

	protected bool m_hidden;

	public bool ignoreClipping;

	protected SpriteRootMirror mirror;

	protected Vector2 tempUV;

	protected Mesh oldMesh;

	protected SpriteManagerEZ savedManager;

	public enum SPRITE_PLANE
	{
		XY,
		XZ,
		YZ
	}

	public enum ANCHOR_METHOD
	{
		UPPER_LEFT,
		UPPER_CENTER,
		UPPER_RIGHT,
		MIDDLE_LEFT,
		MIDDLE_CENTER,
		MIDDLE_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_CENTER,
		BOTTOM_RIGHT,
		TEXTURE_OFFSET
	}

	public enum WINDING_ORDER
	{
		CCW,
		CW
	}

	public delegate void SpriteResizedDelegate(float newWidth, float newHeight, SpriteRoot sprite);
}

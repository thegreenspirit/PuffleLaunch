using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("EZ GUI/Utility/EZ Screen Placement")]
[ExecuteInEditMode]
[Serializable]
public class BHUIScreenPlacement : MonoBehaviour, IUseCamera
{
	public virtual void Awake()
	{
		if (this.m_awake)
		{
			return;
		}
		this.m_awake = true;
		this.SetupScreenPlacement();
		IUseCamera useCamera = (IUseCamera)base.GetComponent("IUseCamera");
		if (useCamera != null)
		{
			this.renderCamera = useCamera.RenderCamera;
		}
		if (this.renderCamera == null)
		{
			this.renderCamera = Camera.main;
		}
		if (this.relativeTo == null)
		{
			this.relativeTo = new BHUIScreenPlacement.RelativeTo(this);
		}
		else if (this.relativeTo.Script != this)
		{
			BHUIScreenPlacement.RelativeTo relativeTo = new BHUIScreenPlacement.RelativeTo(this, this.relativeTo);
			this.relativeTo = relativeTo;
		}
	}

	protected virtual void SetupScreenPlacement()
	{
		if (this.relativeTo == null || SizeCategory.Instance == null)
		{
			return;
		}
		Vector3 vector = default(Vector3);
		switch (SizeCategory.Instance.CurCategoryId)
		{
		case SizeCategory.CategoryId.eSmall:
			vector = this.screenPosSmall;
			goto IL_0064;
		case SizeCategory.CategoryId.eLarge:
			vector = this.screenPosLarge;
			goto IL_0064;
		}
		vector = this.screenPosMedium;
		IL_0064:
		if (this.relativeTo.horizontal != BHUIScreenPlacement.HORIZONTAL_ALIGN.NONE)
		{
			this.screenPos.x = vector.x;
		}
		if (this.relativeTo.vertical != BHUIScreenPlacement.VERTICAL_ALIGN.NONE)
		{
			this.screenPos.y = vector.y;
		}
		this.screenPos.z = vector.z;
	}

	public void Start()
	{
		if (this.m_started)
		{
			return;
		}
		this.m_started = true;
		if (this.renderCamera != null)
		{
			this.screenSize.x = this.renderCamera.pixelWidth;
			this.screenSize.y = this.renderCamera.pixelHeight;
		}
		this.PositionOnScreenRecursively();
	}

	public void PositionOnScreenRecursively()
	{
		if (!this.m_started)
		{
			this.Start();
		}
		if (this.relativeObject != null)
		{
			BHUIScreenPlacement bhuiscreenPlacement = this.relativeObject.GetComponent(typeof(BHUIScreenPlacement)) as BHUIScreenPlacement;
			if (bhuiscreenPlacement != null)
			{
				bhuiscreenPlacement.PositionOnScreenRecursively();
			}
		}
		this.PositionOnScreen();
	}

	public Vector3 ScreenPosToLocalPos(Vector3 screenPos)
	{
		return base.transform.InverseTransformPoint(this.ScreenPosToWorldPos(screenPos));
	}

	public Vector3 ScreenPosToParentPos(Vector3 screenPos)
	{
		return this.ScreenPosToLocalPos(screenPos) + base.transform.localPosition;
	}

	public Vector3 ScreenPosToWorldPos(Vector3 screenPos)
	{
		if (!this.m_started)
		{
			this.Start();
		}
		if (this.renderCamera == null)
		{
			Debug.LogError("Render camera not yet assigned to BHUIScreenPlacement component of \"" + base.name + "\" when attempting to call PositionOnScreen()");
			return base.transform.position;
		}
		Vector3 vector = this.renderCamera.WorldToScreenPoint(base.transform.position);
		Vector3 vector2 = screenPos;
		switch (this.relativeTo.horizontal)
		{
		case BHUIScreenPlacement.HORIZONTAL_ALIGN.NONE:
			vector2.x = vector.x;
			break;
		case BHUIScreenPlacement.HORIZONTAL_ALIGN.SCREEN_RIGHT:
			vector2.x = this.screenSize.x + vector2.x;
			break;
		case BHUIScreenPlacement.HORIZONTAL_ALIGN.SCREEN_CENTER:
			vector2.x = this.screenSize.x * 0.5f + vector2.x;
			break;
		case BHUIScreenPlacement.HORIZONTAL_ALIGN.OBJECT:
			if (this.relativeObject != null)
			{
				vector2.x = this.renderCamera.WorldToScreenPoint(this.relativeObject.position).x + vector2.x;
			}
			else
			{
				vector2.x = vector.x;
			}
			break;
		}
		switch (this.relativeTo.vertical)
		{
		case BHUIScreenPlacement.VERTICAL_ALIGN.NONE:
			vector2.y = vector.y;
			break;
		case BHUIScreenPlacement.VERTICAL_ALIGN.SCREEN_TOP:
			vector2.y = this.screenSize.y + vector2.y;
			break;
		case BHUIScreenPlacement.VERTICAL_ALIGN.SCREEN_CENTER:
			vector2.y = this.screenSize.y * 0.5f + vector2.y;
			break;
		case BHUIScreenPlacement.VERTICAL_ALIGN.OBJECT:
			if (this.relativeObject != null)
			{
				vector2.y = this.renderCamera.WorldToScreenPoint(this.relativeObject.position).y + vector2.y;
			}
			else
			{
				vector2.y = vector.y;
			}
			break;
		}
		return this.renderCamera.ScreenToWorldPoint(vector2);
	}

	public void PositionOnScreen()
	{
		if (!this.m_awake)
		{
			return;
		}
		if (this.ignoreZ)
		{
			Plane plane = new Plane(this.renderCamera.transform.forward, this.renderCamera.transform.position);
			this.screenPos.z = plane.GetDistanceToPoint(base.transform.position);
		}
		if (this.ignoreZ)
		{
			Vector3 vector = this.ScreenPosToWorldPos(this.screenPos);
			vector.z = base.transform.position.z;
			base.transform.position = vector;
		}
		else
		{
			base.transform.position = this.ScreenPosToWorldPos(this.screenPos);
		}
		base.SendMessage("OnReposition", SendMessageOptions.DontRequireReceiver);
	}

	public void PositionOnScreen(int x, int y, float depth)
	{
		this.PositionOnScreen(new Vector3((float)x, (float)y, depth));
	}

	public void PositionOnScreen(Vector3 pos)
	{
		this.screenPos = pos;
		this.PositionOnScreen();
	}

	public Camera RenderCamera
	{
		get
		{
			return this.renderCamera;
		}
		set
		{
			this.SetCamera(value);
		}
	}

	public void SetCamera()
	{
		this.SetCamera(this.renderCamera);
	}

	public void SetCamera(Camera c)
	{
		if (c == null)
		{
			return;
		}
		this.renderCamera = c;
		this.screenSize.x = this.renderCamera.pixelWidth;
		this.screenSize.y = this.renderCamera.pixelHeight;
		if (this.alwaysRecursive || (Application.isEditor && !Application.isPlaying))
		{
			this.PositionOnScreenRecursively();
		}
		else
		{
			this.PositionOnScreen();
		}
	}

	public void WorldToScreenPos(Vector3 worldPos)
	{
		if (this.renderCamera == null)
		{
			return;
		}
		Vector3 vector = this.renderCamera.WorldToScreenPoint(worldPos);
		switch (this.relativeTo.horizontal)
		{
		case BHUIScreenPlacement.HORIZONTAL_ALIGN.SCREEN_LEFT:
			this.screenPos.x = vector.x;
			break;
		case BHUIScreenPlacement.HORIZONTAL_ALIGN.SCREEN_RIGHT:
			this.screenPos.x = vector.x - this.renderCamera.pixelWidth;
			break;
		case BHUIScreenPlacement.HORIZONTAL_ALIGN.SCREEN_CENTER:
			this.screenPos.x = vector.x - this.renderCamera.pixelWidth / 2f;
			break;
		case BHUIScreenPlacement.HORIZONTAL_ALIGN.OBJECT:
			if (this.relativeObject != null)
			{
				Vector3 vector2 = this.renderCamera.WorldToScreenPoint(this.relativeObject.transform.position);
				this.screenPos.x = vector.x - vector2.x;
			}
			break;
		}
		switch (this.relativeTo.vertical)
		{
		case BHUIScreenPlacement.VERTICAL_ALIGN.SCREEN_TOP:
			this.screenPos.y = vector.y - this.renderCamera.pixelHeight;
			break;
		case BHUIScreenPlacement.VERTICAL_ALIGN.SCREEN_BOTTOM:
			this.screenPos.y = vector.y;
			break;
		case BHUIScreenPlacement.VERTICAL_ALIGN.SCREEN_CENTER:
			this.screenPos.y = vector.y - this.renderCamera.pixelHeight / 2f;
			break;
		case BHUIScreenPlacement.VERTICAL_ALIGN.OBJECT:
			if (this.relativeObject != null)
			{
				Vector3 vector3 = this.renderCamera.WorldToScreenPoint(this.relativeObject.transform.position);
				this.screenPos.y = vector.y - vector3.y;
			}
			break;
		}
		this.screenPos.z = vector.z;
		if (this.alwaysRecursive)
		{
			this.PositionOnScreenRecursively();
		}
		else
		{
			this.PositionOnScreen();
		}
	}

	public Vector3 ScreenCoord
	{
		get
		{
			return this.renderCamera.WorldToScreenPoint(base.transform.position);
		}
	}

	public static bool TestDepenency(BHUIScreenPlacement sp)
	{
		if (sp.relativeObject == null)
		{
			return true;
		}
		List<BHUIScreenPlacement> list = new List<BHUIScreenPlacement>();
		list.Add(sp);
		BHUIScreenPlacement bhuiscreenPlacement = sp.relativeObject.GetComponent(typeof(BHUIScreenPlacement)) as BHUIScreenPlacement;
		while (bhuiscreenPlacement != null)
		{
			if (list.Contains(bhuiscreenPlacement))
			{
				return false;
			}
			list.Add(bhuiscreenPlacement);
			if (bhuiscreenPlacement.relativeObject == null)
			{
				return true;
			}
			bhuiscreenPlacement = bhuiscreenPlacement.relativeObject.GetComponent(typeof(BHUIScreenPlacement)) as BHUIScreenPlacement;
		}
		return true;
	}

	public virtual void DoMirror()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (this.mirror == null)
		{
			this.mirror = new BHUIScreenPlacementMirror();
			this.mirror.Mirror(this);
		}
		this.mirror.Validate(this);
		if (this.mirror.DidChange(this))
		{
			this.SetCamera(this.renderCamera);
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

	public Camera renderCamera;

	public Vector3 screenPos = Vector3.forward;

	public Vector3 screenPosSmall;

	public Vector3 screenPosMedium;

	public Vector3 screenPosLarge;

	public bool ignoreZ;

	public BHUIScreenPlacement.RelativeTo relativeTo;

	public Transform relativeObject;

	public bool alwaysRecursive = true;

	public bool allowTransformDrag;

	protected Vector2 screenSize;

	protected bool justEnabled = true;

	protected BHUIScreenPlacementMirror mirror = new BHUIScreenPlacementMirror();

	protected bool m_awake;

	protected bool m_started;

	public enum HORIZONTAL_ALIGN
	{
		NONE,
		SCREEN_LEFT,
		SCREEN_RIGHT,
		SCREEN_CENTER,
		OBJECT
	}

	public enum VERTICAL_ALIGN
	{
		NONE,
		SCREEN_TOP,
		SCREEN_BOTTOM,
		SCREEN_CENTER,
		OBJECT
	}

	[Serializable]
	public class RelativeTo
	{
		public BHUIScreenPlacement.HORIZONTAL_ALIGN horizontal = BHUIScreenPlacement.HORIZONTAL_ALIGN.SCREEN_LEFT;
		public BHUIScreenPlacement.VERTICAL_ALIGN vertical = BHUIScreenPlacement.VERTICAL_ALIGN.SCREEN_TOP;
		protected BHUIScreenPlacement script;

		public RelativeTo(BHUIScreenPlacement sp, BHUIScreenPlacement.RelativeTo rt)
		{
			this.script = sp;
			this.Copy(rt);
		}

		public RelativeTo(BHUIScreenPlacement sp)
		{
			this.script = sp;
		}

		public BHUIScreenPlacement Script
		{
			get { return this.script; }
			set { this.Script = value; }
		}

		public bool Equals(BHUIScreenPlacement.RelativeTo rt)
		{
			return rt != null && this.horizontal == rt.horizontal && this.vertical == rt.vertical;
		}

		public void Copy(BHUIScreenPlacement.RelativeTo rt)
		{
			if (rt == null)
			{
				return;
			}
			this.horizontal = rt.horizontal;
			this.vertical = rt.vertical;
		}
	}
}

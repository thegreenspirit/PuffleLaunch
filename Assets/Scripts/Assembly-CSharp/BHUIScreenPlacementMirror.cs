using System;
using UnityEngine;

public class BHUIScreenPlacementMirror
{
	public BHUIScreenPlacementMirror()
	{
		this.relativeTo = new BHUIScreenPlacement.RelativeTo(null);
	}

	public virtual void Mirror(BHUIScreenPlacement sp)
	{
		this.worldPos = sp.transform.position;
		this.screenPos = sp.screenPos;
		this.relativeTo.Copy(sp.relativeTo);
		this.relativeObject = sp.relativeObject;
		this.renderCamera = sp.renderCamera;
		this.screenSize = new Vector2(sp.renderCamera.pixelWidth, sp.renderCamera.pixelHeight);
	}

	public virtual bool Validate(BHUIScreenPlacement sp)
	{
		if (sp.relativeTo.horizontal != BHUIScreenPlacement.HORIZONTAL_ALIGN.OBJECT && sp.relativeTo.vertical != BHUIScreenPlacement.VERTICAL_ALIGN.OBJECT)
		{
			sp.relativeObject = null;
		}
		if (sp.relativeObject != null && !BHUIScreenPlacement.TestDepenency(sp))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"ERROR: The Relative Object you recently assigned on \"",
				sp.name,
				"\" which points to \"",
				sp.relativeObject.name,
				"\" would create a circular dependency.  Please check your placement dependencies to resolve this."
			}));
			sp.relativeObject = null;
		}
		return true;
	}

	public virtual bool DidChange(BHUIScreenPlacement sp)
	{
		if (this.worldPos != sp.transform.position)
		{
			if (sp.allowTransformDrag)
			{
				sp.WorldToScreenPos(sp.transform.position);
			}
			else
			{
				sp.PositionOnScreen();
			}
			return true;
		}
		return this.screenPos != sp.screenPos || (this.renderCamera != null && (this.screenSize.x != sp.renderCamera.pixelWidth || this.screenSize.y != sp.renderCamera.pixelHeight)) || !this.relativeTo.Equals(sp.relativeTo) || this.renderCamera != sp.renderCamera || this.relativeObject != sp.relativeObject;
	}

	public Vector3 worldPos;

	public Vector3 screenPos;

	public BHUIScreenPlacement.RelativeTo relativeTo;

	public Transform relativeObject;

	public Camera renderCamera;

	public Vector2 screenSize;
}

using System;
using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
	private void Start()
	{
		Bounds bounds = new Bounds(this.anchorObject.position, Vector3.zero);
		Bounds bounds2 = new Bounds(base.transform.position, Vector3.zero);
		if (this.anchorObject.GetComponent<Renderer>())
		{
			bounds = this.anchorObject.GetComponent<Renderer>().bounds;
		}
		if (base.GetComponent<Renderer>())
		{
			bounds2 = base.GetComponent<Renderer>().bounds;
		}
		Vector3 vector = new Vector3(0f, 0f, base.transform.localPosition.z);
		switch (this.anchorPoint)
		{
		case TextAnchor.UpperLeft:
			vector.x = bounds.min.x - bounds2.min.x;
			vector.y = bounds.max.y - bounds2.max.y;
			break;
		case TextAnchor.UpperCenter:
			vector.x = bounds.center.x - bounds2.center.x;
			vector.y = bounds.max.y - bounds2.max.y;
			break;
		case TextAnchor.UpperRight:
			vector.x = bounds.max.x - bounds2.max.x;
			vector.y = bounds.max.y - bounds2.max.y;
			break;
		case TextAnchor.MiddleLeft:
			vector.x = bounds.min.x - bounds2.min.x;
			vector.y = bounds.center.y - bounds2.center.y;
			break;
		case TextAnchor.MiddleCenter:
			vector.x = bounds.center.x - bounds2.center.x;
			vector.y = bounds.center.y - bounds2.center.y;
			break;
		case TextAnchor.MiddleRight:
			vector.x = bounds.max.x - bounds2.max.x;
			vector.y = bounds.center.y - bounds2.center.y;
			break;
		case TextAnchor.LowerLeft:
			vector.x = bounds.min.x - bounds2.min.x;
			vector.y = bounds.min.y - bounds2.min.y;
			break;
		case TextAnchor.LowerCenter:
			vector.x = bounds.center.x - bounds2.center.x;
			vector.y = bounds.min.y - bounds2.min.y;
			break;
		case TextAnchor.LowerRight:
			vector.x = bounds.max.x - bounds2.max.x;
			vector.y = bounds.min.y - bounds2.min.y;
			break;
		}
		base.transform.localPosition = vector;
	}

	public Transform anchorObject;

	public TextAnchor anchorPoint;
}

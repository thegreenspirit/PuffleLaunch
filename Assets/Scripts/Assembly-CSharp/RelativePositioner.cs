using System;
using UnityEngine;

public class RelativePositioner : MonoBehaviour
{
	public void Start()
	{
		if (this.children.Length != this.childrenOffsets.Length)
		{
			Debug.LogWarning("Children array size mismatch");
		}
		for (int i = 0; i < this.children.Length; i++)
		{
			float z = this.children[i].localPosition.z;
			Vector3 vector = (this.childrenOffsets[i] + this.rootOffset) * ScaleItem.Instance.LevelScale + Vector3.forward * z;
			this.children[i].localPosition = vector / base.transform.localScale.x;
		}
	}

	public Vector3 rootOffset;

	public Transform[] children;

	public Vector3[] childrenOffsets;
}

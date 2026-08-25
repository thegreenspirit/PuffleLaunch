using System;
using UnityEngine;

public class SlingshotBand : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		this.mInitialAngles = this.mTransform.localEulerAngles;
		this.mInitialScale = this.mTransform.localScale.x;
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			this.mInitialScale *= 1.25f;
		}
		else if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eLowres)
		{
		}
		this.mBaseLength = (this.secondEndpoint.position - this.firstEndpoint.position).magnitude;
	}

	public void Update()
	{
		Vector3 vector = this.secondEndpoint.position - this.firstEndpoint.position;
		vector.z = 0f;
		Vector3 localScale = this.mTransform.localScale;
		localScale.x = this.mInitialScale * vector.magnitude / this.mBaseLength;
		this.mTransform.localScale = localScale;
		float num = Vector3.Angle(Vector3.right, vector);
		if (Vector3.Cross(Vector3.right, vector).z < 0f)
		{
			num *= -1f;
		}
		this.mTransform.localEulerAngles = this.mInitialAngles;
		this.mTransform.RotateAround(Vector3.zero, Vector3.forward, num);
		this.mTransform.position = this.firstEndpoint.position + vector * 0.5f;
		this.mTransform.position += new Vector3(0f, 0f, 0.1f);
	}

	public Transform firstEndpoint;

	public Transform secondEndpoint;

	private Transform mTransform;

	private Vector3 mInitialAngles;

	private float mInitialScale;

	private float mBaseLength;
}

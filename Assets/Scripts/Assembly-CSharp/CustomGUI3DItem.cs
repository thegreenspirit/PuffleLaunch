using System;
using System.Collections;
using UnityEngine;

public class CustomGUI3DItem : MonoBehaviour
{
	private void Start()
	{
		this.InitPosition();
	}

	public virtual void InitPosition()
	{
		if (this.repositionToRelativeObject && this.relativeObject != null && this.relativeObject.GetComponent<Renderer>() != null)
		{
			base.StartCoroutine(this.WaitToRepositionToRelativeObject());
			return;
		}
		if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eIPad)
		{
			if (this.iPadTransform != null)
			{
				base.transform.localPosition = this.iPadTransform.localPosition;
			}
			else
			{
				Vector3 localPosition = base.gameObject.transform.localPosition;
				localPosition.x *= 0.8888889f;
				base.transform.localPosition = localPosition;
			}
		}
	}

	private IEnumerator WaitToRepositionToRelativeObject()
	{
		while (this.relativeObject.GetComponent<Renderer>().bounds.size.y == 0f)
		{
			yield return null;
		}
		this.RepositionToRelativeObject();
		yield break;
	}

	private void RepositionToRelativeObject()
	{
		Vector3 position = base.gameObject.transform.position;
		position.x = this.relativeObject.transform.position.x;
		position.y = this.relativeObject.transform.position.y - this.relativeObject.GetComponent<Renderer>().bounds.size.y / 2f - 0.5f;
		base.transform.position = position;
	}

	public Transform iPadTransform;

	public bool repositionToRelativeObject;

	public GameObject relativeObject;
}

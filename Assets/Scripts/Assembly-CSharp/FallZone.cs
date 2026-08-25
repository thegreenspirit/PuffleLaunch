using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FallZone : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		SpriteMeshGenerator spriteMeshGenerator = new SpriteMeshGenerator(base.GetComponent<MeshFilter>());
		spriteMeshGenerator.Generate(new Vector2(0f, -1024f), new Vector2((float)Screen.width, 1024f), false);
		Vector3 localScale = this.mTransform.localScale;
		localScale.y *= ScaleItem.Instance.BillboardScale;
		this.mTransform.localScale = localScale;
		this.mBaseScale = localScale;
		Vector3 position = this.mTransform.position;
		position.y += 40f * Mathf.Sign(this.mTransform.localScale.y) * ScaleItem.Instance.LevelScale;
		position.z = 0.01f;
		this.mTransform.position = position;
		Camera main = Camera.main;
		this.mMainCamera = main.transform;
		this.mBaseOrthographicSize = main.GetComponent<CameraFollow>().OriginalOrthographicSize;
		this.mHorizontalOffset = base.GetComponent<Renderer>().bounds.size.x / 2f;
	}

	public void LateUpdate()
	{
		float num = Camera.main.orthographicSize / this.mBaseOrthographicSize;
		Vector3 position = this.mTransform.position;
		position.x = this.mMainCamera.position.x - this.mHorizontalOffset * num;
		this.mTransform.position = position;
		this.mTransform.localScale = this.mBaseScale * num;
	}

	private Transform mTransform;

	private Transform mMainCamera;

	private float mHorizontalOffset;

	private float mBaseOrthographicSize;

	private Vector3 mBaseScale;
}

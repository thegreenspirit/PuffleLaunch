using System;
using UnityEngine;

public class ScaleItem
{
	public static ScaleItem Instance
	{
		get
		{
			if (ScaleItem.m_cInstance == null)
			{
				ScaleItem.m_cInstance = new ScaleItem();
				ScaleItem.m_cInstance.Initialize();
			}
			return ScaleItem.m_cInstance;
		}
	}

	public float BillboardScale
	{
		get
		{
			return (float)Screen.height / 480f * 0.72f;
		}
	}

	public float LevelScale
	{
		get
		{
			return this.mLevelScale;
		}
	}

	public float PlayerRadius
	{
		get
		{
			return this.mPlayerRadius;
		}
		set
		{
			this.mPlayerRadius = value;
		}
	}

	private void Initialize()
	{
		float num = (float)Screen.height / 480f;
		this.mLevelScale = Mathf.Abs((Camera.main.ScreenToWorldPoint(new Vector2(1f, 0f)) - Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f))).x * num);
	}

	public void ScaleLevelItem(Transform aItem, float aXScale, float aYScale, bool aIsPlayer)
	{
		float billboardScale = this.BillboardScale;
		aItem.localScale *= billboardScale;
		Vector3 localScale = aItem.localScale;
		localScale.x *= aXScale;
		localScale.y *= aYScale;
		aItem.localScale = localScale;
		Transform[] componentsInChildren = aItem.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			this.ScaleCollider(transform.GetComponent<Collider>(), aIsPlayer);
		}
	}

	private void ScaleCollider(Collider aCollider, bool aIsPlayer)
	{
		if (aCollider is SphereCollider)
		{
			this.ScaleCollider((SphereCollider)aCollider, aIsPlayer);
		}
		else if (aCollider is BoxCollider)
		{
			this.ScaleCollider((BoxCollider)aCollider, aIsPlayer);
		}
	}

	private void ScaleCollider(SphereCollider aCollider, bool aIsPlayer)
	{
		float billboardScale = this.BillboardScale;
		if (!aIsPlayer)
		{
			aCollider.radius -= this.mPlayerRadius;
		}
		aCollider.radius *= this.mLevelScale / billboardScale;
		aCollider.center *= this.mLevelScale / billboardScale;
	}

	private void ScaleCollider(BoxCollider aCollider, bool aIsPlayer)
	{
		float billboardScale = this.BillboardScale;
		if (!aIsPlayer)
		{
			Vector3 vector = new Vector3(this.mPlayerRadius, this.mPlayerRadius, this.mPlayerRadius);
			aCollider.size -= vector * 2f;
		}
		aCollider.size *= this.mLevelScale / billboardScale;
		aCollider.center *= this.mLevelScale / billboardScale;
	}

	private const float mkNativeHeight = 480f;

	private const float mkDPIFactor = 0.72f;

	private float mLevelScale = 1f;

	private float mPlayerRadius;

	private static ScaleItem m_cInstance;
}

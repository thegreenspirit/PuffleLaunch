using System;
using System.Collections.Generic;
using UnityEngine;

public class SharedSpriteManager : MonoBehaviour
{
	public void Awake()
	{
		this.m_MyTransform = base.transform;
		SharedSpriteManager.SharedSpriteRef sharedSpriteRef = null;
		if (SharedSpriteManager.smSharedSpriteManagers.ContainsKey(this.spriteName))
		{
			sharedSpriteRef = SharedSpriteManager.smSharedSpriteManagers[this.spriteName];
		}
		if (sharedSpriteRef == null)
		{
			SpriteManager spriteManager = (SpriteManager)global::UnityEngine.Object.Instantiate(this.sharedSpritePrefab);
			MeshRenderer component = spriteManager.GetComponent<MeshRenderer>();
			for (int i = 0; i < component.materials.Length; i++)
			{
				component.materials[i] = new Material(component.materials[i]);
				component.materials[i].mainTexture = null;
			}
			sharedSpriteRef = new SharedSpriteManager.SharedSpriteRef();
			sharedSpriteRef.sprite = spriteManager;
		}
		sharedSpriteRef.refCount++;
		SharedSpriteManager.smSharedSpriteManagers[this.spriteName] = sharedSpriteRef;
		sharedSpriteRef.sprite.clipchanged += this.OnClipChanged;
		base.GetComponent<MeshFilter>().sharedMesh = sharedSpriteRef.sprite.GetComponent<MeshFilter>().sharedMesh;
		base.GetComponent<MeshRenderer>().sharedMaterials = sharedSpriteRef.sprite.GetComponent<MeshRenderer>().materials;
	}

	private void Update()
	{
		if (StartOfGameDelay.Instance != null)
		{
			this.m_MyPos = this.m_MyTransform.position;
			this.m_MyTransform.position = this.m_MyPos;
		}
	}

	public void OnClipChanged(object sender, ClipChangedEventArgs e)
	{
		base.GetComponent<MeshFilter>().sharedMesh = ((SpriteManager)sender).GetComponent<MeshFilter>().sharedMesh;
	}

	public void OnDestroy()
	{
		SharedSpriteManager.SharedSpriteRef sharedSpriteRef = SharedSpriteManager.smSharedSpriteManagers[this.spriteName];
		if (--sharedSpriteRef.refCount == 0)
		{
			if (sharedSpriteRef.sprite)
			{
				global::UnityEngine.Object.Destroy(sharedSpriteRef.sprite.gameObject);
			}
			SharedSpriteManager.smSharedSpriteManagers.Remove(this.spriteName);
		}
	}

	public SpriteManager SharedInstance
	{
		get
		{
			return SharedSpriteManager.smSharedSpriteManagers[this.spriteName].sprite;
		}
	}

	public string spriteName;

	public SpriteManager sharedSpritePrefab;

	private static Dictionary<string, SharedSpriteManager.SharedSpriteRef> smSharedSpriteManagers = new Dictionary<string, SharedSpriteManager.SharedSpriteRef>();

	private Transform m_MyTransform;

	private Vector3 m_MyPos;

	private class SharedSpriteRef
	{
		public SpriteManager sprite;

		public int refCount;
	}
}

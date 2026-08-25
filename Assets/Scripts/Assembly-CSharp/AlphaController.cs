using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AlphaController : MonoBehaviour
{
	private void Update()
	{
		Color color = base.GetComponent<Renderer>().material.color;
		color.a = this.alphaValue;
		base.GetComponent<Renderer>().material.color = color;
	}

	public void DeleteSelf()
	{
		global::UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	public float alphaValue;
}

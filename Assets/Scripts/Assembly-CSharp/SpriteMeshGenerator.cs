using System;
using UnityEngine;

public class SpriteMeshGenerator
{
	public SpriteMeshGenerator(MeshFilter mf)
	{
		this.m_MeshFilter = mf;
		this.m_Vertices = new Vector3[4];
		this.m_Normals = new Vector3[4];
		this.m_TextureCoordinates = new Vector2[4];
		this.m_Triangles = new int[] { 0, 1, 2, 2, 3, 0 };
		this.m_TextureCoordinates[0].x = 0f;
		this.m_TextureCoordinates[0].y = 0f;
		this.m_TextureCoordinates[1].x = 0f;
		this.m_TextureCoordinates[1].y = 1f;
		this.m_TextureCoordinates[2].x = 1f;
		this.m_TextureCoordinates[2].y = 1f;
		this.m_TextureCoordinates[3].x = 1f;
		this.m_TextureCoordinates[3].y = 0f;
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, -1f)) - Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
		this.m_Normals[0] = vector;
		this.m_Normals[1] = vector;
		this.m_Normals[2] = vector;
		this.m_Normals[3] = vector;
		CameraFollow component = Camera.main.GetComponent<CameraFollow>();
		if (component)
		{
			this.m_AdjustZoom = true;
			this.m_OrthographicSize = component.OriginalOrthographicSize;
			if (this.m_OrthographicSize == 0f)
			{
				this.m_OrthographicSize = Camera.main.orthographicSize;
			}
		}
	}

	public void Generate(Vector2 aOffset, Vector2 aSize, bool aShared)
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
		this.m_Vertices[0] = Camera.main.ScreenToWorldPoint(new Vector3(aOffset.x, aOffset.y, 0f)) - vector;
		this.m_Vertices[1] = Camera.main.ScreenToWorldPoint(new Vector3(aOffset.x, aOffset.y + aSize.y, 0f)) - vector;
		this.m_Vertices[2] = Camera.main.ScreenToWorldPoint(new Vector3(aOffset.x + aSize.x, aOffset.y + aSize.y, 0f)) - vector;
		this.m_Vertices[3] = Camera.main.ScreenToWorldPoint(new Vector3(aOffset.x + aSize.x, aOffset.y, 0f)) - vector;
		if (this.m_AdjustZoom)
		{
			float num = this.m_OrthographicSize / Camera.main.orthographicSize;
			for (int i = 0; i < this.m_Vertices.Length; i++)
			{
				this.m_Vertices[i] *= num;
			}
		}
		Mesh mesh = new Mesh();
		if (aShared)
		{
			this.m_MeshFilter.sharedMesh = mesh;
		}
		else
		{
			this.m_MeshFilter.mesh = mesh;
		}
		mesh.vertices = this.m_Vertices;
		mesh.normals = this.m_Normals;
		mesh.uv = this.m_TextureCoordinates;
		mesh.triangles = this.m_Triangles;
	}

	public void Generate(object sender, ClipChangedEventArgs e)
	{
		if (e.current == null)
		{
			this.m_MeshFilter.mesh = null;
			return;
		}
		Vector2 offset = e.current.offset;
		Vector2 vector = new Vector2(e.current.stride.x - 1f, e.current.stride.y - 1f);
		this.Generate(offset, vector, ((SpriteManager)sender).sharedMaterial);
	}

	private MeshFilter m_MeshFilter;

	private Vector3[] m_Vertices;

	private Vector3[] m_Normals;

	private Vector2[] m_TextureCoordinates;

	private int[] m_Triangles;

	private bool m_AdjustZoom;

	private float m_OrthographicSize;
}

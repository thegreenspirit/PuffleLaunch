using System;
using UnityEngine;

[RequireComponent(typeof(ElasticMovement))]
public class PathFollower : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		this.mElasticMovement = base.GetComponent<ElasticMovement>();
		this.mCurrentNode = ((!this.reversed) ? 0 : (this.pathNodes.Length - 1));
		if (this.pathNodes.Length == 0)
		{
			base.enabled = false;
		}
		else
		{
			this.mElasticMovement.TargetPosition = this.pathNodes[this.mCurrentNode] * ScaleItem.Instance.LevelScale;
		}
	}

	public void FixedUpdate()
	{
		if ((this.mTransform.position - this.pathNodes[this.mCurrentNode] * ScaleItem.Instance.LevelScale).sqrMagnitude < Mathf.Pow(100f * ScaleItem.Instance.LevelScale, 2f))
		{
			if (!this.reversed)
			{
				if (++this.mCurrentNode == this.pathNodes.Length)
				{
					if (!this.loop)
					{
						base.enabled = false;
						return;
					}
					this.mCurrentNode = 0;
				}
			}
			else if (--this.mCurrentNode == -1)
			{
				if (!this.loop)
				{
					base.enabled = false;
					return;
				}
				this.mCurrentNode = this.pathNodes.Length - 1;
			}
			this.mElasticMovement.TargetPosition = this.pathNodes[this.mCurrentNode] * ScaleItem.Instance.LevelScale;
		}
	}

	public int CurrentNode
	{
		get
		{
			return this.mCurrentNode;
		}
		set
		{
			this.mCurrentNode = value;
		}
	}

	public Vector3[] pathNodes;

	public bool loop;

	public bool reversed;

	private Transform mTransform;

	private ElasticMovement mElasticMovement;

	private int mCurrentNode;
}

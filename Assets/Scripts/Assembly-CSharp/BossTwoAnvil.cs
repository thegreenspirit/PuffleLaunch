using System;
using System.Collections;
using UnityEngine;

public class BossTwoAnvil : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		this.mParentBoss = this.mTransform.root.GetComponent<BossTwo>();
		if (this.relativeObject != null && this.relativeObject.GetComponent<Renderer>() != null)
		{
			base.StartCoroutine(this.PositionAnvilToRelativeObject());
		}
		else if (SizeCategory.Instance.Category == "small")
		{
			this.mTransform.localPosition = new Vector3(-0.6f, -21.28f, 0.01f);
		}
		else if (SizeCategory.Instance.Category == "large")
		{
			this.mTransform.localPosition = new Vector3(-0.6f, -8.87f, 0.01f);
		}
	}

	private IEnumerator PositionAnvilToRelativeObject()
	{
		while (this.relativeObject.GetComponent<Renderer>().bounds.size == Vector3.zero)
		{
			yield return null;
		}
		Vector3 position = this.mTransform.position;
		position.y = this.relativeObject.transform.position.y - this.relativeObject.GetComponent<Renderer>().bounds.size.y / 2f - base.GetComponent<Renderer>().bounds.size.y;
		this.mTransform.position = position;
		yield break;
	}

	public void FixedUpdate()
	{
		this.mLastPosition = this.mTransform.position;
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (aOther.tag == "Player")
		{
			this.OnAnvilHit(aOther.GetComponent<Puffle>(), this.mParentBoss.IsAttacking);
		}
		else
		{
			ElasticMovement component = aOther.GetComponent<ElasticMovement>();
			if (component)
			{
				this.OnAnvilHit(component, this.mParentBoss.IsAttacking);
			}
		}
	}

	private void OnAnvilHit(Puffle aPlayer, bool aIsAttacking)
	{
		Vector3 vector = this.GetPushVector(aPlayer.transform.position, 25f);
		if (aIsAttacking)
		{
			vector -= this.GetPushVector(this.mLastPosition, 50f);
		}
		aPlayer.Velocity = vector;
		if (this.playerHitSFX)
		{
			AudioManager.Instance.PlayObstacleSound(this.playerHitSFX);
		}
	}

	private void OnAnvilHit(ElasticMovement aObstacle, bool aIsAttacking)
	{
		Vector3 vector = this.GetPushVector(aObstacle.transform.position, 50f);
		if (aIsAttacking)
		{
			vector -= this.GetPushVector(this.mLastPosition, 100f);
		}
		aObstacle.Velocity = vector;
		aObstacle.elasticMultiplier = 0.001f;
	}

	private Vector3 GetPushVector(Vector3 aTarget, float aForce)
	{
		Vector3 vector = aTarget - this.mTransform.position;
		float num = Mathf.Round(Mathf.Atan2(vector.y, vector.x) * 57.29578f);
		Vector3 vector2 = new Vector3(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f), 0f);
		return vector2 * aForce * ScaleItem.Instance.LevelScale;
	}

	private Transform mTransform;

	private BossTwo mParentBoss;

	private Vector3 mLastPosition;

	public AudioClip playerHitSFX;

	public Transform relativeObject;
}

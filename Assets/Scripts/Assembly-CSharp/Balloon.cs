using System;
using UnityEngine;

[RequireComponent(typeof(ElasticMovement))]
public class Balloon : MonoBehaviour
{
	public virtual void Start()
	{
		this.mTransform = base.transform;
		this.mElasticMovement = base.GetComponent<ElasticMovement>();
	}

	public void OnTriggerEnter(Collider aCollider)
	{
		if (aCollider.tag == "Player")
		{
			AudioManager.Instance.PlayObstacleSound(this.reboundSound);
			Puffle component = aCollider.GetComponent<Puffle>();
			Vector3 vector = this.mTransform.position - component.transform.position;
			float num = Mathf.Round(Mathf.Atan2(vector.y, vector.x) * 57.29578f);
			Vector3 vector2 = new Vector3(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f), 0f);
			vector2 *= this.pushForce * ScaleItem.Instance.LevelScale;
			this.mElasticMovement.Velocity += vector2;
			this.ReboundPlayer(component, vector2);
			base.GetComponent<ElasticMovement>().UpdateTransform(1f);
		}
	}

	protected virtual void ReboundPlayer(Puffle aPuffle, Vector3 aPush)
	{
		if (aPuffle.Velocity.y <= 0f)
		{
			Vector3 vector = new Vector3(aPuffle.Velocity.x * 0.5f, (Mathf.Abs(aPush.y) + Mathf.Abs(aPush.x)) * 0.5f, 0f);
			aPuffle.Velocity = vector;
			aPuffle.AngularVelocity = (Mathf.Abs(aPush.x) + Mathf.Abs(aPush.y)) / ScaleItem.Instance.LevelScale;
		}
	}

	public AudioClip reboundSound;

	public float pushForce = 50f;

	private Transform mTransform;

	private Vector3 mVelocity;

	private ElasticMovement mElasticMovement;
}

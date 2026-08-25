using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ElasticMovement))]
public class Asteroid : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		this.mElasticMovement = base.GetComponent<ElasticMovement>();
		this.mSpriteManager = base.GetComponent<SpriteManager>();
		this.mSpriteManager.Seek(Random.Range(0, this.mSpriteManager.GetCurrAnimTotalFrames()));
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (aOther.tag == "Player")
		{
			TimeManager.Instance.StopSlowmo();
			Puffle component = aOther.GetComponent<Puffle>();
			Vector3 vector = component.transform.position - this.mTransform.position;
			float num = Mathf.Round(Mathf.Atan2(vector.y, vector.x) * 57.29578f);
			Vector3 vector2 = new Vector3(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f), 0f);
			vector2 *= 50f * ScaleItem.Instance.LevelScale;
			component.Velocity = vector2 * 0.8f;
			this.mElasticMovement.Velocity = vector2 * -0.25f;
		}
	}

	private Transform mTransform;

	private SpriteManager mSpriteManager;

	private ElasticMovement mElasticMovement;
}

using System;
using UnityEngine;

public class PuffleBumper : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (aOther.tag == "Player")
		{
			Puffle component = aOther.GetComponent<Puffle>();
			Vector3 vector = component.transform.position - this.mTransform.position;
			float num = Mathf.Round(Mathf.Atan2(vector.y, vector.x) * 57.29578f);
			Vector3 vector2 = new Vector3(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f), 0f);
			vector2 *= this.bounceStrength * ScaleItem.Instance.LevelScale;
			component.Velocity = vector2 * 0.8f;
			if (this.impactSound != null)
			{
				AudioManager.Instance.PlayObstacleSound(this.impactSound);
			}
			if (this.hitAnimation != null)
			{
				if (vector.x < 0f)
				{
					this.hitAnimation.Play("ObstacleHitLeft");
				}
				else
				{
					this.hitAnimation.Play("ObstacleHitRight");
				}
			}
		}
	}

	public AudioClip impactSound;

	public float bounceStrength = 1f;

	public Animation hitAnimation;

	private Transform mTransform;
}

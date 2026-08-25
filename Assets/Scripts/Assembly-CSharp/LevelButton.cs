using System;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
	private void Awake()
	{
		this.m_Transform = base.transform;
		this.m_TargetScale = new Vector3(1f, 1f, 1f);
		this.m_TargetPosition = new Vector3(0f, -20f, 0f);
	}

	private void Update()
	{
		if (this.m_Transform.localScale != this.m_TargetScale)
		{
			this.ChangeScale();
		}
		if (this.m_Transform.rotation.y != this.m_TargetRotation)
		{
			this.ChangeRotation();
		}
		if (this.m_Transform.position != this.m_TargetPosition)
		{
			this.ChangePosition();
		}
	}

	public static LevelButton Instantiate(int aLevel, int aWorld, int aLevelsPerWorld)
	{
		LevelButton levelButton = null;
		GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("GUI/LevelSelect/Prefabs/LevelButton", typeof(GameObject))) as GameObject;
		if (gameObject != null)
		{
			levelButton = gameObject.GetComponent<LevelButton>();
			if (levelButton != null)
			{
				levelButton.SetLevel(aLevel, aWorld, aLevelsPerWorld);
			}
		}
		return levelButton;
	}

	public void SetLevel(int aLevel, int aWorld, int aLevelsPerWorld)
	{
		this.m_LevelNumber.text = (aLevel + aWorld * aLevelsPerWorld + 1).ToString();
		this.m_LevelNumberShadow.text = (aLevel + aWorld * aLevelsPerWorld + 1).ToString();
		this.m_TimeTrial.text = string.Empty;
		this.m_TimeTrialShadow.text = string.Empty;
		int num = aLevel + aWorld * aLevelsPerWorld;
		if (GameManager.HasCompletedTurboMode(GameManager.Instance.CurrentWorld) || GameManager.Instance.HasAchievedTimeTrialFire(GameManager.Instance.CurrentWorld))
		{
			this.SetButtonToTurboMode(num);
		}
		else if (GameManager.HasCollectedAllRings(GameManager.Instance.CurrentWorld))
		{
			this.SetButtonToTimeTrialMode(num);
		}
		else
		{
			this.SetButtonToRegularMode(num);
		}
	}

	private void ChangePosition()
	{
		this.m_WorkingVector = Vector3.Lerp(this.m_Transform.position, this.m_TargetPosition, Time.deltaTime * this.m_MoveSpeed);
		if ((this.m_TargetPosition - this.m_WorkingVector).magnitude < this.m_SnapDistance)
		{
			this.m_WorkingVector = this.m_TargetPosition;
		}
		this.m_Transform.position = this.m_WorkingVector;
	}

	private void ChangeRotation()
	{
		this.m_WorkingVector = this.m_Transform.eulerAngles;
		this.m_WorkingVector.y = Mathf.Lerp(this.m_Transform.eulerAngles.y, this.m_TargetRotation, Time.deltaTime * this.m_RotateSpeed);
		if (Mathf.Abs(this.m_TargetRotation - this.m_WorkingVector.y) < this.m_SnapRotation)
		{
			this.m_WorkingVector.y = this.m_TargetRotation;
		}
		this.m_Transform.eulerAngles = this.m_WorkingVector;
	}

	private void ChangeScale()
	{
		this.m_WorkingVector = Vector3.Lerp(this.m_Transform.localScale, this.m_TargetScale, Time.deltaTime * this.m_ScaleChangeSpeed);
		if ((this.m_TargetScale - this.m_WorkingVector).magnitude < this.m_SnapScale)
		{
			this.m_WorkingVector = this.m_TargetScale;
		}
		this.m_Transform.localScale = this.m_WorkingVector;
	}

	public void SetTargetPosition(Vector3 aPosition)
	{
		this.m_TargetPosition = aPosition;
	}

	public void SetTargetScale(Vector3 aScale)
	{
		this.m_TargetScale.x = aScale.x;
		this.m_TargetScale.y = this.m_Transform.localScale.z;
		this.m_TargetScale.z = aScale.y;
	}

	public void SetTargetRotation(float aYRotation)
	{
		this.m_TargetRotation = aYRotation;
	}

	public void SetInstantPosition(Vector3 aPosition)
	{
		this.SetTargetPosition(aPosition);
		this.m_Transform.position = aPosition;
	}

	public void SetInstantScale(Vector3 aScale)
	{
		this.SetTargetScale(aScale);
		this.m_Transform.localScale = this.m_TargetScale;
	}

	private void SetButtonToTurboMode(int aGlobalLevel)
	{
		base.GetComponent<Renderer>().material = this.m_CompleteMaterial;
		if (ProfileManager.Instance.CurrentProfile.m_LevelData[aGlobalLevel].BestTimeCount != float.MaxValue)
		{
			string timeFormatedString = GameManager.GetTimeFormatedString(ProfileManager.Instance.CurrentProfile.m_LevelData[aGlobalLevel].BestTimeCount);
			this.m_TimeTrial.text = timeFormatedString;
			this.m_TimeTrialShadow.text = timeFormatedString;
		}
	}

	private void SetButtonToTimeTrialMode(int aGlobalLevel)
	{
		base.GetComponent<Renderer>().material = this.m_CompleteMaterial;
		if (ProfileManager.Instance.CurrentProfile.m_LevelData[aGlobalLevel].BestTimeCount != float.MaxValue)
		{
			string timeFormatedString = GameManager.GetTimeFormatedString(ProfileManager.Instance.CurrentProfile.m_LevelData[aGlobalLevel].BestTimeCount);
			this.m_TimeTrial.text = timeFormatedString;
			this.m_TimeTrialShadow.text = timeFormatedString;
		}
	}

	private void SetButtonToRegularMode(int aGlobalLevel)
	{
		if (ProfileManager.Instance.CurrentProfile.m_LevelData[aGlobalLevel].LevelComplete)
		{
			base.GetComponent<Renderer>().material = this.m_CompleteMaterial;
		}
		else if (ProfileManager.Instance.CurrentProfile.m_LevelData[aGlobalLevel].LevelUnlocked)
		{
			base.GetComponent<Renderer>().material = this.m_UnlockedMaterial;
		}
		else
		{
			base.GetComponent<Renderer>().material = this.m_LockedMaterial;
		}
	}

	public float m_MoveSpeed = 2f;

	public float m_RotateSpeed = 1f;

	public float m_ScaleChangeSpeed = 10f;

	public TextMesh m_LevelNumber;

	public TextMesh m_LevelNumberShadow;

	public TextMesh m_TimeTrial;

	public TextMesh m_TimeTrialShadow;

	public Material m_LockedMaterial;

	public Material m_UnlockedMaterial;

	public Material m_CompleteMaterial;

	private Vector3 m_WorkingVector = default(Vector3);

	private Vector3 m_TargetScale;

	public Vector3 m_TargetPosition;

	private float m_TargetRotation;

	private Transform m_Transform;

	private float m_SnapDistance = 0.02f;

	private float m_SnapRotation = 5f;

	private float m_SnapScale = 0.01f;
}

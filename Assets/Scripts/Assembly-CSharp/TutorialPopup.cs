using System;
using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
	public static TutorialPopup Instance
	{
		get
		{
			return TutorialPopup.m_singleton;
		}
	}

	public bool TutorialActive
	{
		get
		{
			return this.m_isActive;
		}
	}

	private void Start()
	{
		Camera.main.GetComponent<CameraFollow>().ZoomEnabled = false;
		this.mInputController = GameFlowManager.Instance.InputController;
		if (GameManager.smCurrentLevel == GameManager.Level.eLevel_3)
		{
			this.SelectTutorial(1);
		}
		else if (GameManager.smCurrentLevel == GameManager.Level.eLevel_6)
		{
			this.SelectTutorial(5);
		}
		ScaleItem.Instance.ScaleLevelItem(this.mo_Puffle.transform, 1f, 1f, true);
		ScaleItem.Instance.ScaleLevelItem(this.mo_PurpleCannon.transform, 1f, 1f, false);
		ScaleItem.Instance.ScaleLevelItem(this.mo_TurorialPrefab.transform.Find("Cloud1").transform, 1f, 1f, false);
		ScaleItem.Instance.ScaleLevelItem(this.mo_TurorialPrefab.transform.Find("Cloud2").transform, 1f, 1f, false);
	}

	private void Awake()
	{
		TutorialPopup.m_singleton = this;
		this.m_isActive = true;
		this.mo_TurorialPrefab = base.gameObject;
		this.mo_RightThumb = this.mo_TurorialPrefab.transform.Find("RightThumb").gameObject;
		this.mo_LeftThumb = this.mo_TurorialPrefab.transform.Find("LeftThumb").gameObject;
		this.mo_TouchIndicator = this.mo_TurorialPrefab.transform.Find("TouchScaler").gameObject;
		this.mo_Border = this.mo_TurorialPrefab.transform.Find("Border").gameObject;
		this.m_Camera = this.mo_TurorialPrefab.transform.Find("Camera").GetComponent<Camera>();
		this.mo_Puffle = this.mo_TurorialPrefab.transform.Find("Puffle").gameObject;
		this.mo_PurpleCannon = this.mo_TurorialPrefab.transform.Find("ControllableCannon").gameObject;
		float num = 1.5f;
		float num2 = 0.7348f;
		float num3 = 0.13f;
		float num4 = (float)Screen.width / (float)Screen.height;
		float num5 = num / num4 * num2;
		float num6 = (num2 - num5) * 0.5f + num3;
		this.m_Camera.rect = new Rect(num6, 0.12f, num5, 0.79f);
	}

	private void DestroyTutorial()
	{
		this.m_isActive = false;
		Camera.main.GetComponent<CameraFollow>().ZoomEnabled = true;
		global::UnityEngine.Object.DestroyImmediate(this.mo_TurorialPrefab);
	}

	private void CloseAllTutorial()
	{
		this.mo_Puffle.SetActive(false);
		this.mo_PurpleCannon.SetActive(false);
	}

	public void SelectTutorial(int aTutorialNum)
	{
		this.m_CurrentTutorial = (TutorialPopup.Tutorial)aTutorialNum;
		this.CloseAllTutorial();
		this.mo_Puffle.SetActive(true);
		this.mo_TurorialPrefab.GetComponent<Animation>().Stop();
		this.mo_TurorialPrefab.GetComponent<Animation>().Play("Tutorial" + aTutorialNum);
		switch (aTutorialNum)
		{
		case 0:
			this.ResetPuffle();
			this.ResetThumbs();
			break;
		case 1:
			this.ResetPuffle();
			break;
		case 2:
			this.ResetPuffle();
			this.ResetThumbs();
			break;
		case 3:
			this.ResetThumbs();
			break;
		case 4:
			this.ResetThumbs();
			break;
		case 5:
			this.mo_PurpleCannon.SetActive(true);
			this.mo_Puffle.GetComponentInChildren<ParticleEmitter>().emit = false;
			break;
		}
	}

	private void LoadNewTexture(Transform button, int index)
	{
		Dynamic3DBackground component = button.GetComponent<Dynamic3DBackground>();
		string name = button.name;
		switch (name)
		{
		case "Next":
			component.LoadNewTexture(this.nextButtonPath, this.nextButtonTexture[index]);
			break;
		case "Back":
			component.LoadNewTexture(this.backButtonPath, this.backButtonTexture[index]);
			break;
		case "Close":
			component.LoadNewTexture(this.closeButtonPath, this.closeButtonTexture[index]);
			break;
		}
	}

	private void ResetPuffle()
	{
		this.SetPuffleSprite(0);
		this.mo_Puffle.transform.parent = this.mo_TurorialPrefab.transform;
		this.mo_Puffle.transform.eulerAngles = new Vector3(0f, 0f, 0f);
		this.mo_Puffle.GetComponentInChildren<ParticleEmitter>().emit = false;
	}

	private void ResetThumbs()
	{
		this.mo_LeftThumb.transform.localPosition = new Vector3(14f, 7f, 2f);
		this.mo_RightThumb.transform.localPosition = new Vector3(-14f, 7f, 2f);
	}

	private void Update()
	{
		if (GameFlowManager.Instance.m_DoWindowBack && !GameFlowManager.Instance.GUIManager.IsPauseMenu)
		{
			this.DestroyTutorial();
			GameFlowManager.Instance.m_DoWindowBack = false;
			return;
		}
		if (this.mInputController.TouchCount > 0)
		{
			if (this.mInputController.TouchDown)
			{
				this.mCurrentSelection = null;
				foreach (Transform transform in this.mButtonList)
				{
					Bounds bounds = transform.GetComponent<Renderer>().bounds;
					Vector3 center = bounds.center;
					center.z = 0f;
					bounds.center = center;
					bounds.Expand(3f);
					Vector3 vector = this.m_Camera.ScreenToWorldPoint(this.mInputController.TouchPosition1);
					vector.z = 0f;
					if (bounds.Contains(vector))
					{
						this.mCurrentSelection = transform;
						break;
					}
				}
				if (this.mCurrentSelection != null)
				{
					this.LoadNewTexture(this.mCurrentSelection, 1);
				}
			}
			else if (this.mCurrentSelection != null)
			{
				Bounds bounds2 = this.mCurrentSelection.GetComponent<Renderer>().bounds;
				Vector3 center2 = bounds2.center;
				center2.z = 0f;
				bounds2.center = center2;
				bounds2.Expand(3f);
				Vector3 vector2 = this.m_Camera.ScreenToWorldPoint(this.mInputController.TouchPosition1);
				vector2.z = 0f;
				if (bounds2.Contains(vector2))
				{
					if (this.mExitedButton)
					{
						this.LoadNewTexture(this.mCurrentSelection, 1);
						this.mExitedButton = false;
					}
				}
				else if (!this.mExitedButton)
				{
					this.LoadNewTexture(this.mCurrentSelection, 0);
					this.mExitedButton = true;
				}
			}
		}
		else if (this.mCurrentSelection != null)
		{
			this.LoadNewTexture(this.mCurrentSelection, 0);
			if (this.mInputController.Release && !this.mExitedButton)
			{
				string name = this.mCurrentSelection.name;
				switch (name)
				{
					case "Next":
					{
						int num2 = ((int)this.m_CurrentTutorial + 1) % (int)TutorialPopup.Tutorial.eTutorial_COUNT;
						break;
					}
					case "Back":
					{
						int num3 = ((int)this.m_CurrentTutorial - (int)TutorialPopup.Tutorial.ePuffleControlTutorial) % 7;
						num3 = Mathf.Clamp(num3, 0, 7);
						break;
					}
					case "Close":
						this.DestroyTutorial();
						break;
				}
				this.mCurrentSelection = null;
			}
			this.mExitedButton = false;
		}
	}

	private void SetPuffleSprite(int index)
	{
		if (index == 0)
		{
			this.mo_Puffle.GetComponent<SpriteManager>().Seek(1);
		}
		else
		{
			this.mo_Puffle.GetComponent<SpriteManager>().Seek(11);
		}
	}

	private void LaunchPuffleAnim()
	{
		switch (this.m_CurrentTutorial)
		{
		case TutorialPopup.Tutorial.ePurpleCannonTutorial:
			this.mo_PurpleCannon.GetComponentInChildren<TweeningController>().Play(true);
			break;
		}
		this.mo_Puffle.transform.parent = this.mo_TurorialPrefab.transform;
		this.SetPuffleSprite(0);
	}

	private void SetPuffleParent()
	{
		switch (this.m_CurrentTutorial)
		{
		case TutorialPopup.Tutorial.ePurpleCannonTutorial:
			this.mo_Puffle.transform.parent = this.mo_PurpleCannon.transform;
			this.mo_Puffle.transform.localRotation = Quaternion.identity;
			break;
		}
	}

	private void ShowPressAnim(int index)
	{
		Transform transform = this.mo_TouchIndicator.transform;
		if (index == 0)
		{
			transform.parent = this.mo_LeftThumb.transform;
			this.mo_LeftThumb.transform.localScale = Vector3.one * 1.85f;
		}
		else
		{
			transform.parent = this.mo_RightThumb.transform;
			Vector3 vector = Vector3.one * 1.85f;
			vector.x *= -1f;
			this.mo_RightThumb.transform.localScale = vector;
		}
		transform.localPosition = new Vector3(1f, -1f, -1f);
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one * 0.5f;
		this.mo_TouchIndicator.GetComponentInChildren<MeshRenderer>().enabled = true;
	}

	private void RemovePressAnim()
	{
		this.mo_LeftThumb.transform.localScale = Vector3.one * 2f;
		Vector3 vector = Vector3.one * 2f;
		vector.x *= -1f;
		this.mo_RightThumb.transform.localScale = vector;
		this.mo_TouchIndicator.GetComponentInChildren<MeshRenderer>().enabled = false;
	}

	private const string kNextButtonName = "Next";

	private const string kBackButtonName = "Back";

	private const string kCloseButtonName = "Close";

	public string nextButtonPath;

	public string[] nextButtonTexture;

	public string backButtonPath;

	public string[] backButtonTexture;

	public string closeButtonPath;

	public string[] closeButtonTexture;

	public Transform[] mButtonList;

	private Transform mCurrentSelection;

	private bool mExitedButton;

	private InputController mInputController;

	private static TutorialPopup m_singleton;

	private bool m_isActive;

	private GameObject mo_TurorialPrefab;

	private TutorialPopup.Tutorial m_CurrentTutorial;

	private GameObject mo_RightThumb;

	private GameObject mo_LeftThumb;

	private GameObject mo_TouchIndicator;

	private GameObject mo_Border;

	private Camera m_Camera;

	private GameObject mo_Puffle;

	private GameObject mo_PurpleCannon;

	private enum Tutorial
	{
		eGravityTutorial,
		ePuffleControlTutorial,
		eGiantPuffleOTutorial,
		eGreenCannonTutorial,
		eRedCannonTutorial,
		ePurpleCannonTutorial,
		eSlingshotTutorial,
		eTutorial_COUNT
	}
}

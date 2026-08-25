using System;
using UnityEngine;

public class CustomButton3D : CustomGUI3DItem
{
	public event CustomOnSelectHandler customOnSelect;

	public bool Enabled
	{
		get
		{
			return this.mEnabled;
		}
		set
		{
			this.mEnabled = value;
		}
	}

	private void Start()
	{
		this.InitPosition();
		this.InitButtonBounds();
		this.mTextureState = CustomButton3D.TextureState.eTextureState_Default;
		this.mState = CustomButton3D.TouchState.eTouchState_Released;
		this.mInputController = GameFlowManager.Instance.InputController;
		this.mRenderCamera = Camera.main;
		Camera[] allCameras = Camera.allCameras;
		for (int i = 0; i < allCameras.Length; i++)
		{
			if (allCameras[i].name == this.renderCameraName)
			{
				this.mRenderCamera = allCameras[i];
			}
		}
	}

	private void Update()
	{
		if (!this.Enabled)
		{
			if (this.mTextureState != CustomButton3D.TextureState.eTextureState_Default)
			{
				this.LoadDefaultStateTexture();
			}
			return;
		}
		if (this.mInputController.TouchCount > 0)
		{
			if (this.mState == CustomButton3D.TouchState.eTouchState_Released && this.ContainsTouch() && this.mInputController.TouchDown && !this.mDisableTouch)
			{
				if (this.mb_togglable)
				{
					this.mb_toggleState = !this.mb_toggleState;
					if (this.customOnSelect != null)
					{
						this.customOnSelect(this, EventArgs.Empty);
						if (this.mb_toggleState)
						{
							this.LoadPressStateTexture();
						}
						else
						{
							this.LoadDefaultStateTexture();
						}
					}
					this.mState = CustomButton3D.TouchState.eTouchState_Touching;
				}
				else
				{
					if (this.customOnSelect != null)
					{
						this.customOnSelect(this, EventArgs.Empty);
					}
					this.LoadPressStateTexture();
					this.mState = CustomButton3D.TouchState.eTouchState_Touching;
				}
			}
		}
		else if (this.mState == CustomButton3D.TouchState.eTouchState_Touching)
		{
			if (!this.mb_togglable)
			{
				this.LoadDefaultStateTexture();
			}
			this.mState = CustomButton3D.TouchState.eTouchState_Released;
		}
	}

	private void LoadPressStateTexture()
	{
		this.mTextureState = CustomButton3D.TextureState.eTextureState_Pressed;
		this.buttonRenderer.material.mainTexture = GUIUtil.LoadTexture2D(this.pressState);
	}

	private void LoadDefaultStateTexture()
	{
		this.mTextureState = CustomButton3D.TextureState.eTextureState_Default;
		this.buttonRenderer.material.mainTexture = GUIUtil.LoadTexture2D(this.defaultState);
	}

	public void InitButtonBounds()
	{
		Bounds bounds = this.buttonTransform.GetComponent<Renderer>().bounds;
		Vector3 center = bounds.center;
		center.z = 0f;
		bounds.center = center;
		this.mo_buttonBounds = new Bounds(center, new Vector3(this.buttonTransform.GetComponent<Renderer>().bounds.size.x * this.mf_detectionZoneScale, this.buttonTransform.GetComponent<Renderer>().bounds.size.y * this.mf_detectionZoneScale, this.buttonTransform.GetComponent<Renderer>().bounds.size.z * this.mf_detectionZoneScale));
	}

	public void RegisterCallback()
	{
	}

	public bool ContainsTouch()
	{
		Vector3 vector = this.mRenderCamera.ScreenToWorldPoint(this.mInputController.TouchPosition1);
		vector.z = 0f;
		return this.mo_buttonBounds.Contains(vector);
	}

	public bool ContainsTouchRelease()
	{
		Vector3 vector = this.mRenderCamera.ScreenToWorldPoint(this.mInputController.ReleasePosition);
		vector.z = 0f;
		return this.mo_buttonBounds.Contains(vector) && this.mInputController.Release;
	}

	public void DisableTouch(bool aDisable)
	{
		this.mDisableTouch = aDisable;
	}

	public string defaultState;

	public string pressState;

	public Transform buttonTransform;

	public MeshRenderer buttonRenderer;

	public string renderCameraName;

	public bool mb_togglable;

	public bool mb_toggleState;

	public float mf_detectionZoneScale = 1f;

	private InputController mInputController;

	private CustomButton3D.TextureState mTextureState;

	private CustomButton3D.TouchState mState;

	private bool mEnabled = true;

	private bool mDisableTouch;

	private Camera mRenderCamera;

	private Bounds mo_buttonBounds;

	private enum TouchState
	{
		eTouchState_Touching,
		eTouchState_Released,
		eTouchState_COUNT
	}

	private enum TextureState
	{
		eTextureState_Default,
		eTextureState_Pressed
	}
}

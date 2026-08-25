using System;
using UnityEngine;

public class Button3DPressStateController : MonoBehaviour
{
	public event OnPressedHandler onPressed;

	public event OnReleasedHandler onReleased;

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
		this.mTextureState = Button3DPressStateController.TextureState.eTextureState_Default;
		this.mState = Button3DPressStateController.TouchState.eTouchState_Released;
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
			if (this.mTextureState != Button3DPressStateController.TextureState.eTextureState_Default)
			{
				this.LoadDefaultStateTexture();
			}
			return;
		}
		if (this.mInputController.TouchCount > 0)
		{
			if (this.mState == Button3DPressStateController.TouchState.eTouchState_Released)
			{
				Bounds bounds = this.buttonTransform.GetComponent<Renderer>().bounds;
				Vector3 center = bounds.center;
				center.z = 0f;
				bounds.center = center;
				Vector3 vector = this.mRenderCamera.ScreenToWorldPoint(this.mInputController.TouchPosition1);
				vector.z = 0f;
				if (bounds.Contains(vector))
				{
					if (this.onPressed != null)
					{
						this.onPressed(this, EventArgs.Empty);
					}
					this.LoadPressStateTexture();
					this.mState = Button3DPressStateController.TouchState.eTouchState_Touching;
				}
			}
			else
			{
				Bounds bounds2 = this.buttonTransform.GetComponent<Renderer>().bounds;
				Vector3 center2 = bounds2.center;
				center2.z = 0f;
				bounds2.center = center2;
				Vector3 vector2 = this.mRenderCamera.ScreenToWorldPoint(this.mInputController.TouchPosition1);
				vector2.z = 0f;
				if (bounds2.Contains(vector2))
				{
					if (this.mExitedButton)
					{
						this.LoadPressStateTexture();
						this.mExitedButton = false;
					}
				}
				else if (!this.mExitedButton)
				{
					this.LoadDefaultStateTexture();
					this.mExitedButton = true;
				}
			}
		}
		else
		{
			if (this.mState == Button3DPressStateController.TouchState.eTouchState_Touching)
			{
				if (this.onReleased != null)
				{
					this.onReleased(this, EventArgs.Empty);
				}
				this.LoadDefaultStateTexture();
				this.mState = Button3DPressStateController.TouchState.eTouchState_Released;
			}
			this.mExitedButton = false;
		}
	}

	private void LoadPressStateTexture()
	{
		this.mTextureState = Button3DPressStateController.TextureState.eTextureState_Pressed;
		this.buttonRenderer.material.mainTexture = GUIUtil.LoadTexture2D(this.pressState);
	}

	private void LoadDefaultStateTexture()
	{
		this.mTextureState = Button3DPressStateController.TextureState.eTextureState_Default;
		this.buttonRenderer.material.mainTexture = GUIUtil.LoadTexture2D(this.defaultState);
	}

	public void RegisterCallback()
	{
	}

	public string defaultState;

	public string pressState;

	public Transform buttonTransform;

	public MeshRenderer buttonRenderer;

	public string renderCameraName;

	private InputController mInputController;

	private Button3DPressStateController.TextureState mTextureState;

	private Button3DPressStateController.TouchState mState;

	private bool mExitedButton;

	private bool mEnabled = true;

	private Camera mRenderCamera;

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

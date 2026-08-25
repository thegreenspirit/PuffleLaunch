using System;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		this.mBarSpriteManager = base.GetComponent<SpriteManager>();
		this.mShineSpriteManager = this.mShine.GetComponent<SpriteManager>();
		this.mTotalPuffleOs = LevelLoader.Instance.NumPuffleOs;
		this.mBarSpriteManager.clipchanged += this.TilesChangedEventHandler;
		this.mBaseOrthographicSize = this.mCamera.orthographicSize;
		this.mBaseScale = this.mTransform.localScale;
		this.mTextShown = new bool[3];
		this.mTextShown[0] = (this.mTextShown[1] = (this.mTextShown[2] = false));
		this.mTextMesh = this.progressText.GetComponent<TextMesh>();
		if (this.progressText.textShadow != null)
		{
			this.mTextMeshShadow = this.progressText.textShadow.GetComponent<TextMesh>();
		}
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eLowres)
		{
			this.mBaseScale *= 0.5f;
			Transform transform = this.mTransform.Find("Timer").transform;
			transform.localScale *= 2f;
			transform.localPosition *= 2f;
		}
	}

	public void Update()
	{
		if (this.mTotalPuffleOs > 0)
		{
			if (this.mCurrentFrame >= 6 && this.mCurrentFrame <= 11 && this.mShineSpriteManager.CurrentAnimation() != 0)
			{
				this.mShineSpriteManager.Play("Shine");
			}
			if (this.mPercentageCollected >= (float)(this.mCurrentFrame + 1) * 8.333333f)
			{
				this.mCurrentFrame++;
				this.DoTextAnimation();
				if (this.mCurrentFrame <= 11)
				{
					this.mBarSpriteManager.Seek(this.mCurrentFrame);
				}
				else
				{
					this.mShineSpriteManager.Play("EmptyAnim");
					this.mBarSpriteManager.Play("Finished");
				}
			}
		}
	}

	public void LateUpdate()
	{
		float num = this.mCamera.orthographicSize / this.mBaseOrthographicSize;
		this.mTransform.localScale = this.mBaseScale * num;
		this.mScreenPosition.x = (float)Screen.width;
		this.mScreenPosition.y = (float)Screen.height;
		this.mScreenPosition.z = 2f;
		this.mScreenPosition = this.mCamera.ScreenToWorldPoint(this.mScreenPosition);
		this.mTempMeshSize = base.GetComponent<Renderer>().bounds.size;
		this.mScreenPosition.x = this.mScreenPosition.x - this.mTempMeshSize.x * 0.6f;
		this.mScreenPosition.y = this.mScreenPosition.y - this.mTempMeshSize.y * 0.65f;
		this.mTransform.position = this.mScreenPosition;
	}

	public void CollectPuffleO()
	{
		this.mCollectedPuffleOs++;
		this.mPercentageCollected = (float)this.mCollectedPuffleOs / (float)this.mTotalPuffleOs * 100f;
	}

	public void TilesChangedEventHandler(object sender, ClipChangedEventArgs e)
	{
		if (!this.initialized)
		{
			this.initialized = true;
			ScaleItem.Instance.ScaleLevelItem(this.mTransform, 1f, 1f, false);
			this.mTempMeshSize = base.GetComponent<Renderer>().bounds.size;
			this.mTempLocalPosition = this.mTransform.localPosition;
			this.mTempLocalPosition.x = this.mTempLocalPosition.x - this.mTempMeshSize.x * 0.6f;
			this.mTempLocalPosition.y = this.mTempLocalPosition.y - this.mTempMeshSize.y * 0.65f;
			this.mTransform.localPosition = this.mTempLocalPosition;
		}
	}

	private void DoTextAnimation()
	{
		if (this.mPercentageCollected == 100f)
		{
			this.mTextMesh.text = LocalizationManager.Instance.GetString("TXT_Good4");
			if (this.mTextMeshShadow != null)
			{
				this.mTextMeshShadow.text = this.mTextMesh.text;
			}
			this.progressText.Show = true;
		}
		else if (this.mPercentageCollected >= 75f && !this.mTextShown[2])
		{
			this.mTextShown[2] = true;
			this.mTextMesh.text = LocalizationManager.Instance.GetString("TXT_Good3");
			if (this.mTextMeshShadow != null)
			{
				this.mTextMeshShadow.text = this.mTextMesh.text;
			}
			this.progressText.Show = true;
		}
		else if (this.mPercentageCollected >= 50f && !this.mTextShown[1])
		{
			this.mTextShown[1] = true;
			this.mTextMesh.text = LocalizationManager.Instance.GetString("TXT_Good2");
			if (this.mTextMeshShadow != null)
			{
				this.mTextMeshShadow.text = this.mTextMesh.text;
			}
			this.progressText.Show = true;
		}
		else if (this.mPercentageCollected >= 25f && !this.mTextShown[0])
		{
			this.mTextShown[0] = true;
			this.mTextMesh.text = LocalizationManager.Instance.GetString("TXT_Good1");
			if (this.mTextMeshShadow != null)
			{
				this.mTextMeshShadow.text = this.mTextMesh.text;
			}
			this.progressText.Show = true;
		}
	}

	public int TotalPuffleOs
	{
		get { return this.mTotalPuffleOs; }
		set { this.mTotalPuffleOs = value; }
	}

	public GameObject mShine;

	public Camera mCamera;

	public ProgressText progressText;

	private Transform mTransform;

	private SpriteManager mBarSpriteManager;

	private SpriteManager mShineSpriteManager;

	private TextMesh mTextMesh;

	private TextMesh mTextMeshShadow;

	private int mCollectedPuffleOs;

	private int mTotalPuffleOs;

	private float mPercentageCollected;

	private int mCurrentFrame;

	private bool[] mTextShown;

	private bool initialized;

	private float mBaseOrthographicSize;

	private Vector3 mBaseScale;

	private Vector3 mScreenPosition = default(Vector3);

	private Vector3 mTempMeshSize;

	private Vector3 mTempLocalPosition;
}

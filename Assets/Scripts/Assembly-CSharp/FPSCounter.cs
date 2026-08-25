using System;
using System.Threading;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	private void Awake()
	{
		if (FPSCounter.isCreated)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			global::UnityEngine.Object.DontDestroyOnLoad(this);
			FPSCounter.isCreated = true;
		}
	}

	private void Start()
	{
		this.timeleft = this.updateInterval;
		if (this.FPSText != null)
		{
			this.FPSText.pixelOffset = this.fpsPos;
		}
		if (this.FPSTextShadow != null)
		{
			this.FPSTextShadow.pixelOffset = this.fpsPos + this.fpsShadowPosOffset;
			this.FPSTextShadow.material.color = Color.black;
		}
	}

	private void Update()
	{
		if (!this.showFPS)
		{
			return;
		}
		if (this.FPSCap > 0f)
		{
			Thread.Sleep((int)(1000f / this.FPSCap));
		}
		this.timeleft -= Time.deltaTime;
		this.accum += Time.timeScale / Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			float num = this.accum / (float)this.frames;
			this.fpsText = string.Format("{0:F2} FPS", num);
			if (num > 25f)
			{
				this.fpsColor = Color.green;
			}
			else if (num > 10f)
			{
				this.fpsColor = Color.yellow;
			}
			else
			{
				this.fpsColor = Color.red;
			}
			if (this.FPSText != null)
			{
				this.FPSText.material.color = this.fpsColor;
				this.FPSText.text = this.fpsText;
			}
			if (this.FPSTextShadow != null)
			{
				this.FPSTextShadow.text = this.fpsText;
			}
			this.timeleft = this.updateInterval;
			this.accum = 0f;
			this.frames = 0;
		}
	}

	public bool showFPS = true;

	public float FPSCap;

	public float updateInterval = 0.5f;

	public GUIText FPSText;

	public GUIText FPSTextShadow;

	private float accum;

	private int frames;

	private float timeleft;

	private Color fpsColor = default(Color);

	private string fpsText = string.Empty;

	private Vector2 fpsPos = new Vector2(0.05f * (float)Screen.width, 0.05f * (float)Screen.height);

	private Vector2 fpsShadowPosOffset = new Vector2(0.0025f * (float)Screen.width, -0.0025f * (float)Screen.height);

	private static bool isCreated;
}

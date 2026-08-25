using System;
using UnityEngine;

public class EncounterOne : MonoBehaviour
{
	private void Start()
	{
		this.mCutscenePlayed = false;
		this.mFrameCount = 0U;
		this.mTweeningController = base.GetComponent<TweeningController>();
		this.mAudioSource = base.GetComponent<AudioSource>();
		this.mPlayer = Puffle.Instance;
		this.mPlayer.GetComponent<Puffle>().DisableInput = true;
		this.mInCutscene = false;
		GameManager.Instance.DuringCutscene = false;
		Cannon[] array = (Cannon[])global::UnityEngine.Object.FindObjectsOfType(typeof(Cannon));
		this.mPuffleContainer = array[22].GetComponent<PuffleContainer>();
		base.transform.Find("Ship").GetComponent<Renderer>().enabled = false;
		base.transform.Find("Ship").Find("Crabby").GetComponent<Renderer>().enabled = false;
		this.mClaw = base.transform.Find("Ship").Find("Claw");
		this.mClaw.GetComponent<Renderer>().enabled = false;
		this.mClawTweeningController = this.mClaw.GetComponent<TweeningController>();
		this.mClawSpriteManager = this.mClaw.GetComponent<SpriteManager>();
		this.mCrabbyAnimController = base.transform.Find("Ship").Find("Crabby").GetComponent<CrabbyAnimController>();
		this.mGiantPuffleO = (Transform)global::UnityEngine.Object.Instantiate(this.fakeGiantPuffleO);
		this.mGiantPuffleO.position = base.transform.position;
		this.mGiantPuffleOTC = this.mGiantPuffleO.GetComponent<TweeningController>();
		this.mGiantPuffleO.localScale *= ScaleItem.Instance.BillboardScale;
		AudioManager.Instance.PlayMusic(AudioManager.MusicTrack.eMusic_Boss);
		base.transform.position += new Vector3(0.51038f, 0.10208f, -0.14f);
	}

	private void Update()
	{
		this.mAudioSource.mute = AudioManager.Instance.Muted;
	}

	private void FixedUpdate()
	{
		if (this.mPlayer.State == Puffle.PuffleState.eInCannon && this.mPuffleContainer.IsPuffleInside() && !this.mCutscenePlayed)
		{
			this.mCutscenePlayed = true;
			this.mInCutscene = true;
			this.mTweeningController.Play(true);
			this.mClawTweeningController.Play(true);
			return;
		}
		if (this.mInCutscene)
		{
			this.mFrameCount += 1U;
			if (this.mFrameCount == 1U)
			{
				GameManager.Instance.DuringCutscene = true;
				GameFlowManager.Instance.GUIManager.HudManager.InGameHud.SetSlowMoButtonEnable(false);
				base.transform.Find("Ship").GetComponent<Renderer>().enabled = true;
				base.transform.Find("Ship").Find("Crabby").GetComponent<Renderer>().enabled = true;
				this.mClaw.GetComponent<Renderer>().enabled = true;
				this.mRestoreSlowMo = TimeManager.Instance.SlowmoOverride;
				GameManager.Instance.StartCutscene(false);
				Camera.main.GetComponentInChildren<VisualEffects>().ForceSlowMoFX();
				this.mAudioSource.PlayOneShot(this.extendClawSound);
			}
			else if (this.mFrameCount == 60U)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
			}
			else if (this.mFrameCount == 70U)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
			}
			else if (this.mFrameCount == 80U)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
			}
			else if (this.mFrameCount == 101U)
			{
				this.mGiantPuffleO.transform.parent = this.mClaw.transform;
				this.mClawSpriteManager.Seek(1);
			}
			else if (this.mFrameCount == 120U)
			{
				this.mAudioSource.PlayOneShot(this.retractClawSound);
			}
			else if (this.mFrameCount == 140U)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
				this.mClaw.GetComponent<Renderer>().enabled = false;
				this.mGiantPuffleOTC.Play(true);
			}
			else if (this.mFrameCount == 147U)
			{
				global::UnityEngine.Object.Destroy(this.mClaw.gameObject);
				global::UnityEngine.Object.Destroy(this.mGiantPuffleO.gameObject);
			}
			else if (this.mFrameCount == 150U)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
			}
			else if (this.mFrameCount == 160U)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
			}
			else if (this.mFrameCount == 170U)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
			}
			else if (this.mFrameCount == 180U)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
			}
			else if (this.mFrameCount == 190U)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
			}
			else if (this.mFrameCount == 200U)
			{
				this.mCrabbyAnimController.Play(CrabbyAnimController.CrabbyAnim.eLaugh);
			}
			else if (this.mFrameCount == 213U)
			{
				GameFlowManager.Instance.GUIManager.HudManager.InGameHud.SetSlowMoButtonEnable(true);
				GameManager.Instance.EndCutscene();
				if (this.mRestoreSlowMo)
				{
					GameManager.Instance.ActivatePlayerSlowMo();
					GameFlowManager.Instance.GUIManager.HudManager.InGameHud.SetSlowmoButtonState(this.mRestoreSlowMo);
				}
				else
				{
					Camera.main.GetComponentInChildren<VisualEffects>().ShowSlowMoFX(false);
				}
				this.mPlayer.GetComponent<Puffle>().DisableInput = false;
				global::UnityEngine.Object.Destroy(base.gameObject);
				GameManager.Instance.DuringCutscene = false;
			}
		}
	}

	public Transform fakeGiantPuffleO;

	public AudioClip extendClawSound;

	public AudioClip retractClawSound;

	private bool mInCutscene;

	private bool mCutscenePlayed;

	private uint mFrameCount;

	private TweeningController mTweeningController;

	private TweeningController mClawTweeningController;

	private SpriteManager mClawSpriteManager;

	private TweeningController mGiantPuffleOTC;

	private PuffleContainer mPuffleContainer;

	private CrabbyAnimController mCrabbyAnimController;

	private Puffle mPlayer;

	private Transform mGiantPuffleO;

	private Transform mClaw;

	private bool mRestoreSlowMo;

	private AudioSource mAudioSource;
}

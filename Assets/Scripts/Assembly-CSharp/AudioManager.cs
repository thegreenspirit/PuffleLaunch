using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource cannonAudioSource;
	public AudioSource puffleOAudioSource;
	public AudioSource obstacleAudioSource;
	public AudioSource musicAudioSource;
	public AudioSource mUISFx;

	public AudioClip pianoHit;
	public AudioClip cactusHit;
	public AudioClip pinkBalloonBump;
	public AudioClip gameplayMusic;
	public AudioClip bossMusic;
	public AudioClip winMusic;
	public AudioClip cinematicMusic;
	public AudioClip menuMusic;

	public float musicVolume;
	private AudioClip mMusicClip;
	private static AudioManager mSingleton;
	private int mMuteRequestCount;
	private int mMuteBackup;

	public enum MusicTrack { eMusic_Gameplay, eMusic_Boss, eMusic_Win, eMusic_Cinematic, eMusic_Menu, eMusic_COUNT }

	public float MusicVolume
	{
		get { return this.musicVolume; }
		set
		{
			this.musicVolume = value;
			//this.musicAudioSource.volume = value;
			this.musicAudioSource.volume = GetNormalizedMusicVolume();
		}
	}

	public bool Muted
	{
		get { return this.mMuteRequestCount > 0; }
	}

	public AudioClip CurrentMusic
	{
		get { return this.mMusicClip; }
	}

	private void Awake()
	{
		if (PlayerPrefs.GetInt("AudioIsMute", 0) == 1) this.Mute();
	}

	public void Start()
	{
		AudioManager.mSingleton = this;

		//this.musicAudioSource.loop = true;
		//this.musicAudioSource.volume = this.musicVolume;

		ConfigureAudioSource(cannonAudioSource, false, 1f);
		ConfigureAudioSource(puffleOAudioSource, false, 1f);
		ConfigureAudioSource(obstacleAudioSource, false, 1f);
		ConfigureAudioSource(mUISFx, false, 1f);
		ConfigureAudioSource(musicAudioSource, true, GetNormalizedMusicVolume());
	}

	public void PlayCannonSound(AudioClip aSound)
	{
		this.cannonAudioSource.PlayOneShot(aSound);
	}

	public void PlayPuffleOSound(AudioClip aSound)
	{
		this.puffleOAudioSource.PlayOneShot(aSound);
	}

	public void PlayObstacleSound(AudioClip aSound)
	{
		this.obstacleAudioSource.loop = false;
		this.obstacleAudioSource.clip = aSound;
		this.obstacleAudioSource.Play();
	}

	public bool IsObstacleSoundPlaying()
	{
		return this.obstacleAudioSource.isPlaying;
	}

	public void PlayMusic(AudioManager.MusicTrack aMusic)
	{
		AudioClip audioClip = null;
		switch (aMusic)
		{
			case AudioManager.MusicTrack.eMusic_Gameplay:
				audioClip = this.gameplayMusic;
				break;
			case AudioManager.MusicTrack.eMusic_Boss:
				audioClip = this.bossMusic;
				break;
			case AudioManager.MusicTrack.eMusic_Win:
				audioClip = this.winMusic;
				break;
			case AudioManager.MusicTrack.eMusic_Cinematic:
				audioClip = this.cinematicMusic;
				break;
			case AudioManager.MusicTrack.eMusic_Menu:
				audioClip = this.menuMusic;
				break;
		}
		this.PlayMusic(audioClip);
	}

	public void PlayMusic(AudioClip aMusic)
	{
		if (this.mMusicClip != aMusic)
		{
			this.mMusicClip = aMusic;
			this.musicAudioSource.clip = this.mMusicClip;
			this.musicAudioSource.Play();
		}
	}

	public void Mute()
	{
		this.mMuteRequestCount++;
		this.mMuteRequestCount = Mathf.Max(this.mMuteRequestCount, 0);

		if (this.mMuteRequestCount > 0)
		{
			PlayerPrefs.SetInt("AudioIsMute", 1);
			this.SetMuteEnabled(true);
		}
	}

	public void Unmute()
	{
		this.mMuteRequestCount--;

		if (this.mMuteRequestCount <= 0)
		{
			PlayerPrefs.SetInt("AudioIsMute", 0);
			this.SetMuteEnabled(false);
			this.mMuteRequestCount = 0;
		}
	}

	public void ForceMute()
	{
		this.mMuteBackup = PlayerPrefs.GetInt("AudioIsMute");
		this.SetMuteEnabled(true);
	}

	public void ResetMute()
	{
		this.SetMuteEnabled(this.mMuteBackup == 1);
	}

	public void PlayUISFx(AudioClip aAudioClip)
	{
		if (aAudioClip != null && this.IsSoundEnabled())
		{
			this.mUISFx.clip = aAudioClip;
			this.mUISFx.time = 0f;
			this.mUISFx.Play();
		}
	}

	public void SetMuteEnabled(bool ab_soundEnabled)
	{
		this.cannonAudioSource.mute = ab_soundEnabled;
		this.puffleOAudioSource.mute = ab_soundEnabled;
		this.obstacleAudioSource.mute = ab_soundEnabled;
		this.musicAudioSource.mute = ab_soundEnabled;
		this.mUISFx.mute = ab_soundEnabled;
	}

	public bool IsSoundEnabled()
	{
		return this.mMuteRequestCount == 0;
	}

	public static AudioManager Instance
	{
		get { return AudioManager.mSingleton; }
	}

	// Green Spirit: Audio fix helpers
	private void ConfigureAudioSource(AudioSource aSource, bool aLoop, float aVolume)
	{
		if (aSource == null) return;

		aSource.playOnAwake = false;
		aSource.loop = aLoop;
		aSource.volume = Mathf.Clamp01(aVolume);
		aSource.spatialBlend = 0f;
		aSource.dopplerLevel = 0f;
		aSource.rolloffMode = AudioRolloffMode.Linear;
		aSource.minDistance = 1f;
		aSource.maxDistance = 500f;
	}

	private float GetNormalizedMusicVolume()
	{
		if (musicVolume > 1f) return Mathf.Clamp01(musicVolume / 10f);
		return Mathf.Clamp01(musicVolume);
	}
}

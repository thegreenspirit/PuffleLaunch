using System;
using System.Collections;
using UnityEngine;

public class SpriteAnimationPump : MonoBehaviour
{
	public bool IsRunning
	{
		get
		{
			return SpriteAnimationPump.pumpIsRunning;
		}
	}

	private void Awake()
	{
		SpriteAnimationPump.instance = this;
		global::UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused && !SpriteAnimationPump.isPaused)
		{
			SpriteAnimationPump.timePaused = Time.realtimeSinceStartup;
		}
		else if (!paused && SpriteAnimationPump.isPaused)
		{
			float num = Time.realtimeSinceStartup - SpriteAnimationPump.timePaused;
			SpriteAnimationPump.startTime += num;
		}
		SpriteAnimationPump.isPaused = paused;
	}

	public void StartAnimationPump()
	{
		if (!SpriteAnimationPump.pumpIsRunning)
		{
			SpriteAnimationPump.pumpIsRunning = true;
			base.StartCoroutine(this.PumpStarter());
		}
	}

	protected IEnumerator PumpStarter()
	{
		while (!SpriteAnimationPump.pumpIsDone)
		{
			yield return null;
		}
		base.StartCoroutine(SpriteAnimationPump.AnimationPump());
		yield break;
	}

	public static void StopAnimationPump()
	{
	}

	protected static IEnumerator AnimationPump()
	{
		SpriteAnimationPump.startTime = Time.realtimeSinceStartup;
		SpriteAnimationPump.pumpIsDone = false;
		while (SpriteAnimationPump.pumpIsRunning)
		{
			if ((!SpriteAnimationPump.isPaused && Time.timeScale == 0f) || (SpriteAnimationPump.isPaused && Time.timeScale != 0f))
			{
				SpriteAnimationPump.instance.OnApplicationPause(Time.timeScale == 0f);
			}
			yield return null;
			SpriteAnimationPump.time = Time.realtimeSinceStartup;
			float elapsed = SpriteAnimationPump.time - SpriteAnimationPump.startTime;
			SpriteAnimationPump.startTime = SpriteAnimationPump.time;
			SpriteAnimationPump.cur = SpriteAnimationPump.head;
			while (SpriteAnimationPump.cur != null)
			{
				ISpriteAnimatable next = SpriteAnimationPump.cur.next;
				SpriteAnimationPump.cur.StepAnim(elapsed);
				SpriteAnimationPump.cur = next;
			}
		}
		SpriteAnimationPump.pumpIsDone = true;
		yield break;
	}

	public static SpriteAnimationPump Instance
	{
		get
		{
			if (SpriteAnimationPump.instance == null)
			{
				GameObject gameObject = new GameObject("SpriteAnimationPump");
				SpriteAnimationPump.instance = (SpriteAnimationPump)gameObject.AddComponent(typeof(SpriteAnimationPump));
			}
			return SpriteAnimationPump.instance;
		}
	}

	public void OnDestroy()
	{
		SpriteAnimationPump.instance = null;
	}

	public static void Add(ISpriteAnimatable s)
	{
		if (SpriteAnimationPump.head != null)
		{
			s.next = SpriteAnimationPump.head;
			SpriteAnimationPump.head.prev = s;
			SpriteAnimationPump.head = s;
		}
		else
		{
			SpriteAnimationPump.head = s;
			SpriteAnimationPump.Instance.StartAnimationPump();
		}
	}

	public static void Remove(ISpriteAnimatable s)
	{
		if (SpriteAnimationPump.head == s)
		{
			SpriteAnimationPump.head = s.next;
			if (SpriteAnimationPump.head == null)
			{
				SpriteAnimationPump.StopAnimationPump();
			}
		}
		else if (s.next != null)
		{
			s.prev.next = s.next;
			s.next.prev = s.prev;
		}
		else if (s.prev != null)
		{
			s.prev.next = null;
		}
		s.next = null;
		s.prev = null;
	}

	private static SpriteAnimationPump instance;

	protected static ISpriteAnimatable head;

	protected static ISpriteAnimatable cur;

	private static float startTime;

	private static float time;

	private static float timePaused;

	private static bool isPaused;

	protected static bool pumpIsRunning;

	protected static bool pumpIsDone = true;

	public static float animationPumpInterval = 0.03333f;
}

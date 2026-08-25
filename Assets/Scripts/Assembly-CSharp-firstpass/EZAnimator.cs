using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EZAnimator : MonoBehaviour
{
	public static EZAnimator instance
	{
		get
		{
			if (EZAnimator.s_Instance == null)
			{
				GameObject gameObject = new GameObject("EZAnimator");
				EZAnimator.s_Instance = (EZAnimator)gameObject.AddComponent(typeof(EZAnimator));
			}
			return EZAnimator.s_Instance;
		}
	}

	public static bool Exists()
	{
		return EZAnimator.s_Instance != null;
	}

	public void OnDestroy()
	{
		EZAnimator.s_Instance = null;
	}

	private void Awake()
	{
		global::UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnLevelWasLoaded(int level)
	{
		if (EZAnimator.animations == null)
		{
			return;
		}
		EZLinkedListIterator<EZAnimation> ezlinkedListIterator = EZAnimator.animations.Begin();
		while (!ezlinkedListIterator.Done)
		{
			EZAnimation ezanimation = ezlinkedListIterator.Current;
			ezanimation._cancel();
			EZAnimator.animations.Remove(ezanimation);
			EZAnimator.ReturnAnimToPool(ezanimation);
			ezlinkedListIterator.Next();
		}
		ezlinkedListIterator.End();
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			EZAnimator.timePaused = Time.realtimeSinceStartup;
		}
		else
		{
			float num = Time.realtimeSinceStartup - EZAnimator.timePaused;
			EZAnimator.startTime += num;
		}
	}

	protected static IEnumerator AnimPump()
	{
		EZLinkedListIterator<EZAnimation> i = EZAnimator.animations.Begin();
		EZAnimator.startTime = Time.realtimeSinceStartup;
		EZAnimator.pumpIsDone = false;
		while (EZAnimator.pumpIsRunning)
		{
			EZAnimator.time = Time.realtimeSinceStartup;
			EZAnimator.elapsed = EZAnimator.time - EZAnimator.startTime;
			EZAnimator.startTime = EZAnimator.time;
			i.Begin(EZAnimator.animations);
			while (!i.Done)
			{
				if (!i.Current.Step(EZAnimator.elapsed))
				{
					EZAnimator.anim = i.Current;
					EZAnimator.animations.Remove(EZAnimator.anim);
					EZAnimator.ReturnAnimToPool(EZAnimator.anim);
				}
				i.NextNoRemove();
			}
			yield return null;
		}
		EZAnimator.pumpIsDone = true;
		yield break;
	}

	public void StartAnimationPump()
	{
		if (!EZAnimator.pumpIsRunning && base.gameObject.active)
		{
			EZAnimator.pumpIsRunning = true;
			base.StartCoroutine(this.PumpStarter());
		}
	}

	protected IEnumerator PumpStarter()
	{
		while (!EZAnimator.pumpIsDone)
		{
			yield return null;
		}
		base.StartCoroutine(EZAnimator.AnimPump());
		yield break;
	}

	public static void StopAnimationPump()
	{
	}

	protected EZAnimation CreateNewAnimation(EZAnimation.ANIM_TYPE type)
	{
		switch (type)
		{
		case EZAnimation.ANIM_TYPE.AnimClip:
			return new RunAnimClip();
		case EZAnimation.ANIM_TYPE.FadeSprite:
			return new FadeSprite();
		case EZAnimation.ANIM_TYPE.FadeMaterial:
			return new FadeMaterial();
		case EZAnimation.ANIM_TYPE.FadeText:
			return new FadeText();
		case EZAnimation.ANIM_TYPE.Translate:
			return new AnimatePosition();
		case EZAnimation.ANIM_TYPE.PunchPosition:
			return new PunchPosition();
		case EZAnimation.ANIM_TYPE.Crash:
			return new Crash();
		case EZAnimation.ANIM_TYPE.SmoothCrash:
			return new SmoothCrash();
		case EZAnimation.ANIM_TYPE.Shake:
			return new Shake();
		case EZAnimation.ANIM_TYPE.Scale:
			return new AnimateScale();
		case EZAnimation.ANIM_TYPE.PunchScale:
			return new PunchScale();
		case EZAnimation.ANIM_TYPE.Rotate:
			return new AnimateRotation();
		case EZAnimation.ANIM_TYPE.PunchRotation:
			return new PunchRotation();
		case EZAnimation.ANIM_TYPE.ShakeRotation:
			return new ShakeRotation();
		case EZAnimation.ANIM_TYPE.CrashRotation:
			return new CrashRotation();
		case EZAnimation.ANIM_TYPE.FadeAudio:
			return new FadeAudio();
		case EZAnimation.ANIM_TYPE.TuneAudio:
			return new TuneAudio();
		case EZAnimation.ANIM_TYPE.TranslateScreen:
			return new AnimateScreenPosition();
		case EZAnimation.ANIM_TYPE.FadeSpriteAlpha:
			return new FadeSpriteAlpha();
		case EZAnimation.ANIM_TYPE.FadeTextAlpha:
			return new FadeTextAlpha();
		default:
			return null;
		}
	}

	public EZAnimation GetAnimation(EZAnimation.ANIM_TYPE type)
	{
		EZLinkedList<EZAnimation> ezlinkedList;
		if (EZAnimator.freeAnimPool.TryGetValue(type, out ezlinkedList) && !ezlinkedList.Empty)
		{
			EZAnimation head = ezlinkedList.Head;
			ezlinkedList.Remove(head);
			return head;
		}
		return this.CreateNewAnimation(type);
	}

	protected static void ReturnAnimToPool(EZAnimation anim)
	{
		anim.Clear();
		EZLinkedList<EZAnimation> ezlinkedList;
		if (!EZAnimator.freeAnimPool.TryGetValue(anim.type, out ezlinkedList))
		{
			ezlinkedList = new EZLinkedList<EZAnimation>();
			EZAnimator.freeAnimPool.Add(anim.type, ezlinkedList);
		}
		ezlinkedList.Add(anim);
	}

	public void AddAnimation(EZAnimation a)
	{
		if (!a.running)
		{
			EZAnimator.animations.Add(a);
			a.running = true;
		}
		this.StartAnimationPump();
	}

	public void AddTransition(EZTransition t)
	{
		if (t.animationTypes == null)
		{
			return;
		}
		for (int i = 0; i < t.animationTypes.Length; i++)
		{
			EZAnimation.ANIM_TYPE anim_TYPE = t.animationTypes[i];
			if (anim_TYPE == EZAnimation.ANIM_TYPE.FadeSprite || anim_TYPE == EZAnimation.ANIM_TYPE.FadeText || anim_TYPE == EZAnimation.ANIM_TYPE.FadeMaterial)
			{
				EZLinkedList<EZLinkedListNode<GameObject>> subSubjects = t.SubSubjects;
				if (subSubjects.Rewind())
				{
					do
					{
						EZAnimation ezanimation = this.GetAnimation(anim_TYPE);
						t.animParams[i].transition = t;
						if (!ezanimation.Start(subSubjects.Current.val, t.animParams[i]))
						{
							EZAnimator.ReturnAnimToPool(ezanimation);
						}
						else if (ezanimation.running)
						{
							EZLinkedListNode<EZAnimation> ezlinkedListNode = t.AddRunningAnim();
							ezlinkedListNode.val = ezanimation;
							ezanimation.Data = ezlinkedListNode;
						}
					}
					while (subSubjects.MoveNext());
				}
			}
			if (!(t.MainSubject == null))
			{
				EZAnimation ezanimation = this.GetAnimation(anim_TYPE);
				t.animParams[i].transition = t;
				if (!ezanimation.Start(t.MainSubject, t.animParams[i]))
				{
					EZAnimator.ReturnAnimToPool(ezanimation);
				}
				else if (ezanimation.running)
				{
					EZLinkedListNode<EZAnimation> ezlinkedListNode = t.AddRunningAnim();
					ezlinkedListNode.val = ezanimation;
					ezanimation.Data = ezlinkedListNode;
				}
			}
		}
	}

	public void StopAnimation(EZAnimation a)
	{
		this.StopAnimation(a, false);
	}

	public void StopAnimation(EZAnimation a, bool end)
	{
		if (!a.running)
		{
			return;
		}
		if (end)
		{
			a._end();
		}
		else
		{
			a._stop();
		}
		EZAnimator.animations.Remove(a);
		EZAnimator.ReturnAnimToPool(a);
		if (EZAnimator.animations.Empty)
		{
			EZAnimator.StopAnimationPump();
		}
	}

	public void Stop(object obj)
	{
		this.Stop(obj, false);
	}

	public void Stop(object obj, bool end)
	{
		EZLinkedListIterator<EZAnimation> ezlinkedListIterator = EZAnimator.animations.Begin();
		while (!ezlinkedListIterator.Done)
		{
			if (ezlinkedListIterator.Current.GetSubject() == obj)
			{
				EZAnimation ezanimation = ezlinkedListIterator.Current;
				if (ezanimation.running)
				{
					if (end)
					{
						ezanimation._end();
					}
					else
					{
						ezanimation._stop();
					}
					EZAnimator.animations.Remove(ezanimation);
					EZAnimator.ReturnAnimToPool(ezanimation);
				}
			}
			ezlinkedListIterator.Next();
		}
		ezlinkedListIterator.End();
	}

	public void Stop(object obj, EZAnimation.ANIM_TYPE type, bool end)
	{
		EZLinkedListIterator<EZAnimation> ezlinkedListIterator = EZAnimator.animations.Begin();
		while (!ezlinkedListIterator.Done)
		{
			if (ezlinkedListIterator.Current.GetSubject() == obj && ezlinkedListIterator.Current.type == type)
			{
				EZAnimation ezanimation = ezlinkedListIterator.Current;
				if (ezanimation.running)
				{
					if (end)
					{
						ezanimation._end();
					}
					else
					{
						ezanimation._stop();
					}
					EZAnimator.animations.Remove(ezanimation);
					EZAnimator.ReturnAnimToPool(ezanimation);
				}
			}
			ezlinkedListIterator.Next();
		}
		ezlinkedListIterator.End();
	}

	public void End(object obj)
	{
		this.Stop(obj, true);
	}

	public void EndAll()
	{
		EZLinkedListIterator<EZAnimation> ezlinkedListIterator = EZAnimator.animations.Begin();
		while (!ezlinkedListIterator.Done)
		{
			ezlinkedListIterator.Current.End();
			ezlinkedListIterator.Next();
		}
		ezlinkedListIterator.End();
	}

	public void StopAll()
	{
		EZLinkedListIterator<EZAnimation> ezlinkedListIterator = EZAnimator.animations.Begin();
		while (!ezlinkedListIterator.Done)
		{
			ezlinkedListIterator.Current.Stop();
			ezlinkedListIterator.Next();
		}
		ezlinkedListIterator.End();
	}

	public void PauseAll()
	{
		EZLinkedListIterator<EZAnimation> ezlinkedListIterator = EZAnimator.animations.Begin();
		while (!ezlinkedListIterator.Done)
		{
			ezlinkedListIterator.Current.Paused = true;
			ezlinkedListIterator.Next();
		}
		ezlinkedListIterator.End();
	}

	public void UnpauseAll()
	{
		EZLinkedListIterator<EZAnimation> ezlinkedListIterator = EZAnimator.animations.Begin();
		while (!ezlinkedListIterator.Done)
		{
			ezlinkedListIterator.Current.Paused = false;
			ezlinkedListIterator.Next();
		}
		ezlinkedListIterator.End();
	}

	public static int GetNumAnimations()
	{
		return EZAnimator.animations.Count;
	}

	private static EZAnimator s_Instance = null;

	protected static Dictionary<EZAnimation.ANIM_TYPE, EZLinkedList<EZAnimation>> freeAnimPool = new Dictionary<EZAnimation.ANIM_TYPE, EZLinkedList<EZAnimation>>();

	protected static EZLinkedList<EZAnimation> animations = new EZLinkedList<EZAnimation>();

	protected static bool pumpIsRunning = false;

	protected static bool pumpIsDone = true;

	protected static float startTime;

	protected static float time;

	protected static float elapsed;

	protected static EZAnimation anim;

	protected static float timePaused;

	private int i;
}

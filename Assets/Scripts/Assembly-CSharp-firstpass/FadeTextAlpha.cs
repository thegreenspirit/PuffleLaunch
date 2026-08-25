using System;
using UnityEngine;

public class FadeTextAlpha : EZAnimation
{
	public FadeTextAlpha()
	{
		this.type = EZAnimation.ANIM_TYPE.FadeTextAlpha;
	}

	public override object GetSubject()
	{
		return this.text;
	}

	public override void _end()
	{
		if (this.text != null)
		{
			this.text.SetColor(this.end);
		}
		base._end();
	}

	protected override void LoopReset()
	{
		if (base.Mode == EZAnimation.ANIM_MODE.By && !this.restartOnRepeat)
		{
			this.start = this.end;
			this.end = this.start + this.delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();
		if (base.Mode == EZAnimation.ANIM_MODE.By)
		{
			this.start = this.text.color;
		}
	}

	protected override void DoAnim()
	{
		if (this.text == null)
		{
			base._stop();
			return;
		}
		this.temp.a = this.interpolator(this.timeElapsed, this.start.a, this.delta.a, this.interval);
		this.text.SetColor(this.temp);
	}

	public static FadeTextAlpha Do(SpriteText txt, EZAnimation.ANIM_MODE mode, Color begin, Color dest, EZAnimation.Interpolator interp, float dur, float delay, EZAnimation.CompletionDelegate startDel, EZAnimation.CompletionDelegate del)
	{
		FadeTextAlpha fadeTextAlpha = (FadeTextAlpha)EZAnimator.instance.GetAnimation(EZAnimation.ANIM_TYPE.FadeTextAlpha);
		fadeTextAlpha.Start(txt, mode, begin, dest, interp, dur, delay, startDel, del);
		return fadeTextAlpha;
	}

	public static FadeTextAlpha Do(SpriteText txt, EZAnimation.ANIM_MODE mode, Color dest, EZAnimation.Interpolator interp, float dur, float delay, EZAnimation.CompletionDelegate startDel, EZAnimation.CompletionDelegate del)
	{
		FadeTextAlpha fadeTextAlpha = (FadeTextAlpha)EZAnimator.instance.GetAnimation(EZAnimation.ANIM_TYPE.FadeTextAlpha);
		fadeTextAlpha.Start(txt, mode, dest, interp, dur, delay, startDel, del);
		return fadeTextAlpha;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
		{
			return false;
		}
		this.text = (SpriteText)sub.GetComponent(typeof(SpriteText));
		if (this.text == null)
		{
			return false;
		}
		this.pingPong = parms.pingPong;
		this.restartOnRepeat = parms.restartOnRepeat;
		this.repeatDelay = parms.repeatDelay;
		if (parms.mode == EZAnimation.ANIM_MODE.FromTo)
		{
			this.Start(this.text, parms.mode, parms.color, parms.color2, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, new EZAnimation.CompletionDelegate(parms.transition.OnAnimEnd));
		}
		else
		{
			this.Start(this.text, parms.mode, this.text.color, parms.color, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, new EZAnimation.CompletionDelegate(parms.transition.OnAnimEnd));
		}
		return true;
	}

	public void Start(SpriteText txt, EZAnimation.ANIM_MODE mode, Color dest, EZAnimation.Interpolator interp, float dur, float delay, EZAnimation.CompletionDelegate startDel, EZAnimation.CompletionDelegate del)
	{
		this.Start(txt, mode, txt.color, dest, interp, dur, delay, startDel, del);
	}

	public void Start(SpriteText txt, EZAnimation.ANIM_MODE mode, Color begin, Color dest, EZAnimation.Interpolator interp, float dur, float delay, EZAnimation.CompletionDelegate startDel, EZAnimation.CompletionDelegate del)
	{
		this.text = txt;
		this.start = begin;
		this.m_mode = mode;
		if (mode == EZAnimation.ANIM_MODE.By)
		{
			this.delta = dest;
		}
		else
		{
			this.delta = new Color(0f, 0f, 0f, dest.a - this.start.a);
		}
		this.end = this.start + this.delta;
		this.temp = this.start;
		this.interpolator = interp;
		this.duration = dur;
		this.m_wait = delay;
		this.completedDelegate = del;
		this.startDelegate = startDel;
		base.StartCommon();
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0f)
		{
			this.text.SetColor(this.start);
		}
		EZAnimator.instance.AddAnimation(this);
	}

	protected Color start;

	protected Color delta;

	protected Color end;

	protected SpriteText text;

	protected Color temp;
}

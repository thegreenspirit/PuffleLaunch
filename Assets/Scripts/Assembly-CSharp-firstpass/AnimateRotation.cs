using System;
using UnityEngine;

public class AnimateRotation : EZAnimation
{
	public AnimateRotation()
	{
		this.type = EZAnimation.ANIM_TYPE.Rotate;
	}

	public override object GetSubject()
	{
		return this.subject;
	}

	public override void _end()
	{
		if (this.subTrans != null)
		{
			this.subTrans.localRotation = this.end;
			this.subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
		}
		base._end();
	}

	protected override void LoopReset()
	{
		if (base.Mode == EZAnimation.ANIM_MODE.By && !this.restartOnRepeat)
		{
			this.start = this.end;
			this.end.x = this.start.x + this.delta.x;
			this.end.y = this.start.y + this.delta.y;
			this.end.z = this.start.z + this.delta.z;
			this.end.w = this.start.w + this.delta.w;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();
		if (base.Mode == EZAnimation.ANIM_MODE.By)
		{
			this.start = this.subTrans.localRotation;
		}
	}

	protected override void DoAnim()
	{
		if (this.subTrans == null)
		{
			base._stop();
			return;
		}
		this.temp = Quaternion.Slerp(this.start, this.end, this.interpolator(this.timeElapsed, 0f, 1f, this.interval));
		this.subTrans.localRotation = this.temp;
		this.subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
	}

	public static AnimateRotation Do(GameObject sub, EZAnimation.ANIM_MODE mode, Vector3 begin, Vector3 dest, EZAnimation.Interpolator interp, float dur, float delay, EZAnimation.CompletionDelegate startDel, EZAnimation.CompletionDelegate del)
	{
		AnimateRotation animateRotation = (AnimateRotation)EZAnimator.instance.GetAnimation(EZAnimation.ANIM_TYPE.Rotate);
		animateRotation.Start(sub, mode, begin, dest, interp, dur, delay, startDel, del);
		return animateRotation;
	}

	public static AnimateRotation Do(GameObject sub, EZAnimation.ANIM_MODE mode, Vector3 dest, EZAnimation.Interpolator interp, float dur, float delay, EZAnimation.CompletionDelegate startDel, EZAnimation.CompletionDelegate del)
	{
		AnimateRotation animateRotation = (AnimateRotation)EZAnimator.instance.GetAnimation(EZAnimation.ANIM_TYPE.Rotate);
		animateRotation.Start(sub, mode, dest, interp, dur, delay, startDel, del);
		return animateRotation;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
		{
			return false;
		}
		this.pingPong = parms.pingPong;
		this.restartOnRepeat = parms.restartOnRepeat;
		this.repeatDelay = parms.repeatDelay;
		if (parms.mode == EZAnimation.ANIM_MODE.FromTo)
		{
			this.Start(sub, parms.mode, parms.vec, parms.vec2, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, new EZAnimation.CompletionDelegate(parms.transition.OnAnimEnd));
		}
		else
		{
			this.Start(sub, parms.mode, sub.transform.localEulerAngles, parms.vec, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, new EZAnimation.CompletionDelegate(parms.transition.OnAnimEnd));
		}
		return true;
	}

	public void Start(GameObject sub, EZAnimation.ANIM_MODE mode, Vector3 dest, EZAnimation.Interpolator interp, float dur, float delay, EZAnimation.CompletionDelegate startDel, EZAnimation.CompletionDelegate del)
	{
		if (sub == null)
		{
			return;
		}
		this.Start(sub, mode, sub.transform.localEulerAngles, dest, interp, dur, delay, startDel, del);
	}

	public void Start(GameObject sub, EZAnimation.ANIM_MODE mode, Vector3 begin, Vector3 dest, EZAnimation.Interpolator interp, float dur, float delay, EZAnimation.CompletionDelegate startDel, EZAnimation.CompletionDelegate del)
	{
		this.subject = sub;
		this.subTrans = this.subject.transform;
		this.start = Quaternion.Euler(begin);
		this.m_mode = mode;
		if (mode == EZAnimation.ANIM_MODE.By)
		{
			Quaternion quaternion = Quaternion.Euler(begin + dest);
			this.delta = new Quaternion(quaternion.x - this.start.x, quaternion.y - this.start.y, quaternion.z - this.start.z, quaternion.w - this.start.w);
		}
		else
		{
			Quaternion quaternion2 = Quaternion.Euler(dest);
			this.delta = new Quaternion(quaternion2.x - this.start.x, quaternion2.y - this.start.y, quaternion2.z - this.start.z, quaternion2.w - this.start.w);
		}
		this.end.x = this.start.x + this.delta.x;
		this.end.y = this.start.y + this.delta.y;
		this.end.z = this.start.z + this.delta.z;
		this.end.w = this.start.w + this.delta.w;
		this.interpolator = interp;
		this.duration = dur;
		this.m_wait = delay;
		this.completedDelegate = del;
		this.startDelegate = startDel;
		base.StartCommon();
		if (mode == EZAnimation.ANIM_MODE.FromTo && delay == 0f)
		{
			this.subTrans.localRotation = this.start;
		}
		EZAnimator.instance.AddAnimation(this);
	}

	public void Start()
	{
		if (this.subject == null)
		{
			return;
		}
		this.direction = 1f;
		this.timeElapsed = 0f;
		this.wait = this.m_wait;
		if (this.m_mode == EZAnimation.ANIM_MODE.By)
		{
			this.start = this.subject.transform.localRotation;
			this.end.x = this.start.x + this.delta.x;
			this.end.y = this.start.y + this.delta.y;
			this.end.z = this.start.z + this.delta.z;
			this.end.w = this.start.w + this.delta.w;
		}
		EZAnimator.instance.AddAnimation(this);
	}

	protected GameObject subject;

	protected Transform subTrans;

	protected Quaternion temp;

	protected Quaternion delta;

	protected Quaternion start;

	protected Quaternion end;
}

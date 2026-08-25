using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Text Field")]
public class UITextField : AutoSpriteControlBase, IKeyFocusable
{
	public override bool controlIsEnabled
	{
		get
		{
			return this.m_controlIsEnabled;
		}
		set
		{
			this.m_controlIsEnabled = value;
		}
	}

	public override TextureAnim[] States
	{
		get
		{
			return this.states;
		}
		set
		{
			this.states = value;
		}
	}

	public override EZTransitionList GetTransitions(int index)
	{
		if (index >= this.transitions.Length)
		{
			return null;
		}
		return this.transitions[index];
	}

	public override EZTransitionList[] Transitions
	{
		get
		{
			return this.transitions;
		}
		set
		{
			this.transitions = value;
		}
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (this.deleted)
		{
			return;
		}
		if (!this.m_controlIsEnabled || base.IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}
		if (this.inputDelegate != null)
		{
			this.inputDelegate(ref ptr);
		}
		if (!this.m_controlIsEnabled || base.IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}
		if (ptr.evt == this.customFocusEvent && this.focusDelegate != null)
		{
			this.focusDelegate(this);
		}
		base.OnInput(ref ptr);
	}

	public override void Copy(SpriteRoot s)
	{
		this.Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (!(s is UITextField))
		{
			return;
		}
		UITextField uitextField = (UITextField)s;
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			this.maxLength = uitextField.maxLength;
			this.multiline = uitextField.multiline;
			this.password = uitextField.password;
			this.maskingCharacter = uitextField.maskingCharacter;
			this.customKeyboard = uitextField.customKeyboard;
			this.customFocusEvent = uitextField.customFocusEvent;
			this.margins = uitextField.margins;
			this.type = uitextField.type;
			this.autoCorrect = uitextField.autoCorrect;
			this.alert = uitextField.alert;
			this.hideInput = uitextField.hideInput;
			this.typingSoundEffect = uitextField.typingSoundEffect;
			this.fieldFullSound = uitextField.fieldFullSound;
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			this.scriptWithMethodToInvoke = uitextField.scriptWithMethodToInvoke;
			this.methodToInvoke = uitextField.methodToInvoke;
		}
		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			this.caret.Copy(uitextField.caret);
			this.caretSize = uitextField.caretSize;
			this.caretOffset = uitextField.caretOffset;
			this.caretAnchor = uitextField.caretAnchor;
			this.showCaretOnMobile = uitextField.showCaretOnMobile;
		}
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			this.insert = uitextField.insert;
			this.Text = uitextField.Text;
		}
		this.SetMargins(this.margins);
	}

	public override bool GotFocus()
	{
		if (this.customKeyboard)
		{
			return false;
		}
		this.hasFocus = this.m_controlIsEnabled;
		return this.m_controlIsEnabled;
	}

	public string GetInputText(ref KEYBOARD_INFO info)
	{
		info.insert = this.insert;
		info.type = this.type;
		info.autoCorrect = this.autoCorrect;
		info.multiline = this.multiline;
		info.secure = this.password;
		info.alert = this.alert;
		info.hideInput = this.hideInput;
		this.ShowCaret();
		return this.text;
	}

	public string SetInputText(string inputText, ref int insertPt)
	{
		if (!this.multiline)
		{
			int num;
			if ((num = inputText.IndexOf('\n')) != -1)
			{
				inputText = inputText.Remove(num, 1);
				UIManager.instance.FocusObject = null;
			}
			if ((num = inputText.IndexOf('\r')) != -1)
			{
				inputText = inputText.Remove(num, 1);
				UIManager.instance.FocusObject = null;
			}
		}
		if (this.validationDelegate != null)
		{
			inputText = this.validationDelegate(this, inputText);
		}
		if (inputText.Length > this.maxLength && this.maxLength > 0)
		{
			EZValueChangedDelegate changeDelegate = this.changeDelegate;
			this.changeDelegate = null;
			this.Text = inputText.Substring(0, this.maxLength);
			this.insert = Mathf.Clamp(insertPt, 0, this.maxLength);
			this.maxLengthExceeded = true;
			this.changeDelegate = changeDelegate;
			if (this.changeDelegate != null)
			{
				this.changeDelegate(this);
			}
			if (this.fieldFullSound != null)
			{
				this.fieldFullSound.PlayOneShot(this.fieldFullSound.clip);
			}
		}
		else
		{
			this.Text = inputText;
			this.insert = insertPt;
			if (this.typingSoundEffect != null)
			{
				this.typingSoundEffect.PlayOneShot(this.typingSoundEffect.clip);
			}
			if (this.changeDelegate != null)
			{
				this.changeDelegate(this);
			}
		}
		if (this.caret != null && this.caret.IsHidden() && this.hasFocus)
		{
			this.caret.Hide(false);
		}
		this.PositionCaret();
		if (UIManager.instance.FocusObject == null && !this.commitOnLostFocus)
		{
			this.Commit();
		}
		return this.text;
	}

	public void LostFocus()
	{
		if (this.commitOnLostFocus)
		{
			this.Commit();
		}
		this.hasFocus = false;
		this.HideCaret();
	}

	public void Commit()
	{
		if (this.scriptWithMethodToInvoke != null && !string.IsNullOrEmpty(this.methodToInvoke))
		{
			this.scriptWithMethodToInvoke.Invoke(this.methodToInvoke, 0f);
		}
		if (this.commitDelegate != null)
		{
			this.commitDelegate(this);
		}
	}

	public string Content
	{
		get
		{
			return this.Text;
		}
	}

	protected void ShowCaret()
	{
		if (this.caret == null)
		{
			return;
		}
		this.CalcClippingRect();
		this.caret.Hide(false);
		this.PositionCaret();
		if (!this.caret.IsHidden())
		{
			this.transitions[1].list[0].Start();
			if (this.caret.animations.Length > 0)
			{
				this.caret.DoAnim(0);
			}
		}
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);
		if (this.caret != null)
		{
			if (!tf && this.hasFocus)
			{
				this.caret.Hide(tf);
			}
			else
			{
				this.caret.Hide(true);
			}
		}
		if (!tf)
		{
			this.CalcClippingRect();
		}
	}

	protected void HideCaret()
	{
		if (this.caret == null)
		{
			return;
		}
		this.transitions[1].list[0].StopSafe();
		this.caret.Hide(true);
	}

	protected void PositionText(bool recur)
	{
		Vector3 vector = base.transform.InverseTransformPoint(this.spriteText.GetInsertionPointPos(this.spriteText.PlainIndexToDisplayIndex(this.insert)));
		Vector3 vector2 = vector + Vector3.up * this.spriteText.BaseHeight * this.spriteText.transform.localScale.y;
		if (recur)
		{
			if (this.multiline)
			{
				if (vector2.y > this.marginTopLeft.y)
				{
					this.spriteText.transform.localPosition -= Vector3.up * this.spriteText.LineSpan;
					this.PositionText(false);
					this.spriteText.ClippingRect = this.clientClippingRect;
					return;
				}
				if (vector.y < this.marginBottomRight.y)
				{
					this.spriteText.transform.localPosition += Vector3.up * this.spriteText.LineSpan;
					this.PositionText(false);
					this.spriteText.ClippingRect = this.clientClippingRect;
					return;
				}
			}
			else
			{
				if (vector.x < this.marginTopLeft.x)
				{
					Vector3 centerPoint = base.GetCenterPoint();
					Vector3 vector3 = this.spriteText.transform.localPosition + Vector3.right * Mathf.Abs(centerPoint.x - vector.x);
					vector3.x = Mathf.Min(vector3.x, this.origTextPos.x);
					this.spriteText.transform.localPosition = vector3;
					this.PositionText(false);
					this.spriteText.ClippingRect = this.clientClippingRect;
					return;
				}
				if (vector.x > this.marginBottomRight.x)
				{
					Vector3 centerPoint2 = base.GetCenterPoint();
					Vector3 vector4 = this.spriteText.transform.localPosition - Vector3.right * Mathf.Abs(centerPoint2.x - vector.x);
					this.spriteText.transform.localPosition = vector4;
					this.PositionText(false);
					this.spriteText.ClippingRect = this.clientClippingRect;
					return;
				}
			}
		}
	}

	protected void PositionCaret()
	{
		this.PositionCaret(true);
	}

	protected void PositionCaret(bool recur)
	{
		if (this.spriteText == null)
		{
			return;
		}
		if (this.caret == null)
		{
			this.PositionText(true);
			return;
		}
		Vector3 vector = base.transform.InverseTransformPoint(this.spriteText.GetInsertionPointPos(this.spriteText.PlainIndexToDisplayIndex(this.insert)));
		Vector3 vector2 = vector + Vector3.up * this.spriteText.BaseHeight * this.spriteText.transform.localScale.y;
		if (recur)
		{
			if (this.multiline)
			{
				if (vector2.y > this.marginTopLeft.y)
				{
					this.spriteText.transform.localPosition -= Vector3.up * this.spriteText.LineSpan;
					this.PositionCaret(false);
					this.spriteText.ClippingRect = this.clientClippingRect;
					return;
				}
				if (vector.y < this.marginBottomRight.y)
				{
					this.spriteText.transform.localPosition += Vector3.up * this.spriteText.LineSpan;
					this.PositionCaret(false);
					this.spriteText.ClippingRect = this.clientClippingRect;
					return;
				}
			}
			else
			{
				if (vector.x < this.marginTopLeft.x)
				{
					Vector3 centerPoint = base.GetCenterPoint();
					Vector3 vector3 = this.spriteText.transform.localPosition + Vector3.right * Mathf.Abs(centerPoint.x - vector.x);
					vector3.x = Mathf.Min(vector3.x, this.origTextPos.x);
					this.spriteText.transform.localPosition = vector3;
					this.PositionCaret(false);
					this.spriteText.ClippingRect = this.clientClippingRect;
					return;
				}
				if (vector.x > this.marginBottomRight.x)
				{
					Vector3 centerPoint2 = base.GetCenterPoint();
					Vector3 vector4 = this.spriteText.transform.localPosition - Vector3.right * Mathf.Abs(centerPoint2.x - vector.x);
					this.spriteText.transform.localPosition = vector4;
					this.PositionCaret(false);
					this.spriteText.ClippingRect = this.clientClippingRect;
					return;
				}
			}
		}
		this.transitions[1].list[0].StopSafe();
		this.caret.transform.localPosition = vector;
		this.transitions[1].list[0].Start();
		if (this.caret.animations.Length > 0)
		{
			this.caret.DoAnim(0);
		}
		this.caret.ClippingRect = this.clientClippingRect;
	}

	protected void PositionInsertionPoint(Vector3 pt)
	{
		if (this.caret == null || this.spriteText == null)
		{
			return;
		}
		this.insert = this.spriteText.DisplayIndexToPlainIndex(this.spriteText.GetNearestInsertionPoint(pt));
		UIManager.instance.InsertionPoint = this.insert;
		this.PositionCaret(true);
	}

	public void GoUp()
	{
		Vector3 vector = this.spriteText.GetInsertionPointPos(this.spriteText.PlainIndexToDisplayIndex(this.insert));
		vector += this.spriteText.transform.up * this.spriteText.LineSpan * this.spriteText.transform.lossyScale.y;
		this.insert = this.spriteText.DisplayIndexToPlainIndex(this.spriteText.GetNearestInsertionPoint(vector));
		UIManager.instance.InsertionPoint = this.insert;
		this.PositionCaret(true);
	}

	public void GoDown()
	{
		Vector3 vector = this.spriteText.GetInsertionPointPos(this.spriteText.PlainIndexToDisplayIndex(this.insert));
		vector -= this.spriteText.transform.up * this.spriteText.LineSpan * this.spriteText.transform.lossyScale.y;
		this.insert = this.spriteText.DisplayIndexToPlainIndex(this.spriteText.GetNearestInsertionPoint(vector));
		UIManager.instance.InsertionPoint = this.insert;
		this.PositionCaret(true);
	}

	public void SetCommitDelegate(EZKeyboardCommitDelegate del)
	{
		this.commitDelegate = del;
	}

	public void AddCommitDelegate(EZKeyboardCommitDelegate del)
	{
		this.commitDelegate = (EZKeyboardCommitDelegate)Delegate.Combine(this.commitDelegate, del);
	}

	public void RemoveCommitDelegate(EZKeyboardCommitDelegate del)
	{
		this.commitDelegate = (EZKeyboardCommitDelegate)Delegate.Remove(this.commitDelegate, del);
	}

	public void SetFocusDelegate(UITextField.FocusDelegate del)
	{
		this.focusDelegate = del;
	}

	public void AddFocusDelegate(UITextField.FocusDelegate del)
	{
		this.focusDelegate = (UITextField.FocusDelegate)Delegate.Combine(this.focusDelegate, del);
	}

	public void RemoveFocusDelegate(UITextField.FocusDelegate del)
	{
		this.focusDelegate = (UITextField.FocusDelegate)Delegate.Remove(this.focusDelegate, del);
	}

	public void SetValidationDelegate(UITextField.ValidationDelegate del)
	{
		this.validationDelegate = del;
	}

	public void AddValidationDelegate(UITextField.ValidationDelegate del)
	{
		this.validationDelegate = (UITextField.ValidationDelegate)Delegate.Combine(this.validationDelegate, del);
	}

	public void RemoveValidationDelegate(UITextField.ValidationDelegate del)
	{
		this.validationDelegate = (UITextField.ValidationDelegate)Delegate.Remove(this.validationDelegate, del);
	}

	protected override void Awake()
	{
		base.Awake();
		this.defaultTextAlignment = SpriteText.Alignment_Type.Left;
		this.defaultTextAnchor = SpriteText.Anchor_Pos.Upper_Left;
	}

	public override void Start()
	{
		if (this.m_started)
		{
			return;
		}
		base.Start();
		if (this.spriteText == null)
		{
			this.Text = " ";
			this.Text = string.Empty;
		}
		if (this.spriteText != null)
		{
			this.spriteText.password = this.password;
			this.spriteText.maskingCharacter = this.maskingCharacter;
			this.spriteText.multiline = this.multiline;
			this.origTextPos = this.spriteText.transform.localPosition;
			this.SetMargins(this.margins);
		}
		this.insert = this.Text.Length;
		if (Application.isPlaying)
		{
			if (base.GetComponent<Collider>() == null)
			{
				this.AddCollider();
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				if (this.showCaretOnMobile)
				{
					this.CreateCaret();
				}
			}
			else
			{
				this.CreateCaret();
			}
		}
		this.cachedPos = base.transform.position;
		this.cachedRot = base.transform.rotation;
		this.cachedScale = base.transform.lossyScale;
		this.CalcClippingRect();
		if (this.managed && this.m_hidden)
		{
			this.Hide(true);
		}
	}

	protected void CreateCaret()
	{
		this.caret = (AutoSprite)new GameObject
		{
			name = base.name + " - caret",
			transform = 
			{
				parent = base.transform,
				localPosition = Vector3.zero,
				localRotation = Quaternion.identity,
				localScale = Vector3.one
			},
			layer = base.gameObject.layer
		}.AddComponent(typeof(AutoSprite));
		this.caret.plane = this.plane;
		this.caret.offset = this.caretOffset;
		this.caret.SetAnchor(this.caretAnchor);
		this.caret.persistent = this.persistent;
		if (!this.managed)
		{
			if (this.caret.spriteMesh != null)
			{
				((SpriteMesh)this.caret.spriteMesh).material = base.GetComponent<Renderer>().sharedMaterial;
			}
		}
		else if (this.manager != null)
		{
			this.caret.Managed = this.managed;
			this.manager.AddSprite(this.caret);
			this.caret.SetDrawLayer(this.drawLayer + 1);
		}
		else
		{
			Debug.LogError("Sprite on object \"" + base.name + "\" not assigned to a SpriteManager!");
		}
		this.caret.autoResize = this.autoResize;
		if (this.pixelPerfect)
		{
			this.caret.pixelPerfect = this.pixelPerfect;
		}
		else
		{
			this.caret.SetSize(this.caretSize.x, this.caretSize.y);
		}
		if (this.states[1].spriteFrames.Length != 0)
		{
			this.caret.animations = new UVAnimation[1];
			this.caret.animations[0] = new UVAnimation();
			this.caret.animations[0].SetAnim(this.states[1], 0);
			this.caret.PlayAnim(0, 0);
		}
		this.caret.renderCamera = this.renderCamera;
		this.caret.SetCamera(this.renderCamera);
		this.caret.Hide(true);
		this.transitions[1].list[0].MainSubject = this.caret.gameObject;
		this.PositionCaret();
		if (this.container != null)
		{
			this.container.AddSubject(this.caret.gameObject);
		}
		if (this.autoResize)
		{
			this.caret.Start();
			this.caret.SetSize(this.caretSize.x, this.caretSize.y);
		}
	}

	public void CalcClippingRect()
	{
		if (this.spriteText == null)
		{
			return;
		}
		Vector3 vector = this.marginTopLeft;
		Vector3 vector2 = this.marginBottomRight;
		if (this.clipped)
		{
			Vector3 vector3 = vector;
			Vector3 vector4 = vector2;
			vector.x = Mathf.Clamp(this.localClipRect.x, vector3.x, vector4.x);
			vector2.x = Mathf.Clamp(this.localClipRect.xMax, vector3.x, vector4.x);
			vector.y = Mathf.Clamp(this.localClipRect.yMax, vector4.y, vector3.y);
			vector2.y = Mathf.Clamp(this.localClipRect.y, vector4.y, vector3.y);
		}
		this.clientClippingRect.FromRect(Rect.MinMaxRect(vector.x, vector2.y, vector2.x, vector.y));
		this.clientClippingRect.MultFast(base.transform.localToWorldMatrix);
		this.spriteText.ClippingRect = this.clientClippingRect;
		if (this.caret != null)
		{
			this.caret.ClippingRect = this.clientClippingRect;
		}
	}

	public void OnEZTranslated()
	{
		this.CalcClippingRect();
	}

	public void OnEZRotated()
	{
		this.CalcClippingRect();
	}

	public void OnEZScaled()
	{
		this.CalcClippingRect();
	}

	public void SetMargins(Vector2 marg)
	{
		this.margins = marg;
		Vector3 centerPoint = base.GetCenterPoint();
		this.marginTopLeft = new Vector3(centerPoint.x + this.margins.x - this.width * 0.5f, centerPoint.y - this.margins.y + this.height * 0.5f);
		this.marginBottomRight = new Vector3(centerPoint.x - this.margins.x + this.width * 0.5f, centerPoint.y + this.margins.y - this.height * 0.5f);
		if (this.multiline)
		{
			float num = 0f;
			switch (this.spriteText.anchor)
			{
			case SpriteText.Anchor_Pos.Upper_Left:
			case SpriteText.Anchor_Pos.Middle_Left:
			case SpriteText.Anchor_Pos.Lower_Left:
				num = this.marginBottomRight.x - this.origTextPos.x;
				break;
			case SpriteText.Anchor_Pos.Upper_Center:
			case SpriteText.Anchor_Pos.Middle_Center:
			case SpriteText.Anchor_Pos.Lower_Center:
				num = (this.marginBottomRight.x - this.marginTopLeft.x) * 2f - 2f * Mathf.Abs(this.origTextPos.x);
				break;
			case SpriteText.Anchor_Pos.Upper_Right:
			case SpriteText.Anchor_Pos.Middle_Right:
			case SpriteText.Anchor_Pos.Lower_Right:
				num = this.origTextPos.x - this.marginTopLeft.x;
				break;
			}
			this.spriteText.maxWidth = 1f / this.spriteText.transform.localScale.x * num;
		}
		else
		{
			this.spriteText.maxWidth = 0f;
		}
	}

	public override void InitUVs()
	{
		if (this.states[0].spriteFrames.Length != 0)
		{
			this.frameInfo.Copy(this.states[0].spriteFrames[0]);
		}
		base.InitUVs();
	}

	public bool MaxLengthExceeded
	{
		get
		{
			return this.maxLengthExceeded;
		}
	}

	public override IUIContainer Container
	{
		get
		{
			return base.Container;
		}
		set
		{
			if (value != this.container)
			{
				if (this.container != null && this.caret != null)
				{
					this.container.RemoveChild(this.caret.gameObject);
				}
				if (value != null && this.caret != null)
				{
					value.AddChild(this.caret.gameObject);
				}
			}
			base.Container = value;
		}
	}

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			bool flag = this.spriteText == null;
			if (Application.isPlaying && !this.m_started)
			{
				this.Start();
			}
			bool flag2 = this.insert == this.text.Length;
			base.Text = value;
			if (this.maxLength > 0)
			{
				if (value.Length > this.maxLength)
				{
					this.maxLengthExceeded = true;
				}
				else
				{
					this.maxLengthExceeded = false;
				}
			}
			if (flag && this.spriteText != null)
			{
				this.spriteText.transform.localPosition = new Vector4(this.width * -0.5f + this.margins.x, this.height * 0.5f + this.margins.y);
				this.spriteText.removeUnsupportedCharacters = true;
				this.spriteText.parseColorTags = false;
				this.spriteText.multiline = this.multiline;
			}
			if (this.cachedPos != base.transform.position || this.cachedRot != base.transform.rotation || this.cachedScale != base.transform.lossyScale)
			{
				this.cachedPos = base.transform.position;
				this.cachedRot = base.transform.rotation;
				this.cachedScale = base.transform.lossyScale;
				this.CalcClippingRect();
			}
			if (flag2)
			{
				this.insert = this.Text.Length;
			}
			this.PositionCaret();
			if (this.changeDelegate != null)
			{
				this.changeDelegate(this);
			}
		}
	}

	public static UITextField Create(string name, Vector3 pos)
	{
		return (UITextField)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UITextField));
	}

	public static UITextField Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UITextField)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UITextField));
	}

	public override void Unclip()
	{
		if (this.ignoreClipping)
		{
			return;
		}
		base.Unclip();
		this.CalcClippingRect();
	}

	public override Rect3D ClippingRect
	{
		get
		{
			return base.ClippingRect;
		}
		set
		{
			if (this.ignoreClipping)
			{
				return;
			}
			base.ClippingRect = value;
			this.CalcClippingRect();
		}
	}

	public override bool Clipped
	{
		get
		{
			return base.Clipped;
		}
		set
		{
			if (this.ignoreClipping)
			{
				return;
			}
			base.Clipped = value;
			this.CalcClippingRect();
		}
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		this.scriptWithMethodToInvoke = gui.DrawScriptSelection(this.scriptWithMethodToInvoke, ref this.methodToInvoke);
	}

	public override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = new Color(1f, 0f, 0.5f, 1f);
		Gizmos.DrawLine(this.clientClippingRect.topLeft, this.clientClippingRect.bottomLeft);
		Gizmos.DrawLine(this.clientClippingRect.bottomLeft, this.clientClippingRect.bottomRight);
		Gizmos.DrawLine(this.clientClippingRect.bottomRight, this.clientClippingRect.topRight);
		Gizmos.DrawLine(this.clientClippingRect.topRight, this.clientClippingRect.topLeft);
	}

	public override void DoMirror()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (this.screenSize.x == 0f || this.screenSize.y == 0f)
		{
			this.Start();
		}
		if (this.mirror == null)
		{
			this.mirror = new UITextFieldMirror();
			this.mirror.Mirror(this);
		}
		this.mirror.Validate(this);
		if (this.mirror.DidChange(this))
		{
			this.Init();
			this.mirror.Mirror(this);
		}
	}

	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
	{
		new TextureAnim("Field graphic"),
		new TextureAnim("Caret")
	};

	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
	{
		null,
		new EZTransitionList(new EZTransition[]
		{
			new EZTransition("Caret Flash")
		})
	};

	public Vector2 margins;

	protected Rect3D clientClippingRect;

	protected Vector2 marginTopLeft;

	protected Vector2 marginBottomRight;

	public int maxLength;

	public bool multiline;

	public bool password;

	public string maskingCharacter = "*";

	public Vector2 caretSize;

	public SpriteRoot.ANCHOR_METHOD caretAnchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT;

	public Vector3 caretOffset = new Vector3(0f, 0f, -0.1f);

	public bool showCaretOnMobile;

	public bool allowClickCaretPlacement = true;

	protected bool maxLengthExceeded;

	public TouchScreenKeyboardType type;

	public bool autoCorrect;

	public bool alert;

	public bool hideInput;

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke = string.Empty;

	protected EZKeyboardCommitDelegate commitDelegate;

	protected UITextField.ValidationDelegate validationDelegate;

	public AudioSource typingSoundEffect;

	public AudioSource fieldFullSound;

	public bool customKeyboard;

	public bool commitOnLostFocus;

	public POINTER_INFO.INPUT_EVENT customFocusEvent = POINTER_INFO.INPUT_EVENT.PRESS;

	protected AutoSprite caret;

	protected UITextField.FocusDelegate focusDelegate;

	protected int insert;

	protected Vector3 cachedPos;

	protected Quaternion cachedRot;

	protected Vector3 cachedScale;

	protected bool hasFocus;

	protected Vector3 origTextPos;

	protected int[,] stateIndices;

	public delegate void FocusDelegate(UITextField field);

	public delegate string ValidationDelegate(UITextField field, string text);
}

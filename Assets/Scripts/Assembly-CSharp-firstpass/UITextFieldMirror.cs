using System;
using UnityEngine;

public class UITextFieldMirror : AutoSpriteControlBaseMirror
{
	public override void Mirror(SpriteRoot s)
	{
		base.Mirror(s);
		UITextField uitextField = (UITextField)s;
		this.margins = uitextField.margins;
		this.multiline = uitextField.multiline;
	}

	public override bool Validate(SpriteRoot s)
	{
		return base.Validate(s);
	}

	public override bool DidChange(SpriteRoot s)
	{
		UITextField uitextField = (UITextField)s;
		if (this.margins.x != uitextField.margins.x || this.margins.y != uitextField.margins.y || this.width != uitextField.width || this.height != uitextField.height)
		{
			uitextField.SetMargins(uitextField.margins);
			uitextField.CalcClippingRect();
			this.margins = uitextField.margins;
		}
		if (this.multiline != uitextField.multiline)
		{
			if (uitextField.spriteText != null)
			{
				uitextField.spriteText.multiline = uitextField.multiline;
			}
			return true;
		}
		return base.DidChange(s);
	}

	public Vector2 margins;

	public bool multiline;
}

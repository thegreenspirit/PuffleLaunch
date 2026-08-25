using System;

public class AutoSpriteControlBaseMirror : SpriteRootMirror
{
	public override void Mirror(SpriteRoot s)
	{
		AutoSpriteControlBase autoSpriteControlBase = (AutoSpriteControlBase)s;
		base.Mirror(s);
		this.text = autoSpriteControlBase.text;
		this.textOffsetZ = autoSpriteControlBase.textOffsetZ;
	}

	public override bool DidChange(SpriteRoot s)
	{
		AutoSpriteControlBase autoSpriteControlBase = (AutoSpriteControlBase)s;
		if (this.text != autoSpriteControlBase.text)
		{
			autoSpriteControlBase.Text = autoSpriteControlBase.text;
			return true;
		}
		if (this.textOffsetZ != autoSpriteControlBase.textOffsetZ)
		{
			if (autoSpriteControlBase.spriteText != null)
			{
				autoSpriteControlBase.spriteText.offsetZ = this.textOffsetZ;
			}
			return true;
		}
		return base.DidChange(s);
	}

	private string text;

	private float textOffsetZ;
}

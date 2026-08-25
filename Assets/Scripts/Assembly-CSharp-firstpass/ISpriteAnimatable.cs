using System;

public interface ISpriteAnimatable
{
	bool StepAnim(float time);

	ISpriteAnimatable prev { get; set; }

	ISpriteAnimatable next { get; set; }
}

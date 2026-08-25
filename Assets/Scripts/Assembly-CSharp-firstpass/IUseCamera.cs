using System;
using UnityEngine;

public interface IUseCamera
{
	void SetCamera();

	void SetCamera(Camera c);

	Camera RenderCamera { get; set; }
}

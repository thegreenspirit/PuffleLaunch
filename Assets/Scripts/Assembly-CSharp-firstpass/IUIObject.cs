using System;
using UnityEngine;

public interface IUIObject : IEZDragDrop
{
	bool controlIsEnabled { get; set; }

	bool DetargetOnDisable { get; set; }

	IUIObject GetControl(ref POINTER_INFO ptr);

	IUIContainer Container { get; set; }

	GameObject gameObject { get; }

	Transform transform { get; }

	string ToString();

	string name { get; }

	bool RequestContainership(IUIContainer cont);

	bool GotFocus();

	void OnInput(POINTER_INFO ptr);

	void SetInputDelegate(EZInputDelegate del);

	void AddInputDelegate(EZInputDelegate del);

	void RemoveInputDelegate(EZInputDelegate del);

	void SetValueChangedDelegate(EZValueChangedDelegate del);

	void AddValueChangedDelegate(EZValueChangedDelegate del);

	void RemoveValueChangedDelegate(EZValueChangedDelegate del);
}

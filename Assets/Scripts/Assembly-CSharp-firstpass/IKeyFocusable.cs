using System;

public interface IKeyFocusable
{
	void LostFocus();

	string SetInputText(string inputText, ref int insertPt);

	string GetInputText(ref KEYBOARD_INFO info);

	void Commit();

	void SetCommitDelegate(EZKeyboardCommitDelegate del);

	string Content { get; }

	void GoUp();

	void GoDown();
}

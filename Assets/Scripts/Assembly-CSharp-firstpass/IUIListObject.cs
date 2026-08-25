using System;
using UnityEngine;

public interface IUIListObject : IEZDragDrop, IUIObject
{
	bool IsContainer();

	void FindOuterEdges();

	Vector2 TopLeftEdge { get; }

	Vector2 BottomRightEdge { get; }

	void Hide(bool tf);

	bool Managed { get; }

	Rect3D ClippingRect { get; set; }

	bool Clipped { get; set; }

	void Unclip();

	void UpdateCollider();

	void SetList(UIScrollList c);

	UIScrollList GetScrollList();

	int Index { get; set; }

	string Text { get; set; }

	SpriteText TextObj { get; }

	bool selected { get; set; }

	void Delete();

	Camera RenderCamera { get; set; }
}

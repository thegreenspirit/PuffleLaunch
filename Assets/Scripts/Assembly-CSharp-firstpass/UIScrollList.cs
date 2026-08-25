using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Scroll List")]
public class UIScrollList : MonoBehaviour, IEZDragDrop, IUIObject
{
	protected void Awake()
	{
		if (this.m_awake)
		{
			return;
		}
		this.m_awake = true;
		this.mover = new GameObject();
		this.mover.name = "Mover";
		this.mover.transform.parent = base.transform;
		this.mover.transform.localPosition = Vector3.zero;
		this.mover.transform.localRotation = Quaternion.identity;
		this.mover.transform.localScale = Vector3.one;
		if (this.direction == UIScrollList.DIRECTION.BtoT_RtoL)
		{
			this.scrollPos = 1f;
		}
		this.autoScrollInterpolator = EZAnimation.GetInterpolator(this.snapEasing);
		this.lowPassFilterFactor = this.inertiaLerpInterval / 0.045f;
	}

	protected void Start()
	{
		if (this.m_started)
		{
			return;
		}
		this.m_started = true;
		this.SetupCameraAndSizes();
		this.lastTime = Time.realtimeSinceStartup;
		this.cachedPos = base.transform.position;
		this.cachedRot = base.transform.rotation;
		this.cachedScale = base.transform.lossyScale;
		this.CalcClippingRect();
		if (this.slider != null)
		{
			this.slider.AddValueChangedDelegate(new EZValueChangedDelegate(this.SliderMoved));
			this.slider.AddInputDelegate(new EZInputDelegate(this.SliderInputDel));
		}
		if (base.GetComponent<Collider>() == null && this.touchScroll)
		{
			BoxCollider boxCollider = (BoxCollider)base.gameObject.AddComponent(typeof(BoxCollider));
			boxCollider.size = new Vector3(this.viewableAreaActual.x, this.viewableAreaActual.y, 0.001f);
			boxCollider.center = Vector3.forward * 0.01f;
			boxCollider.isTrigger = true;
		}
		for (int i = 0; i < this.sceneItems.Length; i++)
		{
			if (this.sceneItems[i] != null)
			{
				this.AddItem(this.sceneItems[i]);
			}
		}
		for (int j = 0; j < this.prefabItems.Length; j++)
		{
			if (this.prefabItems[j] != null)
			{
				if (this.prefabItems[j].item == null)
				{
					if (this.prefabItems[0].item != null)
					{
						this.CreateItem(this.prefabItems[0].item, (!(this.prefabItems[j].itemText == string.Empty)) ? this.prefabItems[j].itemText : null);
					}
				}
				else
				{
					this.CreateItem(this.prefabItems[j].item, (!(this.prefabItems[j].itemText == string.Empty)) ? this.prefabItems[j].itemText : null);
				}
			}
		}
		if (float.IsNaN(this.dragThreshold))
		{
			this.dragThreshold = UIManager.instance.dragThreshold;
		}
		this.ScrollToItem(0, 0f);
	}

	public void UpdateCamera()
	{
		this.SetupCameraAndSizes();
		this.CalcClippingRect();
		this.RepositionItems();
	}

	public void SetupCameraAndSizes()
	{
		if (this.renderCamera == null)
		{
			if (UIManager.Exists() && UIManager.instance.uiCameras[0].camera != null)
			{
				this.renderCamera = UIManager.instance.uiCameras[0].camera;
			}
			else
			{
				this.renderCamera = Camera.main;
			}
		}
		if (this.unitsInPixels)
		{
			this.CalcScreenToWorldUnits();
			this.viewableAreaActual = new Vector2(this.viewableArea.x * this.localUnitsPerPixel, this.viewableArea.y * this.localUnitsPerPixel);
			this.itemSpacingActual = this.itemSpacing * this.localUnitsPerPixel;
			this.extraEndSpacingActual = this.extraEndSpacing * this.localUnitsPerPixel;
		}
		else
		{
			this.viewableAreaActual = this.viewableArea;
			this.itemSpacingActual = this.itemSpacing;
			this.extraEndSpacingActual = this.extraEndSpacing;
		}
	}

	protected void CalcScreenToWorldUnits()
	{
		Plane plane = new Plane(this.renderCamera.transform.forward, this.renderCamera.transform.position);
		float distanceToPoint = plane.GetDistanceToPoint(base.transform.position);
		this.localUnitsPerPixel = Vector3.Distance(this.renderCamera.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), this.renderCamera.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
	}

	protected void CalcClippingRect()
	{
		this.clientClippingRect.FromPoints(new Vector3(-this.viewableAreaActual.x * 0.5f, this.viewableAreaActual.y * 0.5f), new Vector3(this.viewableAreaActual.x * 0.5f, this.viewableAreaActual.y * 0.5f), new Vector3(-this.viewableAreaActual.x * 0.5f, -this.viewableAreaActual.y * 0.5f));
		this.clientClippingRect.MultFast(base.transform.localToWorldMatrix);
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.items[i].TextObj != null)
			{
				this.items[i].TextObj.ClippingRect = this.clientClippingRect;
			}
		}
	}

	public void SliderMoved(IUIObject slider)
	{
		this.ScrollListTo_Internal(((UISlider)slider).Value);
	}

	public void SliderInputDel(ref POINTER_INFO ptr)
	{
		if (!this.snap)
		{
			return;
		}
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP || ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE || ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF)
		{
			this.CalcSnapItem();
		}
	}

	protected void ScrollListTo_Internal(float pos)
	{
		if (float.IsNaN(pos) || this.mover == null)
		{
			return;
		}
		if (this.orientation == UIScrollList.ORIENTATION.VERTICAL)
		{
			float num = ((this.direction != UIScrollList.DIRECTION.TtoB_LtoR) ? (-1f) : 1f);
			this.mover.transform.localPosition = Vector3.up * num * Mathf.Clamp(this.amtOfPlay, 0f, this.amtOfPlay) * pos;
		}
		else
		{
			float num2 = ((this.direction != UIScrollList.DIRECTION.TtoB_LtoR) ? 1f : (-1f));
			this.mover.transform.localPosition = Vector3.right * num2 * Mathf.Clamp(this.amtOfPlay, 0f, this.amtOfPlay) * pos;
		}
		this.scrollPos = pos;
		this.ClipItems();
		if (this.slider != null)
		{
			this.slider.Value = this.scrollPos;
		}
	}

	public void ScrollListTo(float pos)
	{
		this.scrollInertia = 0f;
		this.scrollDelta = 0f;
		this.isScrolling = false;
		this.autoScrolling = false;
		this.ScrollListTo_Internal(pos);
	}

	public float ScrollPosition
	{
		get
		{
			return this.scrollPos;
		}
		set
		{
			this.ScrollListTo(value);
		}
	}

	public IUIListObject SnappedItem
	{
		get
		{
			return this.snappedItem;
		}
	}

	public void ScrollToItem(IUIListObject item, float scrollTime, EZAnimation.EASING_TYPE easing)
	{
		this.snappedItem = item;
		if (this.newItems.Count != 0)
		{
			if (this.itemsInserted || this.doItemEasing)
			{
				this.RepositionItems();
			}
			else
			{
				this.PositionNewItems();
			}
			this.itemsInserted = false;
			this.newItems.Clear();
		}
		if (this.orientation == UIScrollList.ORIENTATION.HORIZONTAL)
		{
			if (this.direction == UIScrollList.DIRECTION.TtoB_LtoR)
			{
				this.autoScrollPos = Mathf.Clamp01(item.transform.localPosition.x / this.amtOfPlay);
			}
			else
			{
				this.autoScrollPos = Mathf.Clamp01(-item.transform.localPosition.x / this.amtOfPlay);
			}
		}
		else if (this.direction == UIScrollList.DIRECTION.TtoB_LtoR)
		{
			this.autoScrollPos = Mathf.Clamp01(-item.transform.localPosition.y / this.amtOfPlay);
		}
		else
		{
			this.autoScrollPos = Mathf.Clamp01(item.transform.localPosition.y / this.amtOfPlay);
		}
		this.autoScrollInterpolator = EZAnimation.GetInterpolator(easing);
		this.autoScrollStart = this.scrollPos;
		this.autoScrollDelta = this.autoScrollPos - this.scrollPos;
		this.autoScrollDuration = scrollTime;
		this.autoScrollTime = 0f;
		this.autoScrolling = true;
		this.scrollDelta = 0f;
		this.isScrolling = false;
		if (this.itemSnappedDel != null)
		{
			this.itemSnappedDel(this.snappedItem);
		}
	}

	public void ScrollToItem(int index, float scrollTime, EZAnimation.EASING_TYPE easing)
	{
		if (index < 0 || index >= this.items.Count)
		{
			return;
		}
		this.ScrollToItem(this.items[index], scrollTime, easing);
	}

	public void ScrollToItem(IUIListObject item, float scrollTime)
	{
		this.ScrollToItem(item, scrollTime, this.snapEasing);
	}

	public void ScrollToItem(int index, float scrollTime)
	{
		this.ScrollToItem(index, scrollTime, this.snapEasing);
	}

	public void SetViewableAreaPixelDimensions(Camera cam, int width, int height)
	{
		Plane plane = new Plane(cam.transform.forward, cam.transform.position);
		float distanceToPoint = plane.GetDistanceToPoint(base.transform.position);
		float num = Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), cam.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
		this.viewableAreaActual = new Vector2((float)width * num, (float)height * num);
		this.CalcClippingRect();
		this.RepositionItems();
	}

	public void InsertItem(IUIListObject item, int position)
	{
		this.InsertItem(item, position, null, false);
	}

	public void InsertItem(IUIListObject item, int position, bool doEasing)
	{
		this.InsertItem(item, position, null, doEasing);
	}

	public void InsertItem(IUIListObject item, int position, string text)
	{
		this.InsertItem(item, position, text, false);
	}

	public void InsertItem(IUIListObject item, int position, string text, bool doEasing)
	{
		if (position >= this.items.Count)
		{
			this.doItemEasing = false;
		}
		else
		{
			this.doItemEasing = doEasing;
		}
		this.doPosEasing = doEasing;
		if (!this.m_awake)
		{
			this.Awake();
		}
		if (!this.m_started)
		{
			this.Start();
		}
		if (this.activateWhenAdding && !((Component)item).gameObject.active)
		{
			((Component)item).gameObject.SetActive(true);
		}
		if (!base.gameObject.active)
		{
			((Component)item).gameObject.SetActive(false);
		}
		item.gameObject.layer = base.gameObject.layer;
		if (this.container != null)
		{
			this.container.AddChild(item.gameObject);
		}
		item.transform.parent = this.mover.transform;
		item.transform.localRotation = Quaternion.identity;
		item.transform.localScale = Vector3.one;
		item.transform.localPosition = Vector3.zero;
		item.SetList(this);
		if (text != null)
		{
			item.Text = text;
		}
		position = Mathf.Clamp(position, 0, this.items.Count);
		if (this.clipContents)
		{
			item.Hide(true);
			if (!item.Managed)
			{
				item.gameObject.SetActive(false);
			}
		}
		item.Index = position;
		this.newItems.Add(item);
		if (position != this.items.Count)
		{
			this.itemsInserted = true;
			this.items.Insert(position, item);
			if (this.visibleItems.Count == 0)
			{
				this.visibleItems.Add(item);
			}
			else if (item.Index > 0)
			{
				int num = this.visibleItems.IndexOf(this.items[item.Index - 1]);
				if (num == -1)
				{
					if (this.visibleItems[0].Index >= item.Index)
					{
						this.visibleItems.Insert(0, item);
					}
					else
					{
						this.visibleItems.Add(item);
					}
				}
				else
				{
					this.visibleItems.Insert(num + 1, item);
				}
			}
		}
		else
		{
			this.items.Add(item);
			this.visibleItems.Add(item);
		}
		if (this.positionItemsImmediately)
		{
			if (this.itemsInserted || this.doItemEasing)
			{
				this.RepositionItems();
			}
			else
			{
				this.PositionNewItems();
			}
		}
	}

	protected void PositionNewItems()
	{
		IUIListObject iuilistObject = null;
		float num = 0f;
		for (int i = 0; i < this.newItems.Count; i++)
		{
			if (this.newItems[i] != null)
			{
				int index = this.newItems[i].Index;
				IUIListObject iuilistObject2 = this.items[index];
				iuilistObject2.FindOuterEdges();
				iuilistObject2.UpdateCollider();
				float num2 = 0f;
				float num3 = 0f;
				bool flag = false;
				if (this.orientation == UIScrollList.ORIENTATION.HORIZONTAL)
				{
					if (index > 0)
					{
						flag = true;
						iuilistObject = this.items[index - 1];
						if (this.direction == UIScrollList.DIRECTION.TtoB_LtoR)
						{
							num2 = iuilistObject.transform.localPosition.x + iuilistObject.BottomRightEdge.x + this.itemSpacingActual - iuilistObject2.TopLeftEdge.x;
						}
						else
						{
							num2 = iuilistObject.transform.localPosition.x - iuilistObject.BottomRightEdge.x - this.itemSpacingActual + iuilistObject2.TopLeftEdge.x;
						}
					}
					else
					{
						if (this.spacingAtEnds)
						{
							flag = true;
						}
						if (this.direction == UIScrollList.DIRECTION.TtoB_LtoR)
						{
							num2 = this.viewableAreaActual.x * -0.5f - iuilistObject2.TopLeftEdge.x + ((!this.spacingAtEnds) ? 0f : this.itemSpacingActual) + this.extraEndSpacingActual;
						}
						else
						{
							num2 = this.viewableAreaActual.x * 0.5f - iuilistObject2.BottomRightEdge.x - ((!this.spacingAtEnds) ? 0f : this.itemSpacingActual) - this.extraEndSpacingActual;
						}
					}
					switch (this.alignment)
					{
					case UIScrollList.ALIGNMENT.LEFT_TOP:
						num3 = this.viewableAreaActual.y * 0.5f - iuilistObject2.TopLeftEdge.y;
						break;
					case UIScrollList.ALIGNMENT.CENTER:
						num3 = 0f;
						break;
					case UIScrollList.ALIGNMENT.RIGHT_BOTTOM:
						num3 = this.viewableAreaActual.y * -0.5f - iuilistObject2.BottomRightEdge.y;
						break;
					}
					num += iuilistObject2.BottomRightEdge.x - iuilistObject2.TopLeftEdge.x + ((!flag || iuilistObject == null) ? 0f : this.itemSpacingActual);
				}
				else
				{
					if (index > 0)
					{
						flag = true;
						iuilistObject = this.items[index - 1];
						if (this.direction == UIScrollList.DIRECTION.TtoB_LtoR)
						{
							num3 = iuilistObject.transform.localPosition.y + iuilistObject.BottomRightEdge.y - this.itemSpacingActual - iuilistObject2.TopLeftEdge.y;
						}
						else
						{
							num3 = iuilistObject.transform.localPosition.y - iuilistObject.BottomRightEdge.y + this.itemSpacingActual + iuilistObject2.TopLeftEdge.y;
						}
					}
					else
					{
						if (this.spacingAtEnds)
						{
							flag = true;
						}
						if (this.direction == UIScrollList.DIRECTION.TtoB_LtoR)
						{
							num3 = this.viewableAreaActual.y * 0.5f - iuilistObject2.TopLeftEdge.y - ((!this.spacingAtEnds) ? 0f : this.itemSpacingActual) - this.extraEndSpacingActual;
						}
						else
						{
							num3 = this.viewableAreaActual.y * -0.5f - iuilistObject2.BottomRightEdge.y + ((!this.spacingAtEnds) ? 0f : this.itemSpacingActual) + this.extraEndSpacingActual;
						}
					}
					switch (this.alignment)
					{
					case UIScrollList.ALIGNMENT.LEFT_TOP:
						num2 = this.viewableAreaActual.x * -0.5f - iuilistObject2.TopLeftEdge.x;
						break;
					case UIScrollList.ALIGNMENT.CENTER:
						num2 = 0f;
						break;
					case UIScrollList.ALIGNMENT.RIGHT_BOTTOM:
						num2 = this.viewableAreaActual.x * 0.5f - iuilistObject2.BottomRightEdge.x;
						break;
					}
					num += iuilistObject2.TopLeftEdge.y - iuilistObject2.BottomRightEdge.y + ((!flag || iuilistObject == null) ? 0f : this.itemSpacingActual);
				}
				iuilistObject2.transform.localPosition = new Vector3(num2, num3);
			}
		}
		this.UpdateContentExtents(num);
		this.ClipItems();
		this.newItems.Clear();
	}

	public void AddItem(GameObject itemGO)
	{
		IUIListObject iuilistObject = (IUIListObject)itemGO.GetComponent(typeof(IUIListObject));
		if (iuilistObject == null)
		{
			Debug.LogWarning(string.Concat(new string[] { "GameObject \"", itemGO.name, "\" does not contain any list item component suitable to be added to scroll list \"", base.name, "\"." }));
			return;
		}
		this.AddItem(iuilistObject, null);
	}

	public void AddItem(IUIListObject item)
	{
		this.AddItem(item, null);
	}

	public void AddItem(IUIListObject item, string text)
	{
		if (!this.m_awake)
		{
			this.Awake();
		}
		if (!this.m_started)
		{
			this.Start();
		}
		this.InsertItem(item, this.items.Count, text, false);
	}

	public IUIListObject CreateItem(GameObject prefab)
	{
		if (!this.m_awake)
		{
			this.Awake();
		}
		if (!this.m_started)
		{
			this.Start();
		}
		return this.CreateItem(prefab, this.items.Count, null);
	}

	public IUIListObject CreateItem(GameObject prefab, string text)
	{
		if (!this.m_awake)
		{
			this.Awake();
		}
		if (!this.m_started)
		{
			this.Start();
		}
		return this.CreateItem(prefab, this.items.Count, text);
	}

	public IUIListObject CreateItem(GameObject prefab, int position, bool doEasing)
	{
		return this.CreateItem(prefab, position, null, doEasing);
	}

	public IUIListObject CreateItem(GameObject prefab, int position)
	{
		return this.CreateItem(prefab, position, null, false);
	}

	public IUIListObject CreateItem(GameObject prefab, int position, string text)
	{
		return this.CreateItem(prefab, position, text, false);
	}

	public IUIListObject CreateItem(GameObject prefab, int position, string text, bool doEasing)
	{
		IUIListObject iuilistObject = (IUIListObject)prefab.GetComponent(typeof(IUIListObject));
		if (iuilistObject == null)
		{
			return null;
		}
		iuilistObject.RenderCamera = this.renderCamera;
		GameObject gameObject;
		if (this.manager != null)
		{
			if (iuilistObject.IsContainer())
			{
				gameObject = (GameObject)global::UnityEngine.Object.Instantiate(prefab);
				Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(SpriteRoot));
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					this.manager.AddSprite((SpriteRoot)componentsInChildren[i]);
				}
			}
			else
			{
				SpriteRoot spriteRoot = this.manager.CreateSprite(prefab);
				if (spriteRoot == null)
				{
					return null;
				}
				gameObject = spriteRoot.gameObject;
			}
		}
		else
		{
			gameObject = (GameObject)global::UnityEngine.Object.Instantiate(prefab);
		}
		iuilistObject = (IUIListObject)gameObject.GetComponent(typeof(IUIListObject));
		if (iuilistObject == null)
		{
			return null;
		}
		this.InsertItem(iuilistObject, position, text, doEasing);
		return iuilistObject;
	}

	protected void UpdateContentExtents(float change)
	{
		float num = this.amtOfPlay;
		float num2 = ((!this.spacingAtEnds) ? 0f : (this.itemSpacingActual * 2f)) + this.extraEndSpacingActual * 2f;
		this.contentExtents += change;
		if (this.orientation == UIScrollList.ORIENTATION.HORIZONTAL)
		{
			this.amtOfPlay = this.contentExtents + num2 - this.viewableAreaActual.x;
			this.scrollMax = this.viewableAreaActual.x / (this.contentExtents + num2 - this.viewableAreaActual.x) * 0.5f;
		}
		else
		{
			this.amtOfPlay = this.contentExtents + num2 - this.viewableAreaActual.y;
			this.scrollMax = this.viewableAreaActual.y / (this.contentExtents + num2 - this.viewableAreaActual.y) * 0.5f;
		}
		float num3 = num * this.scrollPos / this.amtOfPlay;
		if (this.doPosEasing && num3 > 1f)
		{
			this.scrollPosAnim = AnimateRotation.Do(base.gameObject, EZAnimation.ANIM_MODE.By, Vector3.zero, new EZAnimation.Interpolator(this.ScrollPosInterpolator), this.positionEaseDuration, this.positionEaseDelay, null, new EZAnimation.CompletionDelegate(this.OnPosEasingDone));
			this.scrollPosAnim.Data = new Vector2(num3, 1f - num3);
			this.itemEasers.Add(this.scrollPosAnim);
		}
		else
		{
			this.ScrollListTo_Internal(Mathf.Clamp01(num3));
		}
		this.doPosEasing = false;
	}

	protected float ScrollPosInterpolator(float time, float start, float delta, float duration)
	{
		Vector2 vector = (Vector2)this.scrollPosAnim.Data;
		this.ScrollListTo_Internal(EZAnimation.GetInterpolator(this.positionEasing)(time, vector.x, vector.y, duration));
		if (time >= duration)
		{
			this.scrollPosAnim = null;
		}
		return start;
	}

	protected float GetYCentered(IUIListObject item)
	{
		return 0f;
	}

	protected float GetYAlignTop(IUIListObject item)
	{
		return this.viewableAreaActual.y * 0.5f - item.TopLeftEdge.y;
	}

	protected float GetYAlignBottom(IUIListObject item)
	{
		return this.viewableAreaActual.y * -0.5f - item.BottomRightEdge.y;
	}

	protected float GetXCentered(IUIListObject item)
	{
		return 0f;
	}

	protected float GetXAlignLeft(IUIListObject item)
	{
		return this.viewableAreaActual.x * -0.5f - item.TopLeftEdge.x;
	}

	protected float GetXAlignRight(IUIListObject item)
	{
		return this.viewableAreaActual.x * 0.5f - item.BottomRightEdge.x;
	}

	public void PositionItems()
	{
		if (this.itemEasers.Count > 0)
		{
			for (int i = 0; i < this.itemEasers.Count; i++)
			{
				this.itemEasers[i].CompletedDelegate = null;
				this.itemEasers[i].End();
			}
			this.itemEasers.Clear();
			if (this.blockInputWhileEasing)
			{
				UIManager.instance.UnlockInput();
			}
		}
		if (this.orientation == UIScrollList.ORIENTATION.HORIZONTAL)
		{
			this.PositionHorizontally(false);
		}
		else
		{
			this.PositionVertically(false);
		}
		this.UpdateContentExtents(0f);
		this.ClipItems();
		if (this.itemEasers.Count > 0 && this.blockInputWhileEasing)
		{
			UIManager.instance.LockInput();
		}
		this.doItemEasing = false;
	}

	public void RepositionItems()
	{
		if (this.itemEasers.Count > 0)
		{
			for (int i = 0; i < this.itemEasers.Count; i++)
			{
				this.itemEasers[i].CompletedDelegate = null;
				this.itemEasers[i].End();
			}
			this.itemEasers.Clear();
			if (this.blockInputWhileEasing)
			{
				UIManager.instance.UnlockInput();
			}
		}
		if (this.orientation == UIScrollList.ORIENTATION.HORIZONTAL)
		{
			this.PositionHorizontally(true);
		}
		else
		{
			this.PositionVertically(true);
		}
		this.UpdateContentExtents(0f);
		this.ClipItems();
		if (this.itemEasers.Count > 0 && this.blockInputWhileEasing)
		{
			UIManager.instance.LockInput();
		}
		this.doItemEasing = false;
	}

	protected void PositionHorizontally(bool updateExtents)
	{
		this.contentExtents = 0f;
		UIScrollList.ItemAlignmentDel itemAlignmentDel;
		switch (this.alignment)
		{
		case UIScrollList.ALIGNMENT.LEFT_TOP:
			itemAlignmentDel = new UIScrollList.ItemAlignmentDel(this.GetYAlignTop);
			break;
		case UIScrollList.ALIGNMENT.CENTER:
			itemAlignmentDel = new UIScrollList.ItemAlignmentDel(this.GetYCentered);
			break;
		case UIScrollList.ALIGNMENT.RIGHT_BOTTOM:
			itemAlignmentDel = new UIScrollList.ItemAlignmentDel(this.GetYAlignBottom);
			break;
		default:
			itemAlignmentDel = new UIScrollList.ItemAlignmentDel(this.GetYCentered);
			break;
		}
		if (this.direction == UIScrollList.DIRECTION.TtoB_LtoR)
		{
			float num = this.viewableAreaActual.x * -0.5f + ((!this.spacingAtEnds) ? 0f : this.itemSpacingActual) + this.extraEndSpacingActual;
			for (int i = 0; i < this.items.Count; i++)
			{
				if (updateExtents)
				{
					this.items[i].FindOuterEdges();
					this.items[i].UpdateCollider();
				}
				Vector3 vector = new Vector3(num - this.items[i].TopLeftEdge.x, itemAlignmentDel(this.items[i]));
				if (this.doItemEasing)
				{
					if (this.newItems.Contains(this.items[i]))
					{
						this.items[i].transform.localPosition = vector;
					}
					else
					{
						this.itemEasers.Add(AnimatePosition.Do(this.items[i].gameObject, EZAnimation.ANIM_MODE.To, vector, EZAnimation.GetInterpolator(this.positionEasing), this.positionEaseDuration, this.positionEaseDelay, null, new EZAnimation.CompletionDelegate(this.OnPosEasingDone)));
					}
				}
				else
				{
					this.items[i].transform.localPosition = vector;
				}
				float num2 = this.items[i].BottomRightEdge.x - this.items[i].TopLeftEdge.x + this.itemSpacingActual;
				this.contentExtents += num2;
				num += num2;
				this.items[i].Index = i;
			}
			if (!this.spacingAtEnds)
			{
				this.contentExtents -= this.itemSpacingActual;
			}
		}
		else
		{
			float num = this.viewableAreaActual.x * 0.5f - ((!this.spacingAtEnds) ? 0f : this.itemSpacingActual) - this.extraEndSpacingActual;
			for (int j = 0; j < this.items.Count; j++)
			{
				if (updateExtents)
				{
					this.items[j].FindOuterEdges();
					this.items[j].UpdateCollider();
				}
				Vector3 vector = new Vector3(num - this.items[j].BottomRightEdge.x, itemAlignmentDel(this.items[j]));
				if (this.doItemEasing)
				{
					if (this.newItems.Contains(this.items[j]))
					{
						this.items[j].transform.localPosition = vector;
					}
					else
					{
						this.itemEasers.Add(AnimatePosition.Do(this.items[j].gameObject, EZAnimation.ANIM_MODE.To, vector, EZAnimation.GetInterpolator(this.positionEasing), this.positionEaseDuration, this.positionEaseDelay, null, new EZAnimation.CompletionDelegate(this.OnPosEasingDone)));
					}
				}
				else
				{
					this.items[j].transform.localPosition = vector;
				}
				float num2 = this.items[j].BottomRightEdge.x - this.items[j].TopLeftEdge.x + this.itemSpacingActual;
				this.contentExtents += num2;
				num -= num2;
				this.items[j].Index = j;
			}
			if (!this.spacingAtEnds)
			{
				this.contentExtents -= this.itemSpacingActual;
			}
		}
	}

	protected void PositionVertically(bool updateExtents)
	{
		this.contentExtents = 0f;
		UIScrollList.ItemAlignmentDel itemAlignmentDel;
		switch (this.alignment)
		{
		case UIScrollList.ALIGNMENT.LEFT_TOP:
			itemAlignmentDel = new UIScrollList.ItemAlignmentDel(this.GetXAlignLeft);
			break;
		case UIScrollList.ALIGNMENT.CENTER:
			itemAlignmentDel = new UIScrollList.ItemAlignmentDel(this.GetXCentered);
			break;
		case UIScrollList.ALIGNMENT.RIGHT_BOTTOM:
			itemAlignmentDel = new UIScrollList.ItemAlignmentDel(this.GetXAlignRight);
			break;
		default:
			itemAlignmentDel = new UIScrollList.ItemAlignmentDel(this.GetXCentered);
			break;
		}
		if (this.direction == UIScrollList.DIRECTION.TtoB_LtoR)
		{
			float num = this.viewableAreaActual.y * 0.5f - ((!this.spacingAtEnds) ? 0f : this.itemSpacingActual) - this.extraEndSpacingActual;
			for (int i = 0; i < this.items.Count; i++)
			{
				if (updateExtents)
				{
					this.items[i].FindOuterEdges();
					this.items[i].UpdateCollider();
				}
				Vector3 vector = new Vector3(itemAlignmentDel(this.items[i]), num - this.items[i].TopLeftEdge.y);
				if (this.doItemEasing)
				{
					if (this.newItems.Contains(this.items[i]))
					{
						this.items[i].transform.localPosition = vector;
					}
					else
					{
						this.itemEasers.Add(AnimatePosition.Do(this.items[i].gameObject, EZAnimation.ANIM_MODE.To, vector, EZAnimation.GetInterpolator(this.positionEasing), this.positionEaseDuration, this.positionEaseDelay, null, new EZAnimation.CompletionDelegate(this.OnPosEasingDone)));
					}
				}
				else
				{
					this.items[i].transform.localPosition = vector;
				}
				float num2 = this.items[i].TopLeftEdge.y - this.items[i].BottomRightEdge.y + this.itemSpacingActual;
				this.contentExtents += num2;
				num -= num2;
				this.items[i].Index = i;
			}
			if (!this.spacingAtEnds)
			{
				this.contentExtents -= this.itemSpacingActual;
			}
		}
		else
		{
			float num = this.viewableAreaActual.y * -0.5f + ((!this.spacingAtEnds) ? 0f : this.itemSpacingActual) + this.extraEndSpacingActual;
			for (int j = 0; j < this.items.Count; j++)
			{
				if (updateExtents)
				{
					this.items[j].FindOuterEdges();
					this.items[j].UpdateCollider();
				}
				Vector3 vector = new Vector3(itemAlignmentDel(this.items[j]), num - this.items[j].BottomRightEdge.y);
				if (this.doItemEasing)
				{
					if (this.newItems.Contains(this.items[j]))
					{
						this.items[j].transform.localPosition = vector;
					}
					else
					{
						this.itemEasers.Add(AnimatePosition.Do(this.items[j].gameObject, EZAnimation.ANIM_MODE.To, vector, EZAnimation.GetInterpolator(this.positionEasing), this.positionEaseDuration, this.positionEaseDelay, null, new EZAnimation.CompletionDelegate(this.OnPosEasingDone)));
					}
				}
				else
				{
					this.items[j].transform.localPosition = vector;
				}
				float num2 = this.items[j].TopLeftEdge.y - this.items[j].BottomRightEdge.y + this.itemSpacingActual;
				this.contentExtents += num2;
				num += num2;
				this.items[j].Index = j;
			}
			if (!this.spacingAtEnds)
			{
				this.contentExtents -= this.itemSpacingActual;
			}
		}
	}

	protected void OnPosEasingDone(EZAnimation anim)
	{
		this.itemEasers.Remove(anim);
		if (this.itemEasers.Count == 0 && this.blockInputWhileEasing)
		{
			UIManager.instance.UnlockInput();
		}
	}

	protected void ClipItems()
	{
		if (this.mover == null || this.items.Count < 1 || !this.clipContents || !base.gameObject.active)
		{
			return;
		}
		IUIListObject iuilistObject = null;
		if (this.orientation == UIScrollList.ORIENTATION.HORIZONTAL)
		{
			float x = this.mover.transform.localPosition.x;
			float num = this.viewableAreaActual.x * -0.5f - x;
			float num2 = this.viewableAreaActual.x * 0.5f - x;
			int i = (int)((float)(this.items.Count - 1) * Mathf.Clamp01(this.scrollPos));
			if (this.direction == UIScrollList.DIRECTION.TtoB_LtoR)
			{
				float num3 = this.items[i].transform.localPosition.x;
				if (this.items[i].BottomRightEdge.x + num3 >= num)
				{
					for (i--; i > -1; i--)
					{
						num3 = this.items[i].transform.localPosition.x;
						if (this.items[i].BottomRightEdge.x + num3 < num)
						{
							break;
						}
					}
					iuilistObject = this.items[i + 1];
				}
				else
				{
					while (i < this.items.Count)
					{
						num3 = this.items[i].transform.localPosition.x;
						if (this.items[i].BottomRightEdge.x + num3 >= num)
						{
							iuilistObject = this.items[i];
							break;
						}
						i++;
					}
				}
				if (iuilistObject != null)
				{
					this.tempVisItems.Add(iuilistObject);
					if (!iuilistObject.gameObject.active)
					{
						iuilistObject.gameObject.SetActive(true);
					}
					iuilistObject.Hide(false);
					iuilistObject.ClippingRect = this.clientClippingRect;
					num3 = iuilistObject.transform.localPosition.x;
					if (iuilistObject.BottomRightEdge.x + num3 < num2)
					{
						for (i = iuilistObject.Index + 1; i < this.items.Count; i++)
						{
							num3 = this.items[i].transform.localPosition.x;
							if (this.items[i].BottomRightEdge.x + num3 >= num2)
							{
								if (!this.items[i].gameObject.active)
								{
									this.items[i].gameObject.SetActive(true);
								}
								this.items[i].Hide(false);
								this.items[i].ClippingRect = this.clientClippingRect;
								this.tempVisItems.Add(this.items[i]);
								break;
							}
							if (!this.items[i].gameObject.active)
							{
								this.items[i].gameObject.SetActive(true);
							}
							this.items[i].Hide(false);
							this.items[i].Clipped = false;
							this.tempVisItems.Add(this.items[i]);
						}
					}
				}
			}
			else
			{
				float num3 = this.items[i].transform.localPosition.x;
				if (this.items[i].TopLeftEdge.x + num3 <= num2)
				{
					for (i--; i > -1; i--)
					{
						num3 = this.items[i].transform.localPosition.x;
						if (this.items[i].TopLeftEdge.x + num3 > num2)
						{
							break;
						}
					}
					iuilistObject = this.items[i + 1];
				}
				else
				{
					while (i < this.items.Count)
					{
						num3 = this.items[i].transform.localPosition.x;
						if (this.items[i].TopLeftEdge.x + num3 <= num2)
						{
							iuilistObject = this.items[i];
							break;
						}
						i++;
					}
				}
				if (iuilistObject != null)
				{
					this.tempVisItems.Add(iuilistObject);
					if (!iuilistObject.gameObject.active)
					{
						iuilistObject.gameObject.SetActive(true);
					}
					iuilistObject.Hide(false);
					iuilistObject.ClippingRect = this.clientClippingRect;
					num3 = iuilistObject.transform.localPosition.x;
					if (iuilistObject.TopLeftEdge.x + num3 > num)
					{
						for (i = iuilistObject.Index + 1; i < this.items.Count; i++)
						{
							num3 = this.items[i].transform.localPosition.x;
							if (this.items[i].TopLeftEdge.x + num3 <= num)
							{
								if (!this.items[i].gameObject.active)
								{
									this.items[i].gameObject.SetActive(true);
								}
								this.items[i].Hide(false);
								this.items[i].ClippingRect = this.clientClippingRect;
								this.tempVisItems.Add(this.items[i]);
								break;
							}
							if (!this.items[i].gameObject.active)
							{
								this.items[i].gameObject.SetActive(true);
							}
							this.items[i].Hide(false);
							this.items[i].Clipped = false;
							this.tempVisItems.Add(this.items[i]);
						}
					}
				}
			}
		}
		else
		{
			float y = this.mover.transform.localPosition.y;
			float num4 = this.viewableAreaActual.y * 0.5f - y;
			float num5 = this.viewableAreaActual.y * -0.5f - y;
			int j = (int)((float)(this.items.Count - 1) * Mathf.Clamp01(this.scrollPos));
			if (this.direction == UIScrollList.DIRECTION.TtoB_LtoR)
			{
				float num6 = this.items[j].transform.localPosition.y;
				if (this.items[j].BottomRightEdge.y + num6 <= num4)
				{
					for (j--; j > -1; j--)
					{
						num6 = this.items[j].transform.localPosition.y;
						if (this.items[j].BottomRightEdge.y + num6 > num4)
						{
							break;
						}
					}
					iuilistObject = this.items[j + 1];
				}
				else
				{
					while (j < this.items.Count)
					{
						num6 = this.items[j].transform.localPosition.y;
						if (this.items[j].BottomRightEdge.y + num6 <= num4)
						{
							iuilistObject = this.items[j];
							break;
						}
						j++;
					}
				}
				if (iuilistObject != null)
				{
					this.tempVisItems.Add(iuilistObject);
					if (!iuilistObject.gameObject.active)
					{
						iuilistObject.gameObject.SetActive(true);
					}
					iuilistObject.Hide(false);
					iuilistObject.ClippingRect = this.clientClippingRect;
					num6 = iuilistObject.transform.localPosition.y;
					if (iuilistObject.BottomRightEdge.y + num6 > num5)
					{
						for (j = iuilistObject.Index + 1; j < this.items.Count; j++)
						{
							num6 = this.items[j].transform.localPosition.y;
							if (this.items[j].BottomRightEdge.y + num6 <= num5)
							{
								if (!this.items[j].gameObject.active)
								{
									this.items[j].gameObject.SetActive(true);
								}
								this.items[j].Hide(false);
								this.items[j].ClippingRect = this.clientClippingRect;
								this.tempVisItems.Add(this.items[j]);
								break;
							}
							if (!this.items[j].gameObject.active)
							{
								this.items[j].gameObject.SetActive(true);
							}
							this.items[j].Hide(false);
							this.items[j].Clipped = false;
							this.tempVisItems.Add(this.items[j]);
						}
					}
				}
			}
			else
			{
				float num6 = this.items[j].transform.localPosition.y;
				if (this.items[j].TopLeftEdge.y + num6 >= num5)
				{
					for (j--; j > -1; j--)
					{
						num6 = this.items[j].transform.localPosition.y;
						if (this.items[j].TopLeftEdge.y + num6 < num5)
						{
							break;
						}
					}
					iuilistObject = this.items[j + 1];
				}
				else
				{
					while (j < this.items.Count)
					{
						num6 = this.items[j].transform.localPosition.y;
						if (this.items[j].TopLeftEdge.y + num6 >= num5)
						{
							iuilistObject = this.items[j];
							break;
						}
						j++;
					}
				}
				if (iuilistObject != null)
				{
					this.tempVisItems.Add(iuilistObject);
					if (!iuilistObject.gameObject.active)
					{
						iuilistObject.gameObject.SetActive(true);
					}
					iuilistObject.Hide(false);
					iuilistObject.ClippingRect = this.clientClippingRect;
					num6 = iuilistObject.transform.localPosition.y;
					if (iuilistObject.TopLeftEdge.y + num6 < num4)
					{
						for (j = iuilistObject.Index + 1; j < this.items.Count; j++)
						{
							num6 = this.items[j].transform.localPosition.y;
							if (this.items[j].TopLeftEdge.y + num6 >= num4)
							{
								if (!this.items[j].gameObject.active)
								{
									this.items[j].gameObject.SetActive(true);
								}
								this.items[j].Hide(false);
								this.items[j].ClippingRect = this.clientClippingRect;
								this.tempVisItems.Add(this.items[j]);
								break;
							}
							if (!this.items[j].gameObject.active)
							{
								this.items[j].gameObject.SetActive(true);
							}
							this.items[j].Hide(false);
							this.items[j].Clipped = false;
							this.tempVisItems.Add(this.items[j]);
						}
					}
				}
			}
		}
		if (iuilistObject == null)
		{
			return;
		}
		IUIListObject iuilistObject2 = this.tempVisItems[this.tempVisItems.Count - 1];
		if (this.visibleItems.Count > 0)
		{
			if (this.visibleItems[0].Index > iuilistObject2.Index || this.visibleItems[this.visibleItems.Count - 1].Index < iuilistObject.Index)
			{
				for (int k = 0; k < this.visibleItems.Count; k++)
				{
					this.visibleItems[k].Hide(true);
					if (!this.visibleItems[k].Managed)
					{
						this.visibleItems[k].gameObject.SetActive(false);
					}
				}
			}
			else
			{
				for (int l = 0; l < this.visibleItems.Count; l++)
				{
					if (this.visibleItems[l].Index >= iuilistObject.Index)
					{
						break;
					}
					this.visibleItems[l].Hide(true);
					if (!this.visibleItems[l].Managed)
					{
						this.visibleItems[l].gameObject.SetActive(false);
					}
				}
				for (int m = this.visibleItems.Count - 1; m > -1; m--)
				{
					if (this.visibleItems[m].Index <= iuilistObject2.Index)
					{
						break;
					}
					this.visibleItems[m].Hide(true);
					if (!this.visibleItems[m].Managed)
					{
						this.visibleItems[m].gameObject.SetActive(false);
					}
				}
			}
		}
		List<IUIListObject> list = this.visibleItems;
		this.visibleItems = this.tempVisItems;
		this.tempVisItems = list;
		this.tempVisItems.Clear();
	}

	public void DidSelect(IUIListObject item)
	{
		if (this.selectedItem != null)
		{
			this.selectedItem.selected = false;
		}
		this.selectedItem = item;
		item.selected = true;
		this.DidClick(item);
	}

	public void DidClick(IUIObject item)
	{
		this.lastClickedControl = item;
		if (this.scriptWithMethodToInvoke != null)
		{
			this.scriptWithMethodToInvoke.Invoke(this.methodToInvokeOnSelect, 0f);
		}
		if (this.changeDelegate != null)
		{
			this.changeDelegate(this);
		}
	}

	public void ListDragged(POINTER_INFO ptr)
	{
		if (!this.touchScroll || !this.controlIsEnabled)
		{
			return;
		}
		this.autoScrolling = false;
		this.listMoved = true;
		Plane plane = default(Plane);
		if (Mathf.Approximately(ptr.inputDelta.sqrMagnitude, 0f))
		{
			this.scrollDelta = 0f;
			return;
		}
		plane.SetNormalAndPosition(this.mover.transform.forward * -1f, this.mover.transform.position);
		float num;
		plane.Raycast(ptr.ray, out num);
		Vector3 vector = ptr.ray.origin + ptr.ray.direction * num;
		plane.Raycast(ptr.prevRay, out num);
		Vector3 vector2 = ptr.prevRay.origin + ptr.prevRay.direction * num;
		vector = base.transform.InverseTransformPoint(vector);
		vector2 = base.transform.InverseTransformPoint(vector2);
		Vector3 vector3 = vector - vector2;
		if (this.orientation == UIScrollList.ORIENTATION.HORIZONTAL)
		{
			this.scrollDelta = -vector3.x / this.amtOfPlay;
		}
		else
		{
			this.scrollDelta = vector3.y / this.amtOfPlay;
		}
		float num2 = this.scrollPos + this.scrollDelta;
		if (num2 > 1f)
		{
			this.scrollDelta *= Mathf.Clamp01(1f - (num2 - 1f) / this.scrollMax);
		}
		else if (num2 < 0f)
		{
			this.scrollDelta *= Mathf.Clamp01(1f + num2 / this.scrollMax);
		}
		if (this.direction == UIScrollList.DIRECTION.BtoT_RtoL)
		{
			this.scrollDelta *= -1f;
		}
		this.ScrollListTo_Internal(this.scrollPos + this.scrollDelta);
		this.noTouch = false;
		this.isScrolling = true;
	}

	public void ScrollWheel(float amt)
	{
		if (this.direction == UIScrollList.DIRECTION.BtoT_RtoL)
		{
			amt *= -1f;
		}
		this.ScrollListTo(Mathf.Clamp01(this.scrollPos - amt * this.scrollWheelFactor / this.amtOfPlay));
	}

	public void PointerReleased()
	{
		this.noTouch = true;
		if (this.scrollInertia != 0f)
		{
			this.scrollDelta = this.scrollInertia;
		}
		this.scrollInertia = 0f;
		if (this.snap && this.listMoved)
		{
			this.CalcSnapItem();
		}
		this.listMoved = false;
	}

	public void OnEnable()
	{
		base.gameObject.SetActive(true);
		if (this.repositionOnEnable)
		{
			this.RepositionItems();
		}
		this.ClipItems();
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying)
		{
			if (EZAnimator.Exists())
			{
				EZAnimator.instance.Stop(base.gameObject);
				EZAnimator.instance.Stop(this);
			}
			if (this.detargetOnDisable && UIManager.Exists())
			{
				UIManager.instance.Detarget(this);
			}
		}
	}

	public float ContentExtents
	{
		get
		{
			return this.contentExtents;
		}
	}

	public float UnviewableArea
	{
		get
		{
			return this.amtOfPlay;
		}
	}

	public IUIListObject SelectedItem
	{
		get
		{
			return this.selectedItem;
		}
		set
		{
			if (this.selectedItem != null)
			{
				this.selectedItem.selected = false;
			}
			if (value == null)
			{
				this.selectedItem = null;
				return;
			}
			this.selectedItem = value;
			this.selectedItem.selected = true;
		}
	}

	public IUIObject LastClickedControl
	{
		get
		{
			return this.lastClickedControl;
		}
	}

	public void SetSelectedItem(int index)
	{
		IUIListObject iuilistObject = this.selectedItem;
		if (index < 0 || index >= this.items.Count)
		{
			if (this.selectedItem != null)
			{
				this.selectedItem.selected = false;
			}
			this.selectedItem = null;
			if (iuilistObject != this.selectedItem && this.changeDelegate != null)
			{
				this.changeDelegate(this);
			}
			return;
		}
		IUIListObject iuilistObject2 = this.items[index];
		if (this.selectedItem != null)
		{
			this.selectedItem.selected = false;
		}
		this.selectedItem = iuilistObject2;
		iuilistObject2.selected = true;
		if (iuilistObject != this.selectedItem && this.changeDelegate != null)
		{
			this.changeDelegate(this);
		}
	}

	public int Count
	{
		get
		{
			return this.items.Count;
		}
	}

	public IUIListObject GetItem(int index)
	{
		if (index < 0 || index >= this.items.Count)
		{
			return null;
		}
		return this.items[index];
	}

	public void RemoveItem(int index, bool destroy)
	{
		this.RemoveItem(index, destroy, false);
	}

	public void RemoveItem(int index, bool destroy, bool doEasing)
	{
		if (index < 0 || index >= this.items.Count)
		{
			return;
		}
		if (index == this.items.Count - 1)
		{
			this.doItemEasing = false;
		}
		else
		{
			this.doItemEasing = doEasing;
		}
		this.doPosEasing = doEasing;
		if (this.container != null)
		{
			this.container.RemoveChild(this.items[index].gameObject);
		}
		if (this.selectedItem == this.items[index])
		{
			this.selectedItem = null;
			this.items[index].selected = false;
		}
		if (this.lastClickedControl != null && (this.lastClickedControl == this.items[index] || (this.lastClickedControl.Container != null && this.lastClickedControl.Container.Equals(this.items[index]))))
		{
			this.lastClickedControl = null;
		}
		this.visibleItems.Remove(this.items[index]);
		if (destroy)
		{
			this.items[index].Delete();
			global::UnityEngine.Object.Destroy(this.items[index].gameObject);
		}
		else
		{
			this.items[index].transform.parent = null;
			this.items[index].gameObject.SetActive(false);
		}
		this.items.RemoveAt(index);
		this.PositionItems();
	}

	public void RemoveItem(IUIListObject item, bool destroy)
	{
		this.RemoveItem(item, destroy, false);
	}

	public void RemoveItem(IUIListObject item, bool destroy, bool doEasing)
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.items[i] == item)
			{
				this.RemoveItem(i, destroy, doEasing);
				return;
			}
		}
	}

	public void ClearList(bool destroy)
	{
		this.RemoveItemsFromContainer();
		this.selectedItem = null;
		this.lastClickedControl = null;
		for (int i = 0; i < this.items.Count; i++)
		{
			this.items[i].transform.parent = null;
			if (destroy)
			{
				global::UnityEngine.Object.Destroy(this.items[i].gameObject);
			}
			else
			{
				this.items[i].gameObject.SetActive(false);
			}
		}
		this.visibleItems.Clear();
		this.items.Clear();
		this.PositionItems();
	}

	public void OnInput(POINTER_INFO ptr)
	{
		if (!this.m_controlIsEnabled)
		{
			if (this.Container != null)
			{
				ptr.callerIsControl = true;
				this.Container.OnInput(ptr);
			}
			return;
		}
		if (Vector3.SqrMagnitude(ptr.origPos - ptr.devicePos) > this.dragThreshold * this.dragThreshold)
		{
			ptr.isTap = false;
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
			}
		}
		else
		{
			ptr.isTap = true;
		}
		if (this.inputDelegate != null)
		{
			this.inputDelegate(ref ptr);
		}
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			if (ptr.active)
			{
				this.ListDragged(ptr);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
		case POINTER_INFO.INPUT_EVENT.TAP:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			this.PointerReleased();
			break;
		case POINTER_INFO.INPUT_EVENT.DRAG:
			if (!ptr.isTap)
			{
				this.ListDragged(ptr);
			}
			break;
		}
		if (ptr.inputDelta.z != 0f && ptr.type != POINTER_INFO.POINTER_TYPE.RAY)
		{
			this.ScrollWheel(ptr.inputDelta.z);
		}
		if (this.Container != null)
		{
			ptr.callerIsControl = true;
			this.Container.OnInput(ptr);
		}
	}

	public void LateUpdate()
	{
		if (this.newItems.Count != 0)
		{
			if (this.itemsInserted || this.doItemEasing)
			{
				this.RepositionItems();
			}
			else
			{
				this.PositionNewItems();
			}
			this.itemsInserted = false;
			this.newItems.Clear();
		}
		this.timeDelta = Time.realtimeSinceStartup - this.lastTime;
		this.lastTime = Time.realtimeSinceStartup;
		this.inertiaLerpTime += this.timeDelta;
		if (this.cachedPos != base.transform.position || this.cachedRot != base.transform.rotation || this.cachedScale != base.transform.lossyScale)
		{
			this.cachedPos = base.transform.position;
			this.cachedRot = base.transform.rotation;
			this.cachedScale = base.transform.lossyScale;
			this.CalcClippingRect();
			if (this.clipWhenMoving)
			{
				this.ClipItems();
			}
		}
		if (this.itemEasers.Count > 0)
		{
			this.ClipItems();
		}
		if (!this.noTouch && this.inertiaLerpTime >= this.inertiaLerpInterval)
		{
			this.scrollInertia = Mathf.Lerp(this.scrollInertia, this.scrollDelta, this.lowPassFilterFactor);
			this.scrollDelta = 0f;
			this.inertiaLerpTime %= this.inertiaLerpInterval;
		}
		if (this.isScrolling && this.noTouch && !this.autoScrolling)
		{
			this.scrollDelta -= this.scrollDelta * this.scrollDecelCoef;
			if (this.scrollPos < 0f)
			{
				this.scrollPos -= this.scrollPos * 1f * (this.timeDelta / 0.166f);
				this.scrollDelta *= Mathf.Clamp01(1f + this.scrollPos / this.scrollMax);
			}
			else if (this.scrollPos > 1f)
			{
				this.scrollPos -= (this.scrollPos - 1f) * 1f * (this.timeDelta / 0.166f);
				this.scrollDelta *= Mathf.Clamp01(1f - (this.scrollPos - 1f) / this.scrollMax);
			}
			if (Mathf.Abs(this.scrollDelta) < 0.0001f)
			{
				this.scrollDelta = 0f;
				if (this.scrollPos > -0.0001f && this.scrollPos < 0.0001f)
				{
					this.scrollPos = Mathf.Clamp01(this.scrollPos);
				}
			}
			this.ScrollListTo_Internal(this.scrollPos + this.scrollDelta);
			if (this.scrollPos >= 0f && this.scrollPos <= 1.001f && this.scrollDelta == 0f)
			{
				this.isScrolling = false;
			}
		}
		else if (this.autoScrolling)
		{
			this.autoScrollTime += this.timeDelta;
			if (this.autoScrollTime >= this.autoScrollDuration)
			{
				this.autoScrolling = false;
				this.scrollPos = this.autoScrollPos;
			}
			else
			{
				this.scrollPos = this.autoScrollInterpolator(this.autoScrollTime, this.autoScrollStart, this.autoScrollDelta, this.autoScrollDuration);
			}
			this.ScrollListTo_Internal(this.scrollPos);
		}
	}

	protected void CalcSnapItem()
	{
		int num = 1;
		if (this.items.Count < 1)
		{
			return;
		}
		float num2;
		float num3;
		if (Mathf.Approximately(this.scrollDelta, 0f))
		{
			num2 = this.minSnapDuration;
			num3 = this.scrollPos;
		}
		else
		{
			num3 = this.scrollPos + this.scrollDelta / this.scrollDecelCoef;
			float num4 = Mathf.Abs(this.scrollDelta);
			num2 = Time.fixedDeltaTime * (this.scrollStopThresholdLog - Mathf.Log10(num4)) / Mathf.Log10((num4 - num4 * this.scrollDecelCoef) / num4);
			num2 = Mathf.Max(num2, this.minSnapDuration);
		}
		if (num3 >= 1f || num3 <= 0f)
		{
			if (num3 <= 0f)
			{
				this.ScrollToItem(0, num2);
			}
			else
			{
				this.ScrollToItem(this.items.Count - 1, num2);
			}
			return;
		}
		int num5 = (int)Mathf.Clamp((float)(this.items.Count - 1) * num3, 0f, (float)(this.items.Count - 1));
		if (this.orientation == UIScrollList.ORIENTATION.HORIZONTAL)
		{
			float num6 = ((this.direction != UIScrollList.DIRECTION.TtoB_LtoR) ? 1f : (-1f));
			IUIListObject iuilistObject = this.items[num5];
			float num7 = Mathf.Abs(num3 + num6 * iuilistObject.transform.localPosition.x / this.amtOfPlay);
			if (num5 + num < this.items.Count)
			{
				IUIListObject iuilistObject2 = this.items[num5 + num];
				float num8 = Mathf.Abs(num3 + num6 * iuilistObject2.transform.localPosition.x / this.amtOfPlay);
				if (num8 < num7)
				{
					num7 = num8;
					iuilistObject = iuilistObject2;
					num5 += num;
				}
				else
				{
					num = -1;
				}
			}
			else
			{
				num = -1;
			}
			int num9 = num5 + num;
			while (num9 > -1 && num9 < this.items.Count)
			{
				float num8 = Mathf.Abs(num3 + num6 * this.items[num9].transform.localPosition.x / this.amtOfPlay);
				if (num8 >= num7)
				{
					break;
				}
				num7 = num8;
				iuilistObject = this.items[num9];
				num9 += num;
			}
			this.ScrollToItem(iuilistObject, num2);
		}
		else
		{
			float num10 = ((this.direction != UIScrollList.DIRECTION.TtoB_LtoR) ? (-1f) : 1f);
			IUIListObject iuilistObject = this.items[num5];
			float num7 = Mathf.Abs(num3 + num10 * iuilistObject.transform.localPosition.y / this.amtOfPlay);
			if (num5 + num < this.items.Count)
			{
				IUIListObject iuilistObject2 = this.items[num5 + num];
				float num8 = Mathf.Abs(num3 + num10 * iuilistObject2.transform.localPosition.y / this.amtOfPlay);
				if (num8 < num7)
				{
					num7 = num8;
					iuilistObject = iuilistObject2;
					num5 += num;
				}
				else
				{
					num = -1;
				}
			}
			else
			{
				num = -1;
			}
			int num11 = num5 + num;
			while (num11 > -1 && num11 < this.items.Count)
			{
				float num8 = Mathf.Abs(num3 + num10 * this.items[num11].transform.localPosition.y / this.amtOfPlay);
				if (num8 >= num7)
				{
					break;
				}
				num7 = num8;
				iuilistObject = this.items[num11];
				num11 += num;
			}
			this.ScrollToItem(iuilistObject, num2);
		}
	}

	protected void AddItemsToContainer()
	{
		if (this.container == null)
		{
			return;
		}
		for (int i = 0; i < this.items.Count; i++)
		{
			this.container.AddChild(this.items[i].gameObject);
		}
	}

	protected void RemoveItemsFromContainer()
	{
		if (this.container == null)
		{
			return;
		}
		for (int i = 0; i < this.items.Count; i++)
		{
			this.container.RemoveChild(this.items[i].gameObject);
		}
	}

	public bool controlIsEnabled
	{
		get
		{
			return this.m_controlIsEnabled;
		}
		set
		{
			this.m_controlIsEnabled = value;
			for (int i = 0; i < this.items.Count; i++)
			{
				this.items[i].controlIsEnabled = value;
			}
		}
	}

	public virtual bool DetargetOnDisable
	{
		get
		{
			return this.DetargetOnDisable;
		}
		set
		{
			this.DetargetOnDisable = value;
		}
	}

	public IUIObject GetControl(ref POINTER_INFO ptr)
	{
		return this;
	}

	public virtual IUIContainer Container
	{
		get
		{
			return this.container;
		}
		set
		{
			if (value != this.container)
			{
				if (this.container != null)
				{
					this.RemoveItemsFromContainer();
				}
				this.container = value;
				this.AddItemsToContainer();
			}
			else
			{
				this.container = value;
			}
		}
	}

	public bool RequestContainership(IUIContainer cont)
	{
		Transform transform = base.transform.parent;
		Transform transform2 = ((Component)cont).transform;
		while (transform != null)
		{
			if (transform == transform2)
			{
				this.container = cont;
				return true;
			}
			if (transform.gameObject.GetComponent("IUIContainer") != null)
			{
				return false;
			}
			transform = transform.parent;
		}
		return false;
	}

	public bool GotFocus()
	{
		return false;
	}

	public void SetInputDelegate(EZInputDelegate del)
	{
		this.inputDelegate = del;
	}

	public void AddInputDelegate(EZInputDelegate del)
	{
		this.inputDelegate = (EZInputDelegate)Delegate.Combine(this.inputDelegate, del);
	}

	public void RemoveInputDelegate(EZInputDelegate del)
	{
		this.inputDelegate = (EZInputDelegate)Delegate.Remove(this.inputDelegate, del);
	}

	public void SetValueChangedDelegate(EZValueChangedDelegate del)
	{
		this.changeDelegate = del;
	}

	public void AddValueChangedDelegate(EZValueChangedDelegate del)
	{
		this.changeDelegate = (EZValueChangedDelegate)Delegate.Combine(this.changeDelegate, del);
	}

	public void RemoveValueChangedDelegate(EZValueChangedDelegate del)
	{
		this.changeDelegate = (EZValueChangedDelegate)Delegate.Remove(this.changeDelegate, del);
	}

	public void AddItemSnappedDelegate(UIScrollList.ItemSnappedDelegate del)
	{
		this.itemSnappedDel = (UIScrollList.ItemSnappedDelegate)Delegate.Combine(this.itemSnappedDel, del);
	}

	public void RemoveItemSnappedDelegate(UIScrollList.ItemSnappedDelegate del)
	{
		this.itemSnappedDel = (UIScrollList.ItemSnappedDelegate)Delegate.Remove(this.itemSnappedDel, del);
	}

	public object Data
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public bool IsDraggable
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public LayerMask DropMask
	{
		get
		{
			return -1;
		}
		set
		{
		}
	}

	public float DragOffset
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public EZAnimation.EASING_TYPE CancelDragEasing
	{
		get
		{
			return EZAnimation.EASING_TYPE.Default;
		}
		set
		{
		}
	}

	public float CancelDragDuration
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public bool IsDragging
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public GameObject DropTarget
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public bool DropHandled
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public void DragUpdatePosition(POINTER_INFO ptr)
	{
	}

	public void CancelDrag()
	{
	}

	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
		if (this.dragDropDelegate != null)
		{
			this.dragDropDelegate(parms);
		}
	}

	public void AddDragDropDelegate(EZDragDropDelegate del)
	{
		this.dragDropDelegate = (EZDragDropDelegate)Delegate.Combine(this.dragDropDelegate, del);
	}

	public void RemoveDragDropDelegate(EZDragDropDelegate del)
	{
		this.dragDropDelegate = (EZDragDropDelegate)Delegate.Remove(this.dragDropDelegate, del);
	}

	public void SetDragDropDelegate(EZDragDropDelegate del)
	{
		this.dragDropDelegate = del;
	}

	private void OnDrawGizmosSelected()
	{
		this.SetupCameraAndSizes();
		Vector3 vector = base.transform.position - base.transform.TransformDirection(Vector3.right * this.viewableAreaActual.x * 0.5f * base.transform.lossyScale.x) + base.transform.TransformDirection(Vector3.up * this.viewableAreaActual.y * 0.5f * base.transform.lossyScale.y);
		Vector3 vector2 = base.transform.position - base.transform.TransformDirection(Vector3.right * this.viewableAreaActual.x * 0.5f * base.transform.lossyScale.x) - base.transform.TransformDirection(Vector3.up * this.viewableAreaActual.y * 0.5f * base.transform.lossyScale.y);
		Vector3 vector3 = base.transform.position + base.transform.TransformDirection(Vector3.right * this.viewableAreaActual.x * 0.5f * base.transform.lossyScale.x) - base.transform.TransformDirection(Vector3.up * this.viewableAreaActual.y * 0.5f * base.transform.lossyScale.y);
		Vector3 vector4 = base.transform.position + base.transform.TransformDirection(Vector3.right * this.viewableAreaActual.x * 0.5f * base.transform.lossyScale.x) + base.transform.TransformDirection(Vector3.up * this.viewableAreaActual.y * 0.5f * base.transform.lossyScale.y);
		Gizmos.color = new Color(1f, 0f, 0.5f, 1f);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, vector);
	}

	public static UIScrollList Create(string name, Vector3 pos)
	{
		return (UIScrollList)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UIScrollList));
	}

	public static UIScrollList Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UIScrollList)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UIScrollList));
	}

	GameObject IUIObject.gameObject
	{
		get { return base.gameObject; }
	}

	Transform IUIObject.transform
	{
		get { return base.transform; }
	}

	string IUIObject.name
	{
		get { return base.name; }
	}

	protected const float reboundSpeed = 1f;

	protected const float overscrollAllowance = 0.5f;

	protected const float lowPassKernelWidthInSeconds = 0.045f;

	protected const float backgroundColliderOffset = 0.01f;

	private const float scrollStopThreshold = 0.0001f;

	public bool touchScroll = true;

	public float scrollWheelFactor = 100f;

	public float scrollDecelCoef = 0.04f;

	public bool snap;

	public float minSnapDuration = 1f;

	public EZAnimation.EASING_TYPE snapEasing = EZAnimation.EASING_TYPE.ExponentialOut;

	public UISlider slider;

	public UIScrollList.ORIENTATION orientation;

	public UIScrollList.DIRECTION direction;

	public UIScrollList.ALIGNMENT alignment = UIScrollList.ALIGNMENT.CENTER;

	public Vector2 viewableArea;

	protected Vector2 viewableAreaActual;

	public bool unitsInPixels;

	public Camera renderCamera;

	protected Rect3D clientClippingRect;

	public float itemSpacing;

	protected float itemSpacingActual;

	public bool spacingAtEnds = true;

	public float extraEndSpacing;

	protected float extraEndSpacingActual;

	public bool activateWhenAdding = true;

	public bool clipContents = true;

	public bool clipWhenMoving;

	public bool positionItemsImmediately = true;

	public float dragThreshold = float.NaN;

	public GameObject[] sceneItems = new GameObject[0];

	public PrefabListItem[] prefabItems = new PrefabListItem[0];

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvokeOnSelect;

	public SpriteManagerEZ manager;

	public bool detargetOnDisable;

	public EZAnimation.EASING_TYPE positionEasing = EZAnimation.EASING_TYPE.ExponentialOut;

	public float positionEaseDuration = 0.5f;

	public float positionEaseDelay;

	public bool blockInputWhileEasing = true;

	protected bool doItemEasing;

	protected bool doPosEasing;

	protected List<EZAnimation> itemEasers = new List<EZAnimation>();

	protected EZAnimation scrollPosAnim;

	[HideInInspector]
	public bool repositionOnEnable = true;

	protected float contentExtents;

	protected IUIListObject selectedItem;

	protected IUIObject lastClickedControl;

	protected float scrollPos;

	protected GameObject mover;

	protected List<IUIListObject> items = new List<IUIListObject>();

	protected List<IUIListObject> visibleItems = new List<IUIListObject>();

	protected List<IUIListObject> tempVisItems = new List<IUIListObject>();

	protected bool m_controlIsEnabled = true;

	protected IUIContainer container;

	protected EZInputDelegate inputDelegate;

	protected EZValueChangedDelegate changeDelegate;

	protected UIScrollList.ItemSnappedDelegate itemSnappedDel;

	protected Vector3 cachedPos;

	protected Quaternion cachedRot;

	protected Vector3 cachedScale;

	protected bool m_started;

	protected bool m_awake;

	protected List<IUIListObject> newItems = new List<IUIListObject>();

	protected bool itemsInserted;

	protected bool isScrolling;

	protected bool noTouch = true;

	protected float lowPassFilterFactor;

	private float scrollInertia;

	protected float scrollMax;

	private float scrollDelta;

	private float scrollStopThresholdLog = Mathf.Log10(0.0001f);

	private float lastTime;

	private float timeDelta;

	private float inertiaLerpInterval = 0.06f;

	private float inertiaLerpTime;

	private float amtOfPlay;

	private float autoScrollDuration;

	private float autoScrollStart;

	private float autoScrollPos;

	private float autoScrollDelta;

	private float autoScrollTime;

	private bool autoScrolling;

	private bool listMoved;

	private EZAnimation.Interpolator autoScrollInterpolator;

	private IUIListObject snappedItem;

	private float localUnitsPerPixel;

	protected EZDragDropDelegate dragDropDelegate;

	public enum ORIENTATION
	{
		HORIZONTAL,
		VERTICAL
	}

	public enum DIRECTION
	{
		TtoB_LtoR,
		BtoT_RtoL
	}

	public enum ALIGNMENT
	{
		LEFT_TOP,
		CENTER,
		RIGHT_BOTTOM
	}

	protected delegate float ItemAlignmentDel(IUIListObject item);

	protected delegate bool SnapCoordProc(float val);

	public delegate void ItemSnappedDelegate(IUIListObject item);
}

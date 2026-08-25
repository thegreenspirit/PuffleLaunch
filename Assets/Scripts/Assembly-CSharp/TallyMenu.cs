using System;
using System.Collections.Generic;
using UnityEngine;

public class TallyMenu : BaseGUI
{
	public TallyMenu(GameObject aRefObj)
		: base(aRefObj)
	{
		GameObject.Find("ProgressBar").GetComponent<ProgressBar>().progressText.Show = false;
		this.mLastFrameTimestamp = Time.realtimeSinceStartup;
	}

	protected override void CreateLayouts()
	{
		this.mTimeTrialUnlocked = GameManager.HasCollectedAllRings(GameManager.Instance.CurrentWorld);
		float num = 0f;
		if (!this.mTimeTrialUnlocked)
		{
			if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
			{
				num = 0.06f;
			}
			else
			{
				num = 0.04f;
			}
		}
		base.TextureData = new GUIDefines.TextureData[]
		{
			new GUIDefines.TextureData
			{
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/TallyMenu/end-level_popup"
				},
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.1671875f,
					topRatio = 0.004062501f,
					widthRatio = 0.6604167f,
					heightRatio = 0.925f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 23f,
						topOffset = 52f
					}
				}
			},
			new GUIDefines.TextureData
			{
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/TallyMenu/total_puffle-o_bg"
				},
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.1635416f,
					topRatio = 0.339125f + num,
					widthRatio = 0.67083f,
					heightRatio = 0.24375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 23f,
						topOffset = 5f
					}
				}
			},
			new GUIDefines.TextureData
			{
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/TallyMenu/total_time_bg"
				},
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.1635416f,
					topRatio = 0.4625625f,
					widthRatio = 0.67083f,
					heightRatio = 0.228125f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 23f,
						topOffset = -10f
					}
				},
				invisible = !this.mTimeTrialUnlocked
			},
			new GUIDefines.TextureData
			{
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/TallyMenu/mini-puffle-o"
				},
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.478125f,
					topRatio = 0.4063125f + num,
					widthRatio = 0.05f,
					heightRatio = 0.1078125f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = -4f,
						topOffset = -4f
					}
				}
			},
			new GUIDefines.TextureData
			{
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/TallyMenu/ProgressRing/progress_bar_0"
				},
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.4354166f,
					topRatio = 0.111f,
					widthRatio = 0.1333334f,
					heightRatio = 0.246875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 3f,
						topOffset = 27f
					}
				}
			},
			new GUIDefines.TextureData
			{
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/TallyMenu/clock"
				},
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.4791667f,
					topRatio = 0.536f,
					widthRatio = 0.04791667f,
					heightRatio = 0.071875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = -4f,
						topOffset = -17f
					}
				},
				invisible = !this.mTimeTrialUnlocked
			}
		};
		string text = string.Format("0/{0}", this.ProgressBar.TotalPuffleOs);
		string timeFormatedString = GameManager.GetTimeFormatedString(GameManager.smCurrentTimeCount);
		base.LabelData = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				content = new GUIDefines.ContentInfo
				{
					text = text
				},
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.2604167f,
					topRatio = 0.4344375f + num,
					widthRatio = 0.203125f,
					heightRatio = 0.0546875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 8f,
						topOffset = -5f
					}
				},
				style = new GUIDefines.StyleInfo
				{
					styleName = "TallyScreenCounter"
				}
			},
			new GUIDefines.LabelData
			{
				content = new GUIDefines.ContentInfo
				{
					text = timeFormatedString
				},
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.33125f,
					topRatio = 0.54225f,
					widthRatio = 0.13125f,
					heightRatio = 0.0546875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 10f,
						topOffset = -16f
					}
				},
				style = new GUIDefines.StyleInfo
				{
					styleName = "TallyScreenCounter"
				},
				invisible = !this.mTimeTrialUnlocked
			},
			new GUIDefines.LabelData
			{
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Total"
				},
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.5354167f,
					topRatio = 0.4344375f + num,
					widthRatio = 0.13125f,
					heightRatio = 0.0546875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 8f,
						topOffset = -5f
					}
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eMedium,
					customFontType = GUIDefines.FontType.eInGame,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleLeft
				}
			},
			new GUIDefines.LabelData
			{
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Total"
				},
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.5354167f,
					topRatio = 0.54225f,
					widthRatio = 0.13125f,
					heightRatio = 0.0546875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 8f,
						topOffset = -16f
					}
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eMedium,
					customFontType = GUIDefines.FontType.eInGame,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleLeft
				},
				invisible = !this.mTimeTrialUnlocked
			}
		};
		base.ButtonData = new GUIDefines.ButtonData[]
		{
			new GUIDefines.ButtonData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.2270833f,
					topRatio = 0.645375f,
					widthRatio = 0.128125f,
					heightRatio = 0.1703126f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 18f,
						topOffset = -31f
					}
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/TallyMenu/end-level_menu_button"
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/TallyMenu/end-level_menu_button_pressed"
					}
				}
			},
			new GUIDefines.ButtonData
			{
				buttonId = 2,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.3687499f,
					topRatio = 0.645375f,
					widthRatio = 0.128125f,
					heightRatio = 0.1703126f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 8f,
						topOffset = -31f
					}
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/TallyMenu/end-level_replay_button"
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/TallyMenu/end-level_replay_button_pressed"
					}
				}
			},
			new GUIDefines.ButtonData
			{
				buttonId = 1,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.5072917f,
					topRatio = 0.645375f,
					widthRatio = 0.2625f,
					heightRatio = 0.16875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = -31f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_NEXT"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/TallyMenu/end-level_popup_nextlevel-button"
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/TallyMenu/end-level_popup_nextlevel-button_pressed"
					},
					customFontSize = GUIDefines.FontSize.eMedium,
					customFontType = GUIDefines.FontType.eInGame,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleLeft,
					customPadding = new GUIDefines.Vector2Info
					{
						xRatio = 0.03645833f
					}
				}
			}
		};
	}

	public override void Draw()
	{
		if (!this.CanDraw())
		{
			return;
		}
		if (GameFlowManager.Instance.m_DoWindowBack && !GameFlowManager.Instance.GUIManager.IsRateMyAppPopupShowing)
		{
			base.SelectedButton = 0;
			this.OnButtonSelect();
			GameFlowManager.Instance.m_DoWindowBack = false;
		}
		this.mDeltaTime = Time.realtimeSinceStartup - this.mLastFrameTimestamp;
		this.mLastFrameTimestamp = Time.realtimeSinceStartup;
		this.UpdateProgressTexture();
		if (this.mRotateArrows)
		{
			this.mProgressArrows.Rotate(Vector3.forward, -130f * Time.deltaTime, Space.World);
		}
		if (this.mCurrentState == TallyMenu.State.eState_WaitingForNextState)
		{
			this.mWaitTime -= this.mDeltaTime;
			if (this.mWaitTime < 0f)
			{
				this.mCurrentState = this.mNextState;
				this.mNextState = TallyMenu.State.eState_None;
			}
		}
		else
		{
			this.UpdateRingValue();
		}
		base.Draw();
		this.BlockControl(false);
	}

	protected override void OnButtonSelect()
	{
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		GameFlowManager.Instance.GUIManager.ShowTallyMenu(false);
		Camera.main.GetComponent<Light>().enabled = false;
		GameManager.smCurrentLevelRingCount = 0;
		switch (base.SelectedButton)
		{
		case 0:
			global::UnityEngine.Object.DestroyImmediate(GameObject.Find("TallyMenuCoinTransfer"));
			if (LevelSelect.SelectedLevel - 1 == 23)
			{
				GameFlowManager.Instance.LoadScene("EndCinematic", false);
			}
			else
			{
				GameFlowManager.Instance.LoadScene("LevelSelect", false);
			}
			break;
		case 1:
			global::UnityEngine.Object.DestroyImmediate(GameObject.Find("TallyMenuCoinTransfer"));
			if (LevelSelect.SelectedLevel - 1 == 23)
			{
				LevelSelect.SelectedLevel++;
				GameFlowManager.Instance.LoadScene("EndCinematic", false);
			}
			else if (LevelSelect.SelectedLevel - 1 == 59)
			{
				GameFlowManager.Instance.LoadScene("LevelSelect", false);
			}
			else
			{
				LevelSelect.SelectedLevel++;
				if (LevelSelect.SelectedLevel - 1 <= 11)
				{
					GameManager.Instance.CurrentWorld = GameManager.World.eWorld_BlueSky;
				}
				else if (LevelSelect.SelectedLevel - 1 <= 23)
				{
					GameManager.Instance.CurrentWorld = GameManager.World.eWorld_SodaSunset;
				}
				else if (LevelSelect.SelectedLevel - 1 <= 59)
				{
					GameManager.Instance.CurrentWorld = GameManager.World.eWorld_BonusWorld;
				}
				GameManager.Instance.StartLevel((GameManager.Level)(LevelSelect.SelectedLevel - 1));
				GameFlowManager.Instance.LoadScene("Gameplay", true);
			}
			break;
		case 2:
			global::UnityEngine.Object.DestroyImmediate(GameObject.Find("TallyMenuCoinTransfer"));
			GameManager.Instance.StartLevel((GameManager.Level)(LevelSelect.SelectedLevel - 1));
			GameFlowManager.Instance.LoadScene("Gameplay", true);
			break;
		}
		this.ResetButton();
	}

	private void UpdateProgressTexture()
	{
		if (this.mCurrentIndex < this.mProgressRingTextures.Count)
		{
			this.mTextureTimer += this.mDeltaTime;
			if (this.mTextureTimer > 0.125f)
			{
				this.mTextureTimer = 0f;
				this.mCurrentIndex++;
				if (this.mProgressRingTextures.Count == this.mCurrentIndex)
				{
					if (this.mFinishedRingTextures.Count > 0)
					{
						base.TextureData[4].icon.image = this.mFinishedRingTextures[0];
						this.mFinishedIndex = 0;
					}
				}
				else
				{
					base.TextureData[4].icon.image = this.mProgressRingTextures[this.mCurrentIndex];
				}
			}
		}
		if (this.mFinishedRingTextures.Count > 0)
		{
			this.mTextureTimer += this.mDeltaTime;
			if (this.mTextureTimer > 0.25f)
			{
				this.mFinishedIndex = (this.mFinishedIndex + 1) % 10;
				base.TextureData[4].icon.image = this.mFinishedRingTextures[this.mFinishedIndex];
			}
		}
	}

	public void SetCoinTransfer3DObject(GameObject aCoinTransferObject)
	{
		this.mCoinTransferObject = aCoinTransferObject;
		this.mBlueButtonContainer = this.mCoinTransferObject.transform.Find("BlueButtonContainer");
		this.mBlueButton = this.mBlueButtonContainer.transform.Find("BlueButton");
		this.mCoinSpawner = this.mBlueButton.GetComponent<CoinSpawner>();
		this.mProgressArrows = this.mBlueButtonContainer.transform.Find("Arrows");
		Camera.main.GetComponent<Light>().enabled = true;
		this.mProfileCoinCount = this.mBlueButton.Find("CoinText").GetComponent<TextMesh>();
		this.mProfileCoinCountDropShadow = this.mBlueButton.Find("CoinText").Find("CoinTextDropShadow").GetComponent<TextMesh>();
		this.mBlueButton.Find("TransferText").GetComponent<Renderer>().enabled = false;
		this.mBlueButton.Find("TransferText").Find("TransferTextDropShadow").GetComponent<Renderer>().enabled = false;
		this.mBlueButton.GetComponent<Button3DPressStateController>().Enabled = false;
		this.mBlueButton.GetComponent<Button3DPressStateController>().onReleased += this.TallyMenuBlueButton_onReleased;
		this.mBlueButton.Find("ErrorButton").GetComponent<Button3DPressStateController>().onReleased += this.TallyMenuErrorButton_onReleased;
		this.mBlueButton.Find("ErrorButton").gameObject.active = false;
		this.mBlueButton.Find("TransferText").GetComponent<TextMesh>().text = LocalizationManager.Instance.GetString("TXT_TapToTransfer");
		this.mBlueButton.Find("TransferText").Find("TransferTextDropShadow").GetComponent<TextMesh>()
			.text = LocalizationManager.Instance.GetString("TXT_TapToTransfer");
		this.mBlueButton.Find("CoinsTransferredText").GetComponent<TextMesh>().text = LocalizationManager.Instance.GetString("TXT_CoinsTransferred");
		this.mBlueButton.Find("CoinsTransferredText").Find("CoinsTransferredTextDropShadow").GetComponent<TextMesh>()
			.text = LocalizationManager.Instance.GetString("TXT_CoinsTransferred");
		this.InitGUITextures();
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			Vector3 localScale = this.mBlueButton.localScale;
			localScale.x *= 0.8621f;
			this.mBlueButton.localScale = localScale;
			Vector3 vector = this.mBlueButtonContainer.Find("Penguin Icon").transform.position;
			vector.x -= 1.873847f;
			this.mBlueButtonContainer.Find("Penguin Icon").transform.position = vector;
			vector = this.mProgressArrows.position;
			vector.x -= 1.873847f;
			this.mProgressArrows.position = vector;
		}
		this.mCurrentState = TallyMenu.State.eState_SlideInAnim;
	}

	private void InitGUITextures()
	{
		float num = (float)GameManager.smCurrentLevelRingCount / (float)this.ProgressBar.TotalPuffleOs;
		this.mMaxIndex = Mathf.FloorToInt(num * 12f) + 1;
		this.mProgressRingTextures = new List<Texture>(this.mMaxIndex);
		for (int i = 0; i < this.mMaxIndex; i++)
		{
			GUIDefines.TextureInfo textureInfo = new GUIDefines.TextureInfo
			{
				name = "GUI/TallyMenu/ProgressRing/progress_bar_" + i.ToString()
			};
			textureInfo.Init();
			this.mProgressRingTextures.Add(textureInfo.image);
		}
		this.mFinishedRingTextures = new List<Texture>();
		if ((double)num == 1.0)
		{
			for (int j = 1; j < 11; j++)
			{
				GUIDefines.TextureInfo textureInfo2 = new GUIDefines.TextureInfo
				{
					name = "GUI/TallyMenu/ProgressRing/Finished/finished" + j.ToString()
				};
				textureInfo2.Init();
				this.mFinishedRingTextures.Add(textureInfo2.image);
			}
		}
	}

	private void TallyMenuBlueButton_onReleased(object sender, EventArgs e)
	{
		if (!NetManager.Instance.IsPlayerLoggedIn() && !GameFlowManager.Instance.GUIManager.IsUpsellPopupShowing)
		{
			GameFlowManager.Instance.GUIManager.RegisterLoginBackTraceScene();
			GameFlowManager.Instance.GUIManager.ShowLoginPopup(true);
			this.mBlueButton.GetComponent<Button3DPressStateController>().Enabled = false;
		}
	}

	private void TallyMenuErrorButton_onReleased(object sender, EventArgs e)
	{
		NetManager.Instance.ShowError(NetManager.Instance.GetLastErrorMsg(NetManager.Request.eCoinTransfer), false);
	}

	private void UpdateRingValue()
	{
		switch (this.mCurrentState)
		{
		case TallyMenu.State.eState_SlideInAnim:
			this.PlaySlideInAnim();
			break;
		case TallyMenu.State.eState_SlideOutAnim:
			this.PlaySlideOutAnim();
			break;
		case TallyMenu.State.eState_CountingRingsAndCoins:
			this.CountingRings();
			this.CountingCoins();
			this.ShowRateThisApp();
			break;
		case TallyMenu.State.eState_TransferCoinsFX:
			this.TransferCoinsEffects();
			break;
		case TallyMenu.State.eState_HandlingTransferError:
			this.HandleTransferError();
			break;
		case TallyMenu.State.eState_WaitingForSlideInAnimDone:
			this.WaitForSlideInAnimDone();
			break;
		case TallyMenu.State.eState_WaitingForLogin:
			this.WaitForLogin();
			break;
		case TallyMenu.State.eState_WaitingForTransfer:
			this.WaitForTransfer();
			break;
		}
	}

	private void PlaySlideInAnim()
	{
		this.mBlueButtonContainer.GetComponent<Animation>().Play();
		this.mCurrentState = TallyMenu.State.eState_WaitingForSlideInAnimDone;
	}

	private void PlaySlideOutAnim()
	{
		this.mRotateArrows = false;
		if (!NetManager.Instance.HasCoinTransferError())
		{
			this.mBlueButtonContainer.GetComponent<Animation>()["TallyMenuButtonSlideIn"].speed = -1f;
			this.mBlueButtonContainer.GetComponent<Animation>()["TallyMenuButtonSlideIn"].time = this.mBlueButtonContainer.GetComponent<Animation>()["TallyMenuButtonSlideIn"].length;
			this.mBlueButtonContainer.GetComponent<Animation>().Play("TallyMenuButtonSlideIn");
		}
		this.mCurrentState = TallyMenu.State.eState_Idle;
	}

	private void WaitForSlideInAnimDone()
	{
		if (!this.mBlueButtonContainer.GetComponent<Animation>().isPlaying)
		{
			int oldTotalCoins = this.GetOldTotalCoins();
			this.mProfileCoinCount.text = oldTotalCoins.ToString();
			this.mProfileCoinCountDropShadow.text = oldTotalCoins.ToString();
			this.mProfileCoinCount.GetComponent<Renderer>().enabled = true;
			this.mProfileCoinCountDropShadow.GetComponent<Renderer>().enabled = true;
			this.mCountingRingsCompleted = false;
			this.mCurrentState = TallyMenu.State.eState_WaitingForNextState;
			this.mNextState = TallyMenu.State.eState_CountingRingsAndCoins;
		}
	}

	private void CountingRings()
	{
		this.mRingTimer += this.mDeltaTime;
		float num = this.mRingTimer / (0.125f * (float)this.mMaxIndex);
		num = Mathf.Min(num, 1f);
		num *= (float)GameManager.smCurrentLevelRingCount;
		base.LabelData[0].content.text = string.Format("{0}/{1}", Mathf.FloorToInt(num).ToString(), this.ProgressBar.TotalPuffleOs);
		if (num == (float)GameManager.smCurrentLevelRingCount)
		{
			this.mCountingRingsCompleted = true;
			this.mRingTimer = 0f;
		}
	}

	private void CountingCoins()
	{
		this.mCoinTimer += this.mDeltaTime;
		float num = 1f - this.mCoinTimer / (0.125f * (float)this.mMaxIndex);
		num = Mathf.Max(num, 0f);
		num *= (float)GameManager.smCurrentLevelRingCount;
		int oldTotalCoins = this.GetOldTotalCoins();
		num = this.mCoinTimer / (0.125f * (float)this.mMaxIndex);
		num = Mathf.Min(num, 1f);
		num *= (float)GameManager.smCurrentLevelRingCount;
		num += (float)oldTotalCoins;
		this.mProfileCoinCount.text = string.Format("{0}", Mathf.FloorToInt(num).ToString());
		this.mProfileCoinCountDropShadow.text = string.Format("{0}", Mathf.FloorToInt(num).ToString());
		if (this.mCountingRingsCompleted && num == (float)(GameManager.smCurrentLevelRingCount + oldTotalCoins))
		{
			this.mCurrentState = TallyMenu.State.eState_WaitingForNextState;
			this.mWaitTime = 0.5f;
			this.mCoinTimer = 0f;
			if (NetManager.Instance.IsPlayerLoggedIn())
			{
				if (NetManager.Instance.IsAnyRequestInProgess())
				{
					this.mRotateArrows = true;
					this.mCurrentState = TallyMenu.State.eState_WaitingForTransfer;
					this.mNextState = TallyMenu.State.eState_TransferCoinsFX;
				}
				else if (NetManager.Instance.HasCoinTransferError() && !NetManager.Instance.HasReachedCoinTransferLimitError())
				{
					this.ShowErrorButton();
				}
				else
				{
					this.mNextState = TallyMenu.State.eState_TransferCoinsFX;
				}
			}
			else
			{
				this.mBlueButton.GetComponent<Button3DPressStateController>().Enabled = true;
				this.mBlueButton.Find("TransferText").GetComponent<Renderer>().enabled = true;
				this.mBlueButton.Find("TransferText").Find("TransferTextDropShadow").GetComponent<Renderer>().enabled = true;
				this.mNextState = TallyMenu.State.eState_WaitingForLogin;
			}
		}
	}

	private void TransferCoinsEffects()
	{
		if (!this.mCoinSpawner.enabled)
		{
			this.mCoinSpawner.enabled = true;
			if (GameManager.smCurrentLevelRingCount + ProfileManager.Instance.CurrentProfile.TotalCoins != 0 && (!NetManager.Instance.HasCoinTransferError() || NetManager.Instance.HasReachedCoinTransferLimitError()))
			{
				this.mRotateArrows = true;
				if (!NetManager.Instance.HasReachedCoinTransferLimitError())
				{
					this.mCoinSpawner.SpawnRing();
				}
			}
		}
		this.mCoinTimer += this.mDeltaTime;
		int num = Mathf.Max(GameManager.Instance.CoinsBeforeTransfer - ProfileManager.Instance.CurrentProfile.TotalCoins, 0);
		float num2 = (float)num / 100f * 1f;
		float num3 = this.mCoinTimer / num2;
		num3 = Mathf.Min(num3, 1f);
		float num4 = (float)GameManager.Instance.CoinsBeforeTransfer - (float)num * num3;
		this.mProfileCoinCount.text = string.Format("{0}", Mathf.FloorToInt(num4).ToString());
		this.mProfileCoinCountDropShadow.text = string.Format("{0}", Mathf.FloorToInt(num4).ToString());
		if (num4 == (float)ProfileManager.Instance.CurrentProfile.TotalCoins)
		{
			this.mNextState = TallyMenu.State.eState_SlideOutAnim;
			this.mCurrentState = TallyMenu.State.eState_WaitingForNextState;
			this.mWaitTime = 2f;
			this.mCoinSpawner.enabled = false;
			if (!NetManager.Instance.HasReachedCoinTransferLimitError())
			{
				this.mProfileCoinCount.GetComponent<Renderer>().enabled = false;
				this.mProfileCoinCountDropShadow.GetComponent<Renderer>().enabled = false;
			}
			if (!NetManager.Instance.HasCoinTransferError())
			{
				this.mBlueButton.Find("CoinsTransferredText").GetComponent<Renderer>().enabled = true;
				this.mBlueButton.Find("CoinsTransferredText").Find("CoinsTransferredTextDropShadow").GetComponent<Renderer>().enabled = true;
				this.mBlueButton.Find("CoinsTransferredText").GetComponent<Animation>().Play();
			}
			else if (NetManager.Instance.HasReachedCoinTransferLimitError())
			{
				this.ShowErrorButton();
			}
		}
	}

	private void HandleTransferError()
	{
	}

	private void WaitForLogin()
	{
		if (NetManager.Instance.IsPlayerLoggedIn())
		{
			NetManager.Instance.TransferCoins(ProfileManager.Instance.CurrentProfile.TotalCoins, new BaseNetRequest.RequestCompleteCB(this.TransferCallback), true);
			this.mBlueButton.Find("TransferText").GetComponent<Renderer>().enabled = !NetManager.Instance.IsPlayerLoggedIn();
			this.mBlueButton.Find("TransferText").Find("TransferTextDropShadow").GetComponent<Renderer>().enabled = !NetManager.Instance.IsPlayerLoggedIn();
			this.mCurrentState = TallyMenu.State.eState_Idle;
			this.mNextState = TallyMenu.State.eState_TransferCoinsFX;
		}
		else
		{
			this.mBlueButton.GetComponent<Button3DPressStateController>().Enabled = true;
		}
	}

	private void WaitForTransfer()
	{
		if (!NetManager.Instance.IsAnyRequestInProgess() && NetManager.Instance.HasCoinTransferCompleted())
		{
			if (NetManager.Instance.HasCoinTransferError())
			{
				this.ShowErrorButton();
			}
			else
			{
				this.mCurrentState = this.mNextState;
			}
		}
	}

	private void TransferCallback(bool aSuccess)
	{
		this.mProfileCoinCount.text = ProfileManager.Instance.CurrentProfile.TotalCoins.ToString();
		if (aSuccess)
		{
			this.mCurrentState = TallyMenu.State.eState_TransferCoinsFX;
		}
		else
		{
			this.ShowErrorButton();
		}
	}

	private void ShowErrorButton()
	{
		this.mRotateArrows = false;
		this.mCurrentState = TallyMenu.State.eState_HandlingTransferError;
		this.mBlueButton.Find("ErrorButton").gameObject.active = true;
		this.mBlueButton.Find("ErrorButton").GetComponent<Animation>().Play();
		this.mBlueButton.Find("ErrorButton").GetComponent<Renderer>().enabled = true;
		this.mBlueButton.Find("ErrorButton").GetComponent<Button3DPressStateController>().Enabled = true;
		this.mBlueButton.Find("ErrorButton").GetComponent<ErrorButtonController>().ErrorHappened = true;
	}

	private int GetOldTotalCoins()
	{
		int num = Mathf.Max(GameManager.Instance.CoinsBeforeTransfer, ProfileManager.Instance.CurrentProfile.TotalCoins);
		return Mathf.Max(num - GameManager.smCurrentLevelRingCount, 0);
	}

	public void SetBackground3DObject(GameObject aBackgroundObj)
	{
		this.mPopupBgObject = aBackgroundObj;
		this.mTotalPuffleOBgTransform = this.mPopupBgObject.transform.Find("TotalPuffleOBg");
		this.mTotalTimeBgTransform = this.mPopupBgObject.transform.Find("TotalTimeBg");
		if (!this.mTimeTrialUnlocked)
		{
			Vector3 localPosition = this.mTotalPuffleOBgTransform.localPosition;
			localPosition.z += 0.45f;
			this.mTotalPuffleOBgTransform.localPosition = localPosition;
			this.mTotalTimeBgTransform.GetComponent<MeshRenderer>().enabled = false;
		}
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			Vector3 localScale = this.mPopupBgObject.transform.localScale;
			localScale.x *= 0.8621f;
			localScale.z *= 0.84006f;
			this.mPopupBgObject.transform.localScale = localScale;
			Vector3 position = this.mPopupBgObject.transform.position;
			position.y += 0.4452588f;
			this.mPopupBgObject.transform.position = position;
		}
	}

	public void SetNewBestPuffleOText3DObject(GameObject aNewBestPuffleOTextObj)
	{
		this.mNewBestPuffleOTextObject = aNewBestPuffleOTextObj;
		this.mNewBestPuffleOText = this.mNewBestPuffleOTextObject.GetComponent<AnimatedText>();
		this.mNewBestPuffleOTextMesh = this.mNewBestPuffleOTextObject.GetComponent<TextMesh>();
		if (this.mNewBestPuffleOText.textShadow != null)
		{
			this.mNewBestPuffleOTextMeshShadow = this.mNewBestPuffleOText.textShadow.GetComponent<TextMesh>();
		}
		this.mNewBestPuffleOTextMesh.text = LocalizationManager.Instance.GetString("TXT_NewRecord");
		if (this.mNewBestPuffleOTextMeshShadow != null)
		{
			this.mNewBestPuffleOTextMeshShadow.text = this.mNewBestPuffleOTextMesh.text;
		}
		if (!this.mTimeTrialUnlocked)
		{
			Vector3 position = this.mNewBestPuffleOTextObject.transform.position;
			position.y -= 1.2662555f;
			this.mNewBestPuffleOTextObject.transform.position = position;
		}
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			this.mNewBestPuffleOText.transform.localScale = this.IpadTextScale(this.mNewBestPuffleOText.transform.localScale);
			this.mNewBestPuffleOText.transform.position = this.IpadTextOffset(this.mNewBestPuffleOText.transform.position);
		}
	}

	public void SetNewBestTimeText3DObject(GameObject aNewBestTimeTextObj)
	{
		this.mNewBestTimeTextObject = aNewBestTimeTextObj;
		this.mNewBestTimeText = this.mNewBestTimeTextObject.GetComponent<AnimatedText>();
		this.mNewBestTimeTextMesh = this.mNewBestTimeTextObject.GetComponent<TextMesh>();
		if (this.mNewBestTimeText.textShadow != null)
		{
			this.mNewBestTimeTextMeshShadow = this.mNewBestTimeText.textShadow.GetComponent<TextMesh>();
		}
		this.mNewBestTimeTextMesh.text = LocalizationManager.Instance.GetString("TXT_NewRecord");
		if (this.mNewBestTimeTextMeshShadow != null)
		{
			this.mNewBestTimeTextMeshShadow.text = this.mNewBestTimeTextMesh.text;
		}
		Vector3 position = this.mNewBestTimeTextObject.transform.position;
		position.y += this.mTotalTimeBgTransform.position.y - this.mTotalPuffleOBgTransform.position.y;
		this.mNewBestTimeTextObject.transform.position = position;
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			this.mNewBestTimeTextObject.transform.localScale = this.IpadTextScale(this.mNewBestTimeTextObject.transform.localScale);
			this.mNewBestTimeTextObject.transform.position = this.IpadTextOffset(this.mNewBestTimeTextObject.transform.position);
		}
	}

	private Vector3 IpadTextScale(Vector3 aScale)
	{
		Vector3 vector = aScale;
		vector.x *= 0.8621f;
		vector.y *= 0.84006f;
		return vector;
	}

	private Vector3 IpadTextOffset(Vector3 aPos)
	{
		Vector3 vector = aPos;
		vector.x += 1.123847f;
		vector.y += 0.4452588f;
		return vector;
	}

	private void ShowRateThisApp()
	{
		if (GameManager.smCurrentLevel == GameManager.Level.eLevel_5 && PlayerPrefs.GetInt("RateMyApp") != 10 && PlayerPrefs.GetInt("RateMyAppAtLevel5") > 0)
		{
			PlayerPrefs.SetInt("RateMyAppAtLevel5", 0);
			PlayerPrefs.SetInt("RateMyApp", 0);
			GameFlowManager.Instance.GUIManager.ShowRateMyAppPopup(true);
		}
	}

	private ProgressBar ProgressBar
	{
		get
		{
			if (this.mProgressBar == null)
			{
				this.mProgressBar = GameObject.Find("Main Camera").transform.Find("ProgressBar").GetComponent<ProgressBar>();
			}
			return this.mProgressBar;
		}
	}

	private const float kCoinTransferDurationPer100Coins = 1f;

	private const float k3DObjectIpadXScale = 0.8621f;

	private const float k3DObjectIpadYScale = 0.84006f;

	private const float k3DObjectIpadXOffset = 1.123847f;

	private const float k3DObjectIpadYOffset = 0.4452588f;

	private const float k3DObjectIpadLockedTimeTrialYOffset = 0.45f;

	private const float mTimePerTexture = 0.125f;

	private List<Texture> mProgressRingTextures;

	private List<Texture> mFinishedRingTextures;

	private float mTextureTimer;

	private float mRingTimer;

	private float mCoinTimer;

	private int mMaxIndex;

	private int mCurrentIndex;

	private int mFinishedIndex;

	private TallyMenu.State mCurrentState = TallyMenu.State.eState_Idle;

	private TallyMenu.State mNextState = TallyMenu.State.eState_None;

	private float mDeltaTime;

	private float mLastFrameTimestamp;

	private GameObject mCoinTransferObject;

	private CoinSpawner mCoinSpawner;

	private Transform mBlueButtonContainer;

	private Transform mBlueButton;

	private TextMesh mProfileCoinCount;

	private TextMesh mProfileCoinCountDropShadow;

	private ProgressBar mProgressBar;

	private Transform mProgressArrows;

	private GameObject mNewBestPuffleOTextObject;

	private AnimatedText mNewBestPuffleOText;

	private TextMesh mNewBestPuffleOTextMesh;

	private TextMesh mNewBestPuffleOTextMeshShadow;

	private GameObject mNewBestTimeTextObject;

	private AnimatedText mNewBestTimeText;

	private TextMesh mNewBestTimeTextMesh;

	private TextMesh mNewBestTimeTextMeshShadow;

	private GameObject mPopupBgObject;

	private Transform mTotalPuffleOBgTransform;

	private Transform mTotalTimeBgTransform;

	private float mWaitTime;

	private bool mCountingRingsCompleted;

	private bool mTimeTrialUnlocked;

	private bool mRotateArrows;

	public enum Button
	{
		eMenu,
		eNextLevel,
		eReplayLevel,
		eLogin,
		eButton_COUNT
	}

	public enum Textures
	{
		eBigPuffleO = 4
	}

	private enum State
	{
		eState_SlideInAnim,
		eState_SlideOutAnim,
		eState_CountingRingsAndCoins,
		eState_TransferCoinsFX,
		eState_HandlingTransferError,
		eState_WaitingForNextState,
		eState_WaitingForSlideInAnimDone,
		eState_WaitingForLogin,
		eState_WaitingForTransfer,
		eState_Idle,
		eState_None
	}

	private enum LabelIndex
	{
		eLevelCoinCount
	}
}

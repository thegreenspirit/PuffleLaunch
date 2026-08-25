using System;

public class Information : BaseMonoScreen
{
	protected override void CreateMainScreenLayouts()
	{
		base.MainScreen.TextureData = new GUIDefines.TextureData[]
		{
			new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 1f,
					heightRatio = 0.120313f
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "Create_Account_NavBar"
				}
			}
		};
		base.MainScreen.ButtonData = new GUIDefines.ButtonData[]
		{
			new GUIDefines.ButtonData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.013542f,
					topRatio = 0.01719f,
					widthRatio = 0.15f,
					heightRatio = 0.082813f
				},
				style = new GUIDefines.StyleInfo
				{
					styleName = "CloseButton"
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_EndGameScreen"
				}
			}
		};
	}

	private void Awake()
	{
		this.Init(base.gameObject);
	}

	private void OnGUI()
	{
		if (!base.MainScreen.CanDraw())
		{
			return;
		}
		base.MainScreen.Draw();
		this.BlockControl(false);
	}

	protected override void OnMainScreenButtonSelect()
	{
		Information.Button selectedButton = (Information.Button)base.MainScreen.SelectedButton;
		if (selectedButton == Information.Button.eBack)
		{
			base.MainScreen.StopGUI();
			GameFlowManager.Instance.LoadScene("!Loader_MainMenu", false);
		}
	}

	protected override void OnBack()
	{
		base.MainScreen.BlockControl(true);
		GameFlowManager.Instance.LoadScene("!Loader_MainMenu", false);
	}

	private enum Button
	{
		eBack,
		eButton_COUNT
	}
}

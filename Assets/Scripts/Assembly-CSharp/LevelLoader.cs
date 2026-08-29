using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
	public void Awake()
	{
		this.loadingProgress = 0f;
		this.isLoadingFinished = false;
		base.StartCoroutine(this.AwakeInternal());
	}

	public IEnumerator AwakeInternal()
	{
		LevelLoader.mSingleton = this;
		TextAsset levelFile = (TextAsset)Resources.Load(string.Format("LevelData/Level{0}", LevelSelect.SelectedLevel), typeof(TextAsset));
		this.mReader = new StringReader(levelFile.text);
		string line = this.mReader.ReadLine();
		string[] header = line.Split(new char[] { ',' });
		float totalElements = float.Parse(header[6]);
		this.progressIncrement = 1f / totalElements;
		this.elementChunkToLoad = (int)(0.2f * totalElements);
		int worldNumber = int.Parse(header[0]) - 1;
		Color[] bgColors = new Color[]
		{
			new Color(0.015686275f, 0.4509804f, 0.7529412f),
			new Color(0.96862745f, 0.5411765f, 0.28627452f),
			new Color(0.43529412f, 0.09019608f, 0.6156863f),
			new Color(0.99607843f, 0.7764706f, 0.23529412f)
		};
		this.gameplayCamera.backgroundColor = bgColors[worldNumber];
		float groundPosition = -float.Parse(header[1]) * ScaleItem.Instance.LevelScale;
		global::UnityEngine.Object.Instantiate(this.fallZones[worldNumber], new Vector3(0f, groundPosition, 0f), default(Quaternion));
		this.loadingProgress += this.progressIncrement;
		yield return null;
		Transform puffleTransform = (Transform)global::UnityEngine.Object.Instantiate(AssetLoader.Instance.PuffleTemplate, new Vector3(float.Parse(header[2]) * ScaleItem.Instance.LevelScale, -float.Parse(header[3]) * ScaleItem.Instance.LevelScale, 0f), default(Quaternion));
		Puffle puffleInstance = puffleTransform.GetComponent<Puffle>();
		puffleInstance.tag = "Player";
		puffleInstance.GetComponent<Renderer>().enabled = true;
		puffleInstance.gameObject.active = true;
		puffleInstance.GetComponent<Rigidbody>().WakeUp();
		this.loadingProgress += this.progressIncrement;
		yield return null;
		Camera.main.GetComponent<CameraFollow>().Target = puffleInstance.transform;
		Vector3 targetPos = puffleInstance.transform.position;
		targetPos.z = -10f;
		Camera.main.transform.position = targetPos;
		Camera.main.GetComponent<CameraFollow>().FixedUpdate();
		Camera.main.GetComponent<CameraFollow>().UpdateTransform(100f);
		puffleInstance.spawnPoint = puffleInstance.transform.position;
		puffleInstance.groundPosition = groundPosition;
		puffleInstance.Splash = (Splash)global::UnityEngine.Object.Instantiate(this.splashVariants[worldNumber], puffleInstance.transform.position, default(Quaternion));
		puffleInstance.Splash.gameObject.active = false;
		puffleInstance.Splash.GetComponent<MeshRenderer>().enabled = false;
		ScaleItem.Instance.PlayerRadius = puffleInstance.GetComponent<SphereCollider>().radius;
		ScaleItem.Instance.ScaleLevelItem(puffleInstance.transform, 1f, 1f, true);
		this.loadingProgress += this.progressIncrement;
		yield return null;
		this.mCeilingHeight = puffleInstance.transform.position.y;
		Transform giantPuffleO = (Transform)global::UnityEngine.Object.Instantiate(AssetLoader.Instance.GiantPuffleOTemplate, new Vector3(float.Parse(header[4]) * ScaleItem.Instance.LevelScale, -float.Parse(header[5]) * ScaleItem.Instance.LevelScale, 0f), this.levelEnd.rotation);
		giantPuffleO.GetComponent<Renderer>().enabled = true;
		giantPuffleO.gameObject.active = true;
		giantPuffleO.tag = "Finish";
		ScaleItem.Instance.ScaleLevelItem(giantPuffleO, 1f, 1f, false);
		for (;;)
		{
			line = this.mReader.ReadLine();
			if (line == null)
			{
				break;
			}
			if (line.Length > 0)
			{
				if (line.Equals("[Background]"))
				{
					this.yieldCount = 0;
					while (this.ParseBackground(ref this.mReader, 0.3f))
					{
						this.loadingProgress += this.progressIncrement;
						this.yieldCount++;
						if (this.yieldCount == this.elementChunkToLoad)
						{
							this.yieldCount = 0;
							yield return null;
						}
					}
					if (this.yieldCount > 0)
					{
						yield return null;
					}
				}
				else if (line.Equals("[Cannons]"))
				{
					this.yieldCount = 0;
					while (this.ParseCannons(ref this.mReader))
					{
						this.loadingProgress += this.progressIncrement;
						this.yieldCount++;
						if (this.yieldCount == this.elementChunkToLoad)
						{
							this.yieldCount = 0;
							yield return null;
						}
					}
					if (this.yieldCount > 0)
					{
						yield return null;
					}
				}
				else if (line.Equals("[Obstacles]"))
				{
					this.yieldCount = 0;
					while (this.ParseObstacles(ref this.mReader))
					{
						this.loadingProgress += this.progressIncrement;
						this.yieldCount++;
						if (this.yieldCount == this.elementChunkToLoad)
						{
							this.yieldCount = 0;
							yield return null;
						}
					}
					if (this.yieldCount > 0)
					{
						yield return null;
					}
				}
				else if (line.Equals("[PuffleOs]"))
				{
					this.yieldCount = 0;
					while (this.ParsePuffleOs(ref this.mReader))
					{
						this.loadingProgress += this.progressIncrement;
						this.yieldCount++;
						if (this.yieldCount == this.elementChunkToLoad)
						{
							this.yieldCount = 0;
							yield return null;
						}
					}
					if (this.yieldCount > 0)
					{
						yield return null;
					}
					GameObject.Find("Main Camera").transform.Find("ProgressBar").GetComponent<ProgressBar>().TotalPuffleOs = this.mNumPuffleOs;
				}
			}
		}
		this.mReader.Close();
		if (worldNumber == 2)
		{
			this.mCeilingHeight += 500f * ScaleItem.Instance.LevelScale;
			puffleInstance.ceilingPosition = this.mCeilingHeight;
			FallZone ceiling = (FallZone)global::UnityEngine.Object.Instantiate(this.fallZones[worldNumber], new Vector3(0f, this.mCeilingHeight, 0f), default(Quaternion));
			Vector3 ceilingScale = ceiling.transform.localScale;
			ceilingScale.y *= -1f;
			ceiling.transform.localScale = ceilingScale;
		}
		else
		{
			puffleInstance.ceilingPosition = float.PositiveInfinity;
		}
		this.loadingProgress = 1f;
		GameFlowManager.Instance.GUIManager.LoadingScreen.StopLoadingBar();
		this.isLoadingFinished = true;
		yield return null;
		this.loadingProgress = 0f;
		yield break;
	}

	private IEnumerator LoadAsset(string assetURL)
	{
		yield return null;
		yield break;
	}

	public static LevelLoader Instance
	{
		get { return LevelLoader.mSingleton; }
	}

	private bool ParseBackground(ref StringReader aReader, float aZOffset)
	{
		string text = aReader.ReadLine();
		if (text == null || text.Length == 0)
		{
			return false;
		}
		string[] array = text.Split(new char[] { ',' });
		int num = int.Parse(array[0]) - 1;
		if (num < this.backgroundElements.Length)
		{
			Transform transform = this.backgroundElements[num];
			if (transform)
			{
				Vector3 vector = new Vector3(float.Parse(array[1]) * ScaleItem.Instance.LevelScale, -float.Parse(array[2]) * ScaleItem.Instance.LevelScale, aZOffset);
				Transform transform2 = (Transform)global::UnityEngine.Object.Instantiate(transform, vector, transform.transform.rotation);
				ScaleItem.Instance.ScaleLevelItem(transform2, float.Parse(array[3]), float.Parse(array[4]), false);
				aZOffset += 0.1f;
			}
			else
			{
				Debug.LogWarning(string.Format("Background element not set: {0}", num));
			}
		}
		else
		{
			Debug.LogWarning(string.Format("Background index out of range: {0}", num));
		}
		return true;
	}

	private bool ParseCannons(ref StringReader aReader)
	{
		string text = this.mReader.ReadLine();
		if (text == null || text.Length == 0)
		{
			return false;
		}
		string[] array = text.Split(new char[] { ',' });
		int num = int.Parse(array[0]) - 1;
		if (num < this.cannonVariants.Length)
		{
			Cannon CannonVariant = this.cannonVariants[num];
			if (CannonVariant)
			{
				Cannon CannonGameObject = (Cannon)global::UnityEngine.Object.Instantiate(CannonVariant);
				CannonGameObject.gameObject.SetActiveRecursively(true); // Green Spirit: This was not here for some reason
				CannonGameObject.transform.position = new Vector3(float.Parse(array[1]) * ScaleItem.Instance.LevelScale, -float.Parse(array[2]) * ScaleItem.Instance.LevelScale, 0f);
				CannonGameObject.transform.eulerAngles = new Vector3(0f, 0f, -float.Parse(array[3]));
				ScaleItem.Instance.ScaleLevelItem(CannonGameObject.transform, float.Parse(array[4]), float.Parse(array[5]), false);

				if (CannonGameObject.transform.position.y > this.mCeilingHeight)
				{
					this.mCeilingHeight = CannonGameObject.transform.position.y;
				}
			}
			else Debug.LogWarning(string.Format("Cannon variant not set: {0}", num));
		}
		else Debug.LogWarning(string.Format("Cannon index out of range: {0}", num));

		return true;
	}

	private bool ParseObstacles(ref StringReader aReader)
	{
		string text = this.mReader.ReadLine();
		if (text == null || text.Length == 0)
		{
			return false;
		}
		string[] array = text.Split(new char[] { ',' });
		int num = int.Parse(array[0]) - 1;
		if (num < this.obstacleVariants.Length)
		{
			Transform transform = this.obstacleVariants[num];
			if (transform)
			{
				Transform transform2 = (Transform)global::UnityEngine.Object.Instantiate(transform);
				transform2.gameObject.SetActiveRecursively(true); // Green Spirit: This was not here for some reason
				transform2.transform.position = new Vector3(float.Parse(array[1]) * ScaleItem.Instance.LevelScale, -float.Parse(array[2]) * ScaleItem.Instance.LevelScale, 0f);
				transform2.transform.eulerAngles = new Vector3(0f, 0f, -float.Parse(array[3]));
				ScaleItem.Instance.ScaleLevelItem(transform2, float.Parse(array[4]), float.Parse(array[5]), false);
				if (transform2.position.y > this.mCeilingHeight)
				{
					this.mCeilingHeight = transform2.position.y;
				}
			}
			else Debug.LogWarning(string.Format("Obstacle variant not set: {0}", num));
		}
		else Debug.LogWarning(string.Format("Obstacle index out of range: {0}", num));

		return true;
	}

	private bool ParsePuffleOs(ref StringReader aReader)
	{
		string text = this.mReader.ReadLine();
		if (text == null || text.Length == 0)
		{
			return false;
		}
		string[] array = text.Split(new char[] { ',' });
		PuffleO puffleO = (PuffleO)global::UnityEngine.Object.Instantiate(this.puffleO);
		puffleO.transform.position = new Vector3(float.Parse(array[1]) * ScaleItem.Instance.LevelScale, -float.Parse(array[2]) * ScaleItem.Instance.LevelScale, 0f);
		ScaleItem.Instance.ScaleLevelItem(puffleO.transform, float.Parse(array[3]), float.Parse(array[4]), false);
		this.mNumPuffleOs++;
		if (puffleO.transform.position.y > this.mCeilingHeight)
		{
			this.mCeilingHeight = puffleO.transform.position.y;
		}
		return true;
	}

	public int NumPuffleOs
	{
		get { return this.mNumPuffleOs; }
	}

	private const float chunkPercentage = 0.2f;

	public string levelName;

	public Puffle puffle;

	public Transform levelEnd;

	public Transform[] backgroundElements;

	public Transform[] obstacleVariants;

	public Cannon[] cannonVariants;

	public PuffleO puffleO;

	public Camera gameplayCamera;

	public FallZone[] fallZones;

	public Splash[] splashVariants;

	public float loadingProgress;

	public bool isLoadingFinished;

	private static LevelLoader mSingleton;

	private int mNumPuffleOs;

	private float mCeilingHeight;

	private StringReader mReader;

	private float progressIncrement;

	private int yieldCount;

	private int elementChunkToLoad = 10;
}

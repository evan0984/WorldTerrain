using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ThoughtWorld.Terrain.Model;
using System.IO;
using System.Runtime.InteropServices;

namespace ThoughtWorld.Terrain
{
	public class TerrainGenerator : MonoBehaviour
	{
		[DllImport("__Internal")]
		private static extern void BroadcastThoughtID(string str);

		const string terrainJSONFileName = "terrain_island_data.json";
		const float viewerMoveThresholdForChunkUpdate = 25f;
		const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

		public int colliderLODIndex;
		public LODInfo[] detailLevels;

		public MeshSettings meshSettings;
		public HeightMapSettings heightMapSettings;
		public TextureData textureSettings;

		public Transform viewer;
		public Material mapMaterial;

		Vector2 viewerPosition;
		Vector2 viewerPositionOld;

		float meshWorldSize;
		int chunksVisibleInViewDst;

		float[,] falloffMap;

		Dictionary<Vector2, ThoughtPoint> thoughtPointDictionary = new Dictionary<Vector2, ThoughtPoint>();
		Dictionary<Vector2, bool> emptyTerrainDictionary = new Dictionary<Vector2, bool>();
		Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
		List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

		private ThoughtWorldTerrain thoughtWorldTerrain;
		private Dictionary<Vector2, ThoughtPointAdjacency> thoughtPointAdjacencies = new Dictionary<Vector2, ThoughtPointAdjacency>();
		private string terrainDataFileName = string.Empty;
		private string terrainDataFilePath = string.Empty;
		private string terrainDataFileText = string.Empty;
		private List<GameObject> topicButtons = new List<GameObject>();

		public GameObject topicButtonPrefab;
		public GameObject thoughtPointCanvasPrefab;

		List<GameObject> debugCubes = new List<GameObject>();
		public Material thoughtBarMaterial;
		public Text textDebug;

		void Start()
		{
			textureSettings.ApplyToMaterial(mapMaterial);
			textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

			float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
			meshWorldSize = meshSettings.meshWorldSize;
			chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

			#if !UNITY_EDITOR && UNITY_WEBGL
			// disable WebGLInput.captureAllKeyboardInput so elements on web page can handle keybord inputs
			WebGLInput.captureAllKeyboardInput = false;
			#endif

			GenerateTerrain();
			//GenerateTerrainFromFile("http://127.0.0.1:8887/terrain_island_data.json");
		}

		private void DisplayDebugCube(Vector3 point)
        {
			// Display cube for debug purposes
            //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.localPosition = point;
            //cube.transform.localScale = new Vector3(20f, 20f, 20f);
            //cube.GetComponent<Renderer>().material = thoughtBarMaterial;
            //debugCubes.Add(cube);
        }

        private void Awake()
        {
			//falloffMap = FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine);
		}

		private void GenerateTerrain()
		{
			if (string.IsNullOrEmpty(terrainDataFileText))
			{
				terrainDataFileName = terrainJSONFileName;

				// Use link below to point to dan's folder
				//terrainDataFilePath = Path.Combine("http://danpahomi.com/thoughtworld/unity12/", terrainDataFileName);

				// Use link below to point to azure folder
				//terrainDataFilePath = Path.Combine("https://thoughtworld-fe.azurewebsites.net/assets/terrain/", terrainDataFileName);

				// Use link below to point to local file in unity project folder
				terrainDataFilePath = Path.Combine(terrainDataFileName);

				// Use link below to point to local server folder
				//terrainDataFilePath = Path.Combine("http://127.0.0.1:8887/",terrainDataFileName);

				StartCoroutine(LoadTerrainDataFromFilePath());
			}
		}

		public void GenerateTerrainFromFile(string filePath)
		{
			terrainDataFilePath = filePath;
			thoughtPointDictionary.Clear();
			emptyTerrainDictionary.Clear();
			terrainChunkDictionary.Clear();
			visibleTerrainChunks.Clear();
			thoughtPointAdjacencies.Clear();
			topicButtons.Clear();
			thoughtWorldTerrain = null;
			
			Debug.Log(string .Format("Called Unity ThoughtWorld GenerateTerrainFromFile method! File Path: {0}", filePath));

			StartCoroutine(LoadTerrainDataFromFilePath());
		}

		IEnumerator LoadTerrainDataFromFilePath()
		{
			string requestResult = string.Empty;

			if (terrainDataFilePath.Contains("://"))
			{
				UnityWebRequest www = UnityWebRequest.Get(terrainDataFilePath);
				yield return www.SendWebRequest();
				if (www.result == UnityWebRequest.Result.ConnectionError ||
					www.result == UnityWebRequest.Result.DataProcessingError ||
					www.result == UnityWebRequest.Result.ProtocolError)
				{
					Debug.Log(www.error);
					requestResult = www.error;
				}
				else
				{
					terrainDataFileText = www.downloadHandler.text;
					requestResult = terrainDataFilePath;

					terrainDataFileText = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data, 3, www.downloadHandler.data.Length - 3);
				}
			}
			else
			{
				yield return terrainDataFileText = File.ReadAllText(terrainDataFileName);
				requestResult = terrainDataFilePath;
			}

			//textDebug.text = string.Format("Load Terrain Data File: {0}", requestResult);

			thoughtWorldTerrain = JsonUtility.FromJson<ThoughtWorldTerrain>(terrainDataFileText);
			if (thoughtWorldTerrain != null)
			{
				CacheThoughtPoints(thoughtWorldTerrain.thoughtPoints);
				BuildThoughtPointAdjacencies(thoughtWorldTerrain.thoughtPoints);
			}

			//textDebug.text = string.Format("Terrain JSON Data: {0}", terrainDataFileText);

			UpdateVisibleChunks();
		}

		void BuildThoughtPointAdjacencies(List<ThoughtPoint> thoughtPoints)
        {
			if (thoughtPoints != null)
            {
				thoughtPointAdjacencies = new Dictionary<Vector2, ThoughtPointAdjacency>();

				// Add initial keys
				foreach (var thoughtPoint in thoughtPoints)
                {
					var pt = new Vector2(thoughtPoint.x, thoughtPoint.y);
					ThoughtPointAdjacency adjacencyInfo = new ThoughtPointAdjacency(thoughtPoint);
					thoughtPointAdjacencies.Add(pt, adjacencyInfo);
                }

				// Determine adjacency info based on existing key points
				foreach (var thoughtPoint in thoughtPoints)
                {
					var pt = new Vector2(thoughtPoint.x, thoughtPoint.y);

					if (thoughtPointAdjacencies.TryGetValue(pt, out ThoughtPointAdjacency adjacencyInfo))
                    {
						bool emptyFound = false;

						adjacencyInfo.adjacentTop    = (thoughtPointAdjacencies.TryGetValue(new Vector2(pt.x, pt.y + 1), out ThoughtPointAdjacency adjacencyTop) && ThoughtWeightThresholdIsNear(adjacencyTop.thoughtPoint.weight))
														|| (emptyTerrainDictionary.TryGetValue(new Vector2(pt.x, pt.y + 1), out emptyFound));

						adjacencyInfo.adjacentBottom = (thoughtPointAdjacencies.TryGetValue(new Vector2(pt.x, pt.y - 1), out ThoughtPointAdjacency adjacencyBottom) && ThoughtWeightThresholdIsNear(adjacencyBottom.thoughtPoint.weight))
													    || (emptyTerrainDictionary.TryGetValue(new Vector2(pt.x, pt.y - 1), out emptyFound));

						adjacencyInfo.adjacentLeft   = (thoughtPointAdjacencies.TryGetValue(new Vector2(pt.x-1, pt.y), out ThoughtPointAdjacency adjacencyLeft) && ThoughtWeightThresholdIsNear(adjacencyLeft.thoughtPoint.weight))
														|| (emptyTerrainDictionary.TryGetValue(new Vector2(pt.x - 1, pt.y), out emptyFound));

						adjacencyInfo.adjacentRight  = (thoughtPointAdjacencies.TryGetValue(new Vector2(pt.x+1, pt.y), out ThoughtPointAdjacency adjacencyRight) && ThoughtWeightThresholdIsNear(adjacencyRight.thoughtPoint.weight))
														|| (emptyTerrainDictionary.TryGetValue(new Vector2(pt.x + 1, pt.y), out emptyFound));
					}
				}
			}
        }

		bool ThoughtWeightThresholdIsNear(float weight)
        {
			if (weight > 0.85f)
			{
				// If thought weight > 0.85, connect the terrain in adjacent thought grid(s) if they exist
				return true;
			}

			return false;
        }

		void Update()
		{
			if (thoughtWorldTerrain != null)
			{
				viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

				if (viewerPosition != viewerPositionOld)
				{
					foreach (TerrainChunk chunk in visibleTerrainChunks)
					{
						chunk.UpdateCollisionMesh();
					}
				}

				if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
				{
					viewerPositionOld = viewerPosition;
					UpdateVisibleChunks();
				}
			}
		}

		void UpdateVisibleChunks()
		{
			HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
			for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
			{
				alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
				visibleTerrainChunks[i].UpdateTerrainChunk();
			}

			int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
			int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);
			
			if (topicButtons == null)
			{
				topicButtons = new List<GameObject>();
			}

			for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
			{
				for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
				{
					Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
					if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
					{
						if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
						{
							terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
						}
						else
						{
							TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
							terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
							newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;

							ThoughtPoint thoughtPoint = TryGetThoughtPoint(viewedChunkCoord.x, viewedChunkCoord.y);
							if ((thoughtPoint != null) && (thoughtPointAdjacencies.TryGetValue(new Vector2(thoughtPoint.x, thoughtPoint.y),out ThoughtPointAdjacency thoughtPointAdjacency)))
							{
								DisplayDebugCube(new Vector3(viewedChunkCoord.x * meshSettings.meshWorldSize, 70, viewedChunkCoord.y * meshSettings.meshWorldSize));

								// Generate falloff height map based on thought weight
								falloffMap = FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, thoughtPoint.weight, thoughtPointAdjacency);

								// Add the terrain chunk
								newChunk.LoadWithTerrainFalloffMap(falloffMap, thoughtPoint.height, thoughtPoint.weight);

								float thoughtPointY = thoughtPoint.height * heightMapSettings.heightMultiplier * 2f;
								if (thoughtPoint.height <= 0.4f)
                                {
									thoughtPointY = thoughtPoint.height * heightMapSettings.heightMultiplier * 3.5f;
								}

								var topicButton = Instantiate(thoughtPointCanvasPrefab, new Vector3(0, 0, 0), Quaternion.identity);
								topicButton.transform.localScale = topicButton.transform.localScale / 2;
								topicButton.transform.localPosition = new Vector3(newChunk.terrainMesh.transform.localPosition.x-(viewedChunkCoord.x * meshSettings.meshWorldSize), thoughtPointY, newChunk.terrainMesh.transform.localPosition.y);
								topicButton.transform.SetParent(newChunk.terrainMesh.transform, false);

								Button[] buttons = topicButton.GetComponentsInChildren<Button>(true);
								if (buttons != null && buttons.Length > 0)
								{
									//buttons[0].onClick.AddListener(delegate { TopicButtonClicked(thoughtPoint.thoughtUrl); });
									buttons[0].onClick.AddListener(delegate { TopicButtonClicked(thoughtPoint.thoughtID); });

									Text[] textList = buttons[0].GetComponentsInChildren<Text>(true);
									if (textList != null)
									{
										// Temporarily displaying height and weight for test purposes
										textList[0].text = string.Format("{0}\n\r\n\rHeight = {1}\n\rWeight = {2}", thoughtPoint.thoughtLabel, thoughtPoint.height, thoughtPoint.weight);
										textList[1].text = thoughtPoint.heightDisplayText;
										textList[2].text = thoughtPoint.weightDisplayText;
									}
								}

								topicButton.SetActive(true);
								topicButtons.Add(topicButton);
							}
							else
							{
								// Check if this terrain chunk is an empty terrain with no thought point
								if (IsEmptyTerrain(viewedChunkCoord.x, viewedChunkCoord.y))
								{
									// Generate falloff height map based on thought weight
									falloffMap = FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, 0.89f, null);

									// Use green flat empty terrain chunk
									newChunk.LoadWithEmptyTerrain(falloffMap, 0.093f);
								}
								else
								{
									// Use water terrain chunk
									newChunk.LoadWithNoTerrain();
								}
							}

							newChunk.SetVisible(true);
						}
					}
				}
			}
        }

		//private void TopicButtonClicked(string url)
		//{
			//if (url.StartsWith("http://") || url.StartsWith("https://"))
			//	Application.OpenURL(url);
			//else
			//	Application.OpenURL(string.Format("http://{0}", url));
		//}

		private void TopicButtonClicked(string thoughtID)
		{
			ThoughtMessage message = new ThoughtMessage
			{
				channel = "unity",
				thoughtID = thoughtID
			};
			
			BroadcastThoughtID(JsonUtility.ToJson(message));
		}

		void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
        {
			if (isVisible)
            {
				visibleTerrainChunks.Add(chunk);
            }
			else
            {
				visibleTerrainChunks.Remove(chunk);
			}
        }

		private void CacheThoughtPoints(List<ThoughtPoint> thoughtPoints)
        {
			thoughtPointDictionary.Clear();
			emptyTerrainDictionary.Clear();

			if ((thoughtPoints != null) && (thoughtPoints.Count > 0))
			{
				foreach (var thoughtPoint in thoughtWorldTerrain.thoughtPoints)
				{
					thoughtPointDictionary.Add(new Vector2(thoughtPoint.x, thoughtPoint.y), thoughtPoint);
				}

				int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
				int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

				for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
				{
					for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
					{
						Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
						IsEmptyTerrain(viewedChunkCoord.x, viewedChunkCoord.y);
					}
				}
			}
		}

		private ThoughtPoint TryGetThoughtPoint(float x, float y)
        {
			if ((thoughtPointDictionary != null) && (thoughtPointDictionary.Values.Count > 0))
			{
				Vector2 pointKey = new Vector2(x, y);

				if (thoughtPointDictionary.TryGetValue(pointKey, out ThoughtPoint thoughtPoint))
                {
					return thoughtPoint;
                }
            }

			return null;
        }

		private bool IsEmptyTerrain(float x, float y)
		{
			if ((thoughtPointDictionary != null) && (thoughtPointDictionary.Values.Count > 0))
			{
				Vector2 pointKey = new Vector2(x, y);
				bool emptyFound = false;

				if (emptyTerrainDictionary.TryGetValue(pointKey, out emptyFound))
                {
					return true;
                }
				else
                {
					int counter = 1;
					bool leftFound = false;
					bool topFound = false;

					while (counter < 10)
                    {
						Vector2 prevChunkPoint = new Vector2(x - counter, y);

						if (thoughtPointDictionary.TryGetValue(prevChunkPoint, out ThoughtPoint thoughtPoint))
						{
							leftFound = true;
							break;
						}

						counter++;
					}

					if (leftFound)
                    {
						counter = 1;

						// Determine if terrain chunk is in between 2 thought points
						while (counter < 10)
						{
							Vector2 nextChunkPoint = new Vector2(x + counter, y);

							if (thoughtPointDictionary.TryGetValue(nextChunkPoint, out ThoughtPoint thoughtPoint))
							{
								emptyTerrainDictionary.Add(pointKey, true);
								return true;
							}

							counter++;
						}
					}

					counter = 1;
					while (counter < 10)
					{
						Vector2 prevChunkPoint = new Vector2(x, y - counter);

						if (thoughtPointDictionary.TryGetValue(prevChunkPoint, out ThoughtPoint thoughtPoint))
						{
							topFound = true;
							break;
						}

						counter++;
					}

					if (topFound)
					{
						counter = 1;

						// Determine if terrain chunk is in between 2 thought points
						while (counter < 10)
						{
							Vector2 nextChunkPoint = new Vector2(x, y + counter);

							if (thoughtPointDictionary.TryGetValue(nextChunkPoint, out ThoughtPoint thoughtPoint))
							{
								emptyTerrainDictionary.Add(pointKey, true);
								return true;
							}

							counter++;
						}
					}
				}
			}

			return false;
		}
	}

	[System.Serializable]
	public struct LODInfo
	{
		[Range(0, MeshSettings.numSupportedLODs - 1)]
		public int lod;
		public float visibleDstThreshold;

		public float sqrVisibleDstThreshold
		{
			get
			{
				return visibleDstThreshold * visibleDstThreshold;
			}
		}
	}
}

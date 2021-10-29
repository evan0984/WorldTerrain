using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TriangleNet;
using UnityEngine;
using TriangleNet.Topology;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Networking;
using ThoughtWorld.Terrain.Model;
using ThoughtWorld.Terrain.Enum;

namespace ThoughtWorld.Terrain
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
    public class TerrainGeneratorOld : MonoBehaviour
    {
        [Header("Terrain Adjustment Controls")]

        public Slider heightSlider;
        public Button newSeedButton;
        public Button shiftUpButton;
        public Button shiftDownButton;
        public Button shiftLeftButton;
        public Button shiftRightButton;
        public Button morePointsButton;
        public Button lessPointsButton;
        public Button topicButton1;
        public Button topicButton2;
        public Button topicButton3;
        public Button topicButton4;
        public Text textPolygonJsonFile;
        public Text textNoiseJsonFile;

        [Header("Load & Save Data Controls")]

        public Button loadData1Button;
        public Button saveData1Button;
        public Button loadData2Button;
        public Button saveData2Button;
        public Button loadData3Button;
        public Button saveData3Button;
        public Button loadData4Button;
        public Button saveData4Button;
        public Button loadData5Button;
        public Button loadData6Button;
        public Toggle toggleDisplayGrid;

        public Material gridDotMaterial;
        public Material thoughtBarMaterial;
        public GameObject topicButtonPrefab;
        public GameObject thoughtPointCanvasPrefab;

        private string polygonFileName = string.Empty;
        private string noiseDataFileName = string.Empty;
        private string polygonFilePath = string.Empty;
        private string noiseDataFilePath = string.Empty;
        private string polygonFileText = string.Empty;
        private string noiseDataFileText = string.Empty;

        private string terrainDataFileName = string.Empty;
        private string terrainDataFilePath = string.Empty;
        private string terrainDataFileText = string.Empty;

        private ThoughtWorldTerrain thoughtWorldTerrain;
        private TerrainNoise terrainNoise = null;

        [Header("General Settings")] public Material material;

        [Range(1, 2000)] public int sizeX;
        [Range(1, 1000)] public int sizeY;

        [Header("Point Distribution")]

        [Range(4, 6000)]
        public int pointDensity;
        public bool randomPoints;
        [Range(10, 150)] public float minDistancePerPoint = 10;
        [Range(5, 50)] public int rejectionSamples = 30;

        [Header("Colors")]

        public ColorSetting colorSetting;
        public Gradient heightGradient;

        [Header("Simple Perlin Noise")]

        [Range(1f, 3000f)] public float heightScale = 50f;
        [Range(5f, 300f)] public float scale = 34;
        [Range(0.001f, 1.00f)] public float dampening = 0.21f;

        [Header("Layered Noise")]
        [Range(1, 15)]
        public int octaves = 1;

        [Range(0f, 1f)] public float persistence = 0.1f;
        [Range(1f, 10f)] public float lacunarity = 1.5f;
        public Vector2 offset;

        [HideInInspector] public int seed;

        private List<Vector2> poissonPoints = new List<Vector2>();
        private Polygon polygon;
        private TriangleNet.Mesh mesh;
        private UnityEngine.Mesh terrainMesh;
        private List<float> heights = new List<float>();

        private float minNoiseHeight;
        private float maxNoiseHeight;

        private List<Vector3> gridSectionVertices;
        private List<Vector3> gridSubSectionVertices;
        private List<GameObject> gridSectionDots;
        private List<GameObject> gridSubSectionDots;
        private Dictionary<string, GameObject> gridSectionCenterCubes;

        private List<GameObject> topicButtons;

        private const int sectionGridSize = 99;
        private const int subSectionGridSize = 9;
        private const int sectionGridDotSize = 10;
        private const int subSectionGridDotSize = 2;

        public void Start()
        {
            //gridDotMaterial = Resources.Load<Material>("GridDot");

            if (heightSlider != null)
            {
                heightSlider.value = heightScale;
                heightSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
                //Initiate();
                GenerateTerrain("terrain_data1.json");
            }

            seed = 20;

            if (shiftUpButton != null)
            {
                shiftUpButton.onClick.AddListener(delegate { ShiftUpClicked(); });
            }

            if (shiftDownButton != null)
            {
                shiftDownButton.onClick.AddListener(delegate { ShiftDownClicked(); });
            }

            if (shiftLeftButton != null)
            {
                shiftLeftButton.onClick.AddListener(delegate { ShiftLeftClicked(); });
            }

            if (shiftRightButton != null)
            {
                shiftRightButton.onClick.AddListener(delegate { ShiftRightClicked(); });
            }

            if (morePointsButton != null)
            {
                morePointsButton.onClick.AddListener(delegate { MorePointsClicked(); });
            }

            if (lessPointsButton != null)
            {
                lessPointsButton.onClick.AddListener(delegate { LessPointsClicked(); });
            }

            if (newSeedButton != null)
            {
                newSeedButton.onClick.AddListener(delegate { NewSeedClicked(); });
            }

            if (topicButton1 != null)
            {
                topicButton1.onClick.AddListener(delegate { Topic1ButtonClicked(); });
            }

            if (topicButton2 != null)
            {
                topicButton2.onClick.AddListener(delegate { Topic2ButtonClicked(); });
            }

            if (topicButton3 != null)
            {
                topicButton3.onClick.AddListener(delegate { Topic3ButtonClicked(); });
            }

            if (topicButton4 != null)
            {
                topicButton4.onClick.AddListener(delegate { Topic4ButtonClicked(); });
            }

            if (loadData1Button != null)
            {
                loadData1Button.onClick.AddListener(delegate { LoadData1ButtonClicked(); });
            }

            if (saveData1Button != null)
            {
                saveData1Button.onClick.AddListener(delegate { SaveData1ButtonClicked(); });
            }

            if (loadData2Button != null)
            {
                loadData2Button.onClick.AddListener(delegate { LoadData2ButtonClicked(); });
            }

            if (saveData2Button != null)
            {
                saveData2Button.onClick.AddListener(delegate { SaveData2ButtonClicked(); });
            }

            if (loadData3Button != null)
            {
                loadData3Button.onClick.AddListener(delegate { LoadData3ButtonClicked(); });
            }

            if (saveData3Button != null)
            {
                saveData3Button.onClick.AddListener(delegate { SaveData3ButtonClicked(); });
            }

            if (loadData4Button != null)
            {
                loadData4Button.onClick.AddListener(delegate { LoadData4ButtonClicked(); });
            }

            if (saveData4Button != null)
            {
                saveData4Button.onClick.AddListener(delegate { SaveData4ButtonClicked(); });
            }

            if (loadData5Button != null)
            {
                loadData5Button.onClick.AddListener(delegate { LoadData5ButtonClicked(); });
            }

            if (loadData6Button != null)
            {
                loadData6Button.onClick.AddListener(delegate { LoadData6ButtonClicked(); });
            }

            if (toggleDisplayGrid != null)
            {
                toggleDisplayGrid.onValueChanged.AddListener(delegate { ToggleDisplayGridChanged(toggleDisplayGrid); });
            }

            //DisplayDotGrid(true);
        }

        private void DisplayDotGrid(bool display)
        {
            DisplaySectionDotGrid(ref gridSectionVertices, ref gridSectionDots, sectionGridSize, sectionGridDotSize, display, true);
            DisplaySectionDotGrid(ref gridSubSectionVertices, ref gridSubSectionDots, subSectionGridSize, subSectionGridDotSize, display);
        }

        private void ClearDotGrid()
        {
            if (gridSectionVertices != null)
            {
                gridSectionVertices.Clear();
                gridSectionVertices = null;
            }

            if (gridSectionDots != null)
            {
                foreach (var d in gridSectionDots)
                {
                    Object.Destroy(d);
                }
                gridSectionDots.Clear();
                gridSectionDots = null;
            }

            if (gridSubSectionVertices != null)
            {
                gridSubSectionVertices.Clear();
                gridSubSectionVertices = null;
            }

            if (gridSubSectionDots != null)
            {
                foreach (var d in gridSubSectionDots)
                {
                    Object.Destroy(d);
                }
                gridSubSectionDots.Clear();
                gridSubSectionDots = null;
            }

            if (gridSectionCenterCubes != null)
            {
                foreach (var c in gridSectionCenterCubes)
                {
                    Object.Destroy(c.Value);
                }
                gridSectionCenterCubes.Clear();
                gridSectionCenterCubes = null;
            }
        }

        private void DisplaySectionDotGrid(ref List<Vector3> gridVertices, ref List<GameObject> gridDots, int sectionSize, float dotRadius, bool display, bool trackCooridinates=false)
        {
            int cellXCount = 0;
            int cellZCount = 0;

            if (gridDots == null && display)
            {
                gridVertices = new List<Vector3>();
                gridDots = new List<GameObject>();
                
                int yCoordinate = 0;
                for (int z = 0; z <= sizeY; z++)
                {
                    int xCoordinate = 0;
                    for (int x = 0; x <= sizeX; x++)
                    {
                        if (cellXCount == 0 && cellZCount == 0)
                        {
                            gridVertices.Add(new Vector3(x, 0, z));
                            var gridDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            gridDot.transform.localPosition = new Vector3(x, 0, z);
                            gridDot.transform.localScale = new Vector3(dotRadius, dotRadius, dotRadius);
                            gridDot.GetComponent<Renderer>().material = gridDotMaterial;
                            gridDots.Add(gridDot);

                            if (trackCooridinates && (x != 0 && z != 0))
                            {
                                if (gridSectionCenterCubes == null)
                                {
                                    gridSectionCenterCubes = new Dictionary<string, GameObject>();
                                }

                                var gridCenterCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                gridCenterCube.transform.localPosition = new Vector3(x - (sectionSize / 2), 0, z - (sectionSize / 2));
                                gridCenterCube.transform.localScale = new Vector3(dotRadius*2f, dotRadius*2f, dotRadius*2f);
                                gridCenterCube.name = string.Format("cube_{0}_{1}", xCoordinate, yCoordinate);
                                gridCenterCube.GetComponent<Renderer>().material = thoughtBarMaterial;

                                //var topicButton = Instantiate(thoughtPointCanvasPrefab, new Vector3(0,0,0), Quaternion.identity);
                                //topicButton.transform.localPosition = new Vector3(x - (sectionSize / 2), 0, z - (sectionSize / 2));
                                //topicButton.transform.localScale = new Vector3(20f, 20f, 20f);
                                //topicButton.transform.SetParent(this.transform, false);

                                var coordinateKey = string.Format("{0},{1}", xCoordinate-1, yCoordinate-1);
                                if (gridSectionCenterCubes.ContainsKey(coordinateKey))
                                {
                                    gridSectionCenterCubes.Remove(coordinateKey);
                                }

                                gridSectionCenterCubes.Add(coordinateKey, gridCenterCube);
                            }
                        }

                        if (cellXCount == sectionSize)
                        {
                            xCoordinate++;
                            cellXCount = 0;
                        }
                        else
                        {
                            cellXCount++;
                        }
                    }

                    if (cellZCount == sectionSize)
                    {
                        yCoordinate++;
                        cellZCount = 0;
                    }
                    else
                    {
                        cellZCount++;
                    }
                }
            }
            else
            {
                if (gridDots != null)
                {
                    foreach (var gridDot in gridDots)
                    {
                        gridDot.SetActive(display);
                    }
                }

                if ((gridSectionCenterCubes != null) && (thoughtWorldTerrain != null) && (thoughtWorldTerrain.thoughtPoints != null))
                {
                    foreach (var thoughtPoint in thoughtWorldTerrain.thoughtPoints)
                    {
                        var coordinateKey = string.Format("{0},{1}", (int)thoughtPoint.x, (int)thoughtPoint.y);
                        if (gridSectionCenterCubes.ContainsKey(coordinateKey))
                        {
                            gridSectionCenterCubes[coordinateKey].SetActive(true);
                        }
                        else
                        {
                            gridSectionCenterCubes[coordinateKey].SetActive(false);
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            //if (mesh == null || mesh.vertices == null)
            //    return;

            //for (int i = 0; i < mesh.vertices.Count; i++)
            //{
            //    Gizmos.color = new Color(255, 0, 0);
            //    Gizmos.DrawSphere(new Vector3((float)mesh.vertices[i].X, 0, (float)mesh.vertices[i].Y), 2f);
            //}
        }

        private void ValueChangeCheck()
        {
            //Debug.Log(heightSlider.value);
            UpdateShape(heightSlider.value);
        }

        private void ShiftUpClicked()
        {
            offset.y -= 0.1f;
            Initiate();
        }

        private void ShiftDownClicked()
        {
            offset.y += 0.1f;
            Initiate();
        }

        private void ShiftLeftClicked()
        {
            offset.x += 0.1f;
            Initiate();
        }

        private void ShiftRightClicked()
        {
            offset.x -= 0.1f;
            Initiate();
        }

        private void NewSeedClicked()
        {
            polygonFileText = string.Empty;
            noiseDataFileText = string.Empty;
            morePoints = false;
            seed = Random.Range(0, 100);
            Initiate();
        }

        private bool morePoints = false;

        public void MorePointsClicked()
        {
            morePoints = true;
            Initiate();
        }

        public void LessPointsClicked()
        {
            morePoints = false;
            Initiate();
        }

        private void Topic1ButtonClicked()
        {
            Application.OpenURL("http://unity3d.com/");
        }

        private void Topic2ButtonClicked()
        {
            Application.OpenURL("http://apple.com/");
        }

        private void Topic3ButtonClicked()
        {
            Application.OpenURL("http://google.com/");
        }

        private void Topic4ButtonClicked()
        {
            Application.OpenURL("http://amazon.com/");
        }

        private void LoadData1ButtonClicked()
        {
            //loadTerrainData("polygon_Data1.json", "noise_Data1.json");

            GenerateTerrain("terrain_data1.json");
        }

        private void SaveData1ButtonClicked()
        {
            saveTerrainData("polygon_Data1.json", "noise_Data1.json");
        }

        private void LoadData2ButtonClicked()
        {
            //loadTerrainData("polygon_Data2.json", "noise_Data2.json");

            GenerateTerrain("terrain_data2.json");
        }

        private void SaveData2ButtonClicked()
        {
            saveTerrainData("polygon_Data2.json", "noise_Data2.json");
        }

        private void LoadData3ButtonClicked()
        {
            loadTerrainData("polygon_Data3.json", "noise_Data3.json");
        }

        private void SaveData3ButtonClicked()
        {
            saveTerrainData("polygon_Data3.json", "noise_Data3.json");
        }

        private void LoadData4ButtonClicked()
        {
            loadTerrainData("polygon_Data4.json", "noise_Data4.json");
        }

        private void SaveData4ButtonClicked()
        {
            saveTerrainData("polygon_Data4.json", "noise_Data4.json");
        }

        private void LoadData5ButtonClicked()
        {
            loadTerrainData("polygon_Data5.json", "noise_Data5.json");
        }

        private void LoadData6ButtonClicked()
        {
            GenerateTerrain("terrain_data1.json");
        }

        private void ToggleDisplayGridChanged(Toggle change)
        {
            DisplayDotGrid(change.isOn);
        }

        private void GenerateTerrain(string terrainDataFile)
        {
            terrainDataFileName = terrainDataFile;
            terrainDataFilePath = Path.Combine("http://danpahomi.com/thoughtworld/unity6/", terrainDataFileName);
            terrainDataFilePath = Path.Combine(terrainDataFileName);
            //terrainDataFilePath = Path.Combine("http://127.0.0.1:8887/",terrainDataFileName);

            StartCoroutine(LoadTerrainDataFromFilePath());
        }

        private void GenerateTerrain(bool clearGrid=false)
        {
            polygonFileText = string.Empty;
            noiseDataFileText = string.Empty;
            morePoints = false;

            if (topicButtons == null)
            {
                topicButtons = new List<GameObject>();
            }
            else
            {
                foreach(GameObject t in topicButtons)
                {
                    Destroy(t);
                }
            }

            if (clearGrid)
                ClearDotGrid();

            thoughtWorldTerrain = JsonUtility.FromJson<ThoughtWorldTerrain>(terrainDataFileText);
            if (thoughtWorldTerrain != null)
            {
                sizeX = thoughtWorldTerrain.sizeX;
                sizeY = thoughtWorldTerrain.sizeY;

                DisplayDotGrid(toggleDisplayGrid.isOn);

                heights = new List<float>();
                polygon = new Polygon();

                for (int i = 0; i < pointDensity; i++)
                {
                    var x = Random.Range(.0f, sizeX);
                    var y = Random.Range(.0f, sizeY);
                    polygon.Add(new Vertex(x, y));
                }

                ConstraintOptions constraints = new ConstraintOptions();
                constraints.ConformingDelaunay = true;

                mesh = polygon.Triangulate(constraints) as TriangleNet.Mesh;

                if (gridSectionCenterCubes != null)
                {
                    foreach (var gridCenterCube in gridSectionCenterCubes)
                    {
                        gridCenterCube.Value.SetActive(false);
                    }

                    // Mark areas where thought points will be added to terrain
                    foreach (var thoughtPoint in thoughtWorldTerrain.thoughtPoints)
                    {
                        var coordinateKey = string.Format("{0},{1}", (int)thoughtPoint.x, (int)thoughtPoint.y);
                        if (gridSectionCenterCubes.ContainsKey(coordinateKey))
                        {
                            var thoughtBar = gridSectionCenterCubes[coordinateKey];
                            thoughtBar.SetActive(true);
                            thoughtBar.transform.localScale = new Vector3(20f, 300f * thoughtPoint.height, 20f);
                            thoughtBar.transform.localPosition = new Vector3(thoughtBar.transform.localPosition.x, (300f * thoughtPoint.height) / 2f, thoughtBar.transform.localPosition.z);

                            var topicButton = Instantiate(thoughtPointCanvasPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                            topicButton.transform.localPosition = new Vector3(thoughtBar.transform.localPosition.x, (300f * thoughtPoint.height) + 60f, thoughtBar.transform.localPosition.z);
                            topicButton.transform.SetParent(thoughtBar.transform.parent, false);
                            var button = topicButton.GetComponentsInChildren<Button>();
                            button[0].GetComponentInChildren<Text>().text = thoughtPoint.thoughtLabel;
                            button[0].onClick.AddListener(delegate { TopicButtonClicked(thoughtPoint.thoughtUrl); });
                            topicButton.SetActive(true);
                            topicButtons.Add(topicButton);
                        }
                    }
                }
            }

            ShapeTerrainFromData();
            GenerateMesh();
        }

        private void TopicButtonClicked(string url)
        {
            if (url.StartsWith("http://"))
                Application.OpenURL(url);
            else
                Application.OpenURL(string.Format("http://{0}", url));
        }

        private void loadTerrainData(string polygonDataFile, string noiseDataFile)
        {
            polygonFileName = polygonDataFile;
            noiseDataFileName = noiseDataFile;
            //polygonFilePath = Path.Combine("http://danpahomi.com/thoughtworld/unity4/", polygonFileName);//Path.Combine(Application.streamingAssetsPath, polygonDataFile);
            //noiseDataFilePath = Path.Combine("http://danpahomi.com/thoughtworld/unity4/", noiseDataFileName);//Path.Combine(Application.streamingAssetsPath, noiseDataFile);

            polygonFilePath = Path.Combine(polygonFileName);
            noiseDataFilePath = Path.Combine(noiseDataFileName);

            StartCoroutine(LoadPolygonFromFilePath());
        }

        IEnumerator LoadPolygonFromFilePath()
        {
            string requestResult = string.Empty;

            if (polygonFilePath.Contains("://"))
            {
                UnityWebRequest www = UnityWebRequest.Get(polygonFilePath);
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    requestResult = www.error;
                }
                else
                {
                    polygonFileText = www.downloadHandler.text;
                    requestResult = polygonFilePath;
                }
            }
            else
            {
                yield return polygonFileText = File.ReadAllText(polygonFileName);
                requestResult = polygonFilePath;
            }

            textPolygonJsonFile.text = string.Format("Load Polygon Data File: {0}", requestResult);
            StartCoroutine(LoadNoiseDataFromFilePath());
        }

        IEnumerator LoadNoiseDataFromFilePath()
        {
            string requestResult = string.Empty;

            if (noiseDataFilePath.Contains("://"))
            {
                UnityWebRequest www = UnityWebRequest.Get(noiseDataFilePath);
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    requestResult = www.error;
                }
                else
                {
                    noiseDataFileText = www.downloadHandler.text;
                    requestResult = noiseDataFilePath;
                }
            }
            else
            {
                yield return noiseDataFileText = File.ReadAllText(noiseDataFileName);
                requestResult = noiseDataFilePath;
            }

            textNoiseJsonFile.text = string.Format("Load Noise Data File: {0}", requestResult);
            Initiate();
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

            textNoiseJsonFile.text = string.Format("Load Terrain Data File: {0}", requestResult);
            GenerateTerrain(true);
        }

        private void saveTerrainData(string polygonDataFile, string noiseDataFile)
        {
            polygonFileName = polygonDataFile;
            noiseDataFileName = noiseDataFile;

            // Write polygon data to file
            WritePolygonDataToFile(polygon);

            // Write noise data to file
            WriteHeightDataToFile(terrainNoise);
        }

        public void Initiate()
        {
            heights = new List<float>();
            polygon = new Polygon();

            if (randomPoints == true)
            {
                if (!string.IsNullOrEmpty(polygonFileText))
                {
                    polygon = ReadPolygonDataFromJson(polygonFileText);
                }
                else
                {
                    for (int i = 0; i < pointDensity; i++)
                    {
                        var x = Random.Range(.0f, sizeX);
                        var y = Random.Range(.0f, sizeY);
                        polygon.Add(new Vertex(x, y));
                    }
                }
            }
            else
            {
                poissonPoints = PoissonDiscSampling.GeneratePoints(minDistancePerPoint, new Vector2(sizeX, sizeY), rejectionSamples);
                for (int i = 0; i < poissonPoints.Count; i++)
                {
                    polygon.Add(new Vertex(poissonPoints[i].x, poissonPoints[i].y));
                }
            }

            ConstraintOptions constraints = new ConstraintOptions();
            constraints.ConformingDelaunay = true;

            mesh = polygon.Triangulate(constraints) as TriangleNet.Mesh;

            ShapeTerrain();
            GenerateMesh();
        }

        private void ShapeTerrain()
        {
            minNoiseHeight = float.PositiveInfinity;
            maxNoiseHeight = float.NegativeInfinity;

            terrainNoise = new TerrainNoise();
            List<float> noiseHeights = null;

            if (!string.IsNullOrEmpty(noiseDataFileText))
            {
                noiseHeights = ReadNoiseDataFromJson(noiseDataFileText);
            }

            for (int i = 0; i < mesh.vertices.Count; i++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                if (noiseHeights != null && i < noiseHeights.Count)
                {
                    noiseHeight = noiseHeights[i];
                }
                else
                {
                    for (int o = 0; o < octaves; o++)
                    {
                        float xValue = (float)mesh.vertices[i].x / scale * frequency;
                        float yValue = (float)mesh.vertices[i].y / scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(xValue + offset.x + seed, yValue + offset.y + seed) * 2 - 1;

                        if (morePoints && perlinValue < 0.10f)
                        {
                            perlinValue = perlinValue + Random.Range(0.10f, 0.20f);
                        }

                        perlinValue *= dampening;

                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    // Track mesh vertice noiseHeight
                    TerrainNoiseDetail noiseDetail = new TerrainNoiseDetail { X = mesh.vertices[i].x, Y = mesh.vertices[i].y, Noise = noiseHeight };
                    terrainNoise.NoiseDetail.Add(noiseDetail);
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseHeight = (noiseHeight < 0f) ? noiseHeight * heightScale / 10f : noiseHeight * heightScale;

                heights.Add(noiseHeight);
            }
        }

        private void ShapeTerrainFromData()
        {
            minNoiseHeight = float.PositiveInfinity;
            maxNoiseHeight = float.NegativeInfinity;

            terrainNoise = new TerrainNoise();

            Dictionary<string, float> thoughtPointAreaMap = new Dictionary<string, float>();
            string terrainPointKey = "";
            int thoughtCount = 1;

            if (gridSectionCenterCubes != null)
            {
                if ((thoughtWorldTerrain != null) && (thoughtWorldTerrain.thoughtPoints != null))
                {
                    foreach (var thoughtPoint in thoughtWorldTerrain.thoughtPoints)
                    {
                        var coordinateKey = string.Format("{0},{1}", (int)thoughtPoint.x, (int)thoughtPoint.y);
                        if (gridSectionCenterCubes.ContainsKey(coordinateKey))
                        {
                            // Track grid sections that will be populated with thought points
                            GameObject thoughtPt = gridSectionCenterCubes[coordinateKey];

                            int startX = (int)thoughtPt.transform.localPosition.x - (int)(sectionGridSize * 0.75f);
                            int startY = (int)thoughtPt.transform.localPosition.z - (int)(sectionGridSize * 0.75f);

                            float[,] noiseMap = null;// Noise.GenerateNoiseMap((int)(sectionGridSize * 1.5f), (int)(sectionGridSize * 1.5f), seed + thoughtCount, heightScale, octaves, 0, lacunarity, Vector2.zero, Noise.NormalizeMode.Local);

                            // Add area surrounding thought point
                            for (int x = startX, xIndex = 0; x < (startX + (int)(sectionGridSize * 1.5f)); x++)
                            {
                                for (int y = startY, yIndex = 0; y < (startY + (int)(sectionGridSize * 1.5f)); y++)
                                {
                                    terrainPointKey = string.Format("{0},{1}", x, y);
                                    if (!thoughtPointAreaMap.ContainsKey(terrainPointKey))
                                    {
                                        thoughtPointAreaMap.Add(terrainPointKey, noiseMap[xIndex,yIndex]);
                                    }

                                    yIndex++;
                                }

                                xIndex++;
                            }

                            thoughtCount++;
                        }
                    }
                    
                }
            }

            for (int i = 0; i < mesh.vertices.Count; i++)
            {
                float amplitude = 1f;
                float frequency = 0.75f;
                float noiseHeight = 0f;

                terrainPointKey = string.Format("{0},{1}", (int)mesh.vertices[i].x, (int)mesh.vertices[i].y);
                if (thoughtPointAreaMap.ContainsKey(terrainPointKey))
                {
                    //float offsetX = thoughtPointAreaMap[terrainPointKey].transform.localPosition.x;
                    //float offsetY = thoughtPointAreaMap[terrainPointKey].transform.localPosition.y;
                    /*
                    for (int o = 0; o < octaves; o++)
                    {
                        float xValue = (float)mesh.vertices[i].x / scale * frequency;
                        float yValue = (float)mesh.vertices[i].y / scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(xValue + offset.x + seed, yValue + offset.y + seed) * 2 - 1;
                        //float perlinValue = Mathf.PerlinNoise(xValue + offsetX + seed, yValue + offsetY + seed) * 2 - 1;

                        //perlinValue = Random.Range(-0.001f, 0.001f);

                        perlinValue *= dampening;

                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }
                    */

                    noiseHeight = thoughtPointAreaMap[terrainPointKey];

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                }
                else
                {
                    if (minNoiseHeight < -0.1575f)
                    {
                        noiseHeight = Random.Range(minNoiseHeight, minNoiseHeight + 0.10f);
                    }
                    else
                    {
                        noiseHeight = Random.Range(0f, 0.005f);
                    }

                    noiseHeight = Random.Range(0f, 0.005f);
                }

                // Track mesh vertice noiseHeight
                TerrainNoiseDetail noiseDetail = new TerrainNoiseDetail { X = mesh.vertices[i].x, Y = mesh.vertices[i].y, Noise = noiseHeight };
                terrainNoise.NoiseDetail.Add(noiseDetail);

                noiseHeight = (noiseHeight < 0f) ? noiseHeight * heightScale / 8f : noiseHeight * heightScale;

                heights.Add(noiseHeight);
            }
        }

        public void UpdateShape(float scale)
        {
            heightScale = scale;
            //Initiate();
            GenerateTerrain(false);
        }

        private void GenerateMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> colors = new List<Color>();
            List<int> triangles = new List<int>();

            IEnumerator<Triangle> triangleEnum = mesh.Triangles.GetEnumerator();

            for (int i = 0; i < mesh.Triangles.Count; i++)
            {
                if (!triangleEnum.MoveNext())
                {
                    break;
                }

                Triangle currentTriangle = triangleEnum.Current;

                Vector3 v0 = new Vector3((float)currentTriangle.vertices[2].x, heights[currentTriangle.vertices[2].id], (float)currentTriangle.vertices[2].y);
                Vector3 v1 = new Vector3((float)currentTriangle.vertices[1].x, heights[currentTriangle.vertices[1].id], (float)currentTriangle.vertices[1].y);
                Vector3 v2 = new Vector3((float)currentTriangle.vertices[0].x, heights[currentTriangle.vertices[0].id], (float)currentTriangle.vertices[0].y);

                triangles.Add(vertices.Count);
                triangles.Add(vertices.Count + 1);
                triangles.Add(vertices.Count + 2);

                vertices.Add(v0);
                vertices.Add(v1);
                vertices.Add(v2);

                var normal = Vector3.Cross(v1 - v0, v2 - v0);

                var triangleColor = EvaluateColor(currentTriangle);

                for (int x = 0; x < 3; x++)
                {
                    normals.Add(normal);
                    uvs.Add(Vector3.zero);
                    colors.Add(triangleColor);
                }
            }

            terrainMesh = new UnityEngine.Mesh();
            terrainMesh.vertices = vertices.ToArray();
            terrainMesh.uv = uvs.ToArray();
            terrainMesh.triangles = triangles.ToArray();
            terrainMesh.colors = colors.ToArray();
            terrainMesh.normals = normals.ToArray();

            transform.GetComponent<MeshFilter>().mesh = terrainMesh;
            transform.GetComponent<MeshCollider>().sharedMesh = terrainMesh;
            transform.GetComponent<MeshRenderer>().material = material;
        }

        private Color EvaluateColor(Triangle triangle)
        {
            var currentHeight = heights[triangle.vertices[0].id] + heights[triangle.vertices[1].id] + heights[triangle.vertices[2].id];
            currentHeight /= 3f;

            switch (colorSetting)
            {
                case ColorSetting.Random:

                    return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                case ColorSetting.HeightGradient:

                    currentHeight = (currentHeight < 0f) ? currentHeight / heightScale * 10f : currentHeight / heightScale;

                    var gradientVal = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, currentHeight);
                    return heightGradient.Evaluate(gradientVal);

            }

            return Color.magenta;
        }

        public void SaveMesh()
        {
            if (transform.GetComponent<MeshFilter>() != null)
            {
                #if UNITY_EDITOR
                var path = "Assets/GeneratedMesh" + seed.ToString() + ".asset";
                AssetDatabase.CreateAsset(transform.GetComponent<MeshFilter>().sharedMesh, path);
                #endif
            }
        }

        private void WritePolygonDataToFile(Polygon polygon)
        {
            PolygonPoints polygonPoints = new PolygonPoints();
            polygonPoints.Points = new List<PolygonPoint>();

            if (polygon != null)
            {
                foreach (Point p in polygon.Points)
                {
                    polygonPoints.Points.Add(new PolygonPoint { X = p.X, Y = p.Y });
                }
            }

            if (!string.IsNullOrEmpty(polygonFileName))
            {
                using (StreamWriter file = new StreamWriter(polygonFileName))
                {
                    string pointsList = JsonUtility.ToJson(polygonPoints, true);
                    file.Write(pointsList);
                }
            }
        }

        private Polygon ReadPolygonDataFromJson(string jsonText)
        {
            Polygon polygon = new Polygon();

            if (!string.IsNullOrEmpty(jsonText))
            {
                var polygonPoints = JsonUtility.FromJson<PolygonPoints>(jsonText);
                if (polygonPoints != null)
                {
                    foreach (PolygonPoint point in polygonPoints.Points)
                    {
                        polygon.Add(new Vertex(point.X, point.Y));
                    }
                }
            }
            
            return polygon;
        }

        private void WriteHeightDataToFile(TerrainNoise noiseDetails)
        {
            //TerrainNoise tn = new TerrainNoise();
            //noiseDetails.NoiseDetail.Sort();

            using (StreamWriter file = new StreamWriter(noiseDataFileName))
            {
                string n = JsonUtility.ToJson(noiseDetails, true);
                file.Write(n);
            }
        }

        private List<float> ReadNoiseDataFromJson(string jsonText)
        {
            List<float> noiseHeights = null;

            if (!string.IsNullOrEmpty(jsonText))
            {
                var terrainNoise = JsonUtility.FromJson<TerrainNoise>(jsonText);
                if (terrainNoise != null)
                {
                    noiseHeights = new List<float>();
                    foreach (var n in terrainNoise.NoiseDetail)
                    {
                        noiseHeights.Add((float)n.Noise);
                    }
                }
            }

            return noiseHeights;
        }
    }
}
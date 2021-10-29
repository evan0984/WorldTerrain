using UnityEngine;
using System.Collections;

namespace ThoughtWorld.Terrain
{
	public class MapPreview : MonoBehaviour
	{
		public Renderer textureRender;
		public MeshFilter meshFilter;
		public MeshRenderer meshRenderer;

		public enum DrawMode { NoiseMap, Mesh, FalloffMap };
		public DrawMode drawMode;

		public MeshSettings meshSettings;
		public HeightMapSettings heightMapSettings;
		public TextureData textureData;

		public Material terrainMaterial;

		[Range(0, MeshSettings.numSupportedLODs - 1)]
		public int editorPreviewLOD;
		public bool autoUpdate;

		float[,] falloffMap;

		private void Awake()
		{
			falloffMap = FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine);
		}

		public void DrawMapInEditor()
		{
			textureData.ApplyToMaterial(terrainMaterial);
			textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
			HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero, null);

			if (drawMode == DrawMode.NoiseMap)
			{
				DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
			}
			else if (drawMode == DrawMode.Mesh)
			{
				DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
			}
			else if (drawMode == DrawMode.FalloffMap)
			{
				DrawTexture(TextureGenerator.TextureFromHeightMap(HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero, falloffMap)));
			}
		}

		public void DrawTexture(Texture2D texture)
		{
			textureRender.sharedMaterial.mainTexture = texture;
			textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

			textureRender.gameObject.SetActive(true);
			meshFilter.gameObject.SetActive(false);
		}

		public void DrawMesh(MeshData meshData)
		{
			meshFilter.sharedMesh = meshData.CreateMesh();

			textureRender.gameObject.SetActive(false);
			meshFilter.gameObject.SetActive(true);
		}

		void OnValuesUpdated()
		{
			if (!Application.isPlaying)
			{
				DrawMapInEditor();
			}
		}

		void OnTextureValuesUpdated()
		{
			textureData.ApplyToMaterial(terrainMaterial);
		}

		void OnValidate()
		{
			if (meshSettings != null)
			{
				meshSettings.OnValuesUpdated -= OnValuesUpdated;
				meshSettings.OnValuesUpdated += OnValuesUpdated;
			}
			if (heightMapSettings != null)
			{
				heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
				heightMapSettings.OnValuesUpdated += OnValuesUpdated;
			}
			if (textureData != null)
			{
				textureData.OnValuesUpdated -= OnTextureValuesUpdated;
				textureData.OnValuesUpdated += OnTextureValuesUpdated;
			}
		}
	}
}
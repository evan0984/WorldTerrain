using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThoughtWorld.Terrain.Model;

namespace ThoughtWorld.Terrain
{
	public static class HeightMapGenerator
	{
		public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre, float[,] falloffMap=null, float? maxHeight=null)
		{
			float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);

			AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

			float minValue = float.MaxValue;
			float maxValue = float.MinValue;
			//bool doClamp = true;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (falloffMap != null)
					{
						if (maxHeight.HasValue)
						{
							float heightValue = values[i, j] - falloffMap[i, j];

							var pointHeight = heightCurve_threadsafe.Evaluate(heightValue) * settings.heightMultiplier;
							var maxPointHeight = maxHeight.Value* settings.heightMultiplier;

							if (pointHeight > maxPointHeight)
							{
								values[i, j] = Mathf.Clamp(pointHeight, 0, maxPointHeight);
							}
							//else if (pointHeight < (0.098f * settings.heightMultiplier))
							//{
							//	values[i, j] = Mathf.Clamp(pointHeight, (0.065f * settings.heightMultiplier), (0.098f * settings.heightMultiplier));
							//}
							else
                            {
								values[i, j] = pointHeight;
							}

							if (values[i, j] == maxPointHeight)
                            {
								//doClamp = false;
							}

							//if (settings.noiseSettings.normalizeMode == Noise.NormalizeMode.Global)
							//{
							//	values[i, j] = Mathf.Clamp(values[i, j], 0.5f, int.MaxValue);
							//}

							// Temporarily used for center height point
							if (i == width/2 && j == height/2)
                            {
								values[i, j] = maxPointHeight;
							}
						}
						else
                        {
							values[i, j] = heightCurve_threadsafe.Evaluate(values[i, j] - falloffMap[i, j]) * settings.heightMultiplier;
						}
					}
					else
					{
						values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;
					}

					if (values[i, j] > maxValue)
					{
						maxValue = values[i, j];
					}
					if (values[i, j] < minValue)
					{
						minValue = values[i, j];
					}
				}
			}

			return new HeightMap(values, minValue, maxValue);
		}

		public static HeightMap GenerateEmptyTerrainHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre, float[,] falloffMap = null, float? maxHeight = null)
		{
			float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);

			AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

			float minValue = float.MaxValue;
			float maxValue = float.MinValue;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					values[i, j] = 0.093f * settings.heightMultiplier;

					if ((falloffMap != null) && (maxHeight.HasValue))
					{
						var maxPointHeight = maxHeight.Value * settings.heightMultiplier;
						values[i, j] = maxPointHeight;

						float heightValue = values[i, j] - falloffMap[i, j];

                        var pointHeight = heightCurve_threadsafe.Evaluate(heightValue) * settings.heightMultiplier;

                        if (pointHeight > maxPointHeight)
                        {
                            values[i, j] = Mathf.Clamp(pointHeight, 0, maxPointHeight);
                        }
                    }
				}
			}

			return new HeightMap(values, minValue, maxValue);
		}

		public static HeightMap GenerateNoTerrainHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre)
		{
			float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);

			float minValue = float.MaxValue;
			float maxValue = float.MinValue;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					// Assign lowest point on terrain (sea level)
					values[i, j] = 0f;
				}
			}

			return new HeightMap(values, minValue, maxValue);
		}
	}

	public struct HeightMap
	{
		public readonly float[,] values;
		public readonly float minValue;
		public readonly float maxValue;

		public HeightMap(float[,] values, float minValue, float maxValue)
		{
			this.values = values;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
}
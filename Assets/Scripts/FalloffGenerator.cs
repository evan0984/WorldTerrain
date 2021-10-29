using UnityEngine;
using System.Collections;
using ThoughtWorld.Terrain.Model;

namespace ThoughtWorld.Terrain
{
	public static class FalloffGenerator
	{
		public static float[,] GenerateFalloffMap(int width, int height, float weight=1f, ThoughtPointAdjacency thoughtPointAdjacency=null)
		{
			float[,] values = new float[width, height];

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					// Create fall off based on weight and from center outward
					float x = i / (float)width * 2 - 1;
					float y = j / (float)height * 2 - 1;

					float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

					values[i, j] = Evaluate(value, weight);

					// When weight is > 0.85, determine if falloff area is reduced to allow connection of adjacent terrains
					if (weight > 0.85f && thoughtPointAdjacency != null)
					{
						if (thoughtPointAdjacency.adjacentTop)
						{
							if ((j < (height * 0.25f))) //&&
								//(i > (width * 0.15f) &&
								//(i < (width * 0.85f))))
							{
								values[i, j] = 0; // 0 means no falloff at i, j point
							}
						}

						if (thoughtPointAdjacency.adjacentBottom)
                        {
							if ((j > (height * 0.75f))) //&&
								//(i > (width * 0.15f) &&
								//(i < (width * 0.85f))))
							{
								values[i, j] = 0;
							}
						}

						if (thoughtPointAdjacency.adjacentLeft)
                        {
							if ((i < (width * 0.25f))) //&&
								//(j > (height * 0.15f) &&
								//(j < (height * 0.85f))))
							{
								values[i, j] = 0;
							}

							// Test
							if ((j < (height * 0.25f)) &&
								(i < (width * 0.15f)))
							{
								values[i, j] = values[i + (width / 4), j + (height / 4)];
							}
						}

						if (thoughtPointAdjacency.adjacentRight)
                        {
							if ((i > (width * 0.75f))) //&&
								//(j > (height * 0.15f) &&
								//(j < (height * 0.85f))))
							{
								values[i, j] = 0;
							}

							if ((j < (height * 0.25f)) &&
								(i > (width * 0.85f)))
							{
								values[i, j] = values[i - (width / 4), j];
							}
						}
					}
				}
			}
			
			return values;
		}

		static float EvaluateFalloffNoisePoint(int width, int height, int widthPoint, int heightPoint, float weight)
        {
			float x = widthPoint / (float)width * 2 - 1;
			float y = heightPoint / (float)height * 2 - 1;

			float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

			return Evaluate(value, weight);
		}

		static float Evaluate(float value, float weight=1f)
		{
			float a = 3f;
			float b = 10f*(weight == 0 ? 1f : weight/1.5f);

			return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
		}
	}
}

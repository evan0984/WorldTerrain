using System;
using System.Collections.Generic;

namespace ThoughtWorld.Terrain.Model
{
    [Serializable]
    public class TerrainNoise
    {
        public List<TerrainNoiseDetail> NoiseDetail;

        public TerrainNoise()
        {
            NoiseDetail = new List<TerrainNoiseDetail>();
        }
    }
}

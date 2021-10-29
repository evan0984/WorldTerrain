using UnityEngine;
using System;

namespace ThoughtWorld.Terrain.Model
{
    [Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }
}

using System;
using System.Collections.Generic;

namespace ThoughtWorld.Terrain.Model
{
    [Serializable]
    public class ThoughtWorldTerrain
    {
        /// <summary>
        /// Guid string representing terrain landscape
        /// </summary>
        public string terrainID;

        /// <summary>
        /// Size X of terrain
        /// </summary>
        public int sizeX;

        /// <summary>
        /// Size Y of terrain
        /// </summary>
        public int sizeY;

        /// <summary>
        /// Thought terrain name
        /// </summary>
        public string thoughtTerrainName;

        /// <summary>
        /// Thought category
        /// </summary>
        public int thoughtCategory;

        /// <summary>
        /// List of thought points on the terrain
        /// </summary>
        public List<ThoughtPoint> thoughtPoints;
    }
}

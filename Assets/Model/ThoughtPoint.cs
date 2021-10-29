using System;

namespace ThoughtWorld.Terrain.Model
{
    [Serializable]
    public class ThoughtPoint
    {
        /// <summary>
        /// Guid string representing thought 
        /// </summary>
        public string thoughtID;

        /// <summary>
        /// X position of thought on terrain grid
        /// </summary>
        public float x;

        /// <summary>
        /// Y position of thought on terrain grid
        /// </summary>
        public float y;

        /// <summary>
        /// X offset position of thought on terrain grid (default is center 0)
        /// </summary>
        public float xOffset;

        /// <summary>
        /// Y offset position of thought on terrain grid (default is center 0)
        /// </summary>
        public float yOffset;

        /// <summary>
        /// Calculated height / altitude of the thought
        /// </summary>
        public float height;

        /// <summary>
        /// Calculated weight / impact of the thought
        /// </summary>
        public float weight;

        /// <summary>
        /// Display text for thought height 
        /// </summary>
        public string heightDisplayText;

        /// <summary>
        /// Display text for thought weight
        /// </summary>
        public string weightDisplayText;

        /// <summary>
        /// Label used for displaying short name of thought
        /// </summary>
        public string thoughtLabel;

        /// <summary>
        /// Display text for thought description
        /// </summary>
        public string thoughtDescription;

        /// <summary>
        /// Clickable URL for navigating to thought detail 
        /// </summary>
        public string thoughtUrl;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ThoughtWorld.Terrain.Model
{
    public class ThoughtPointAdjacency
    {
        public ThoughtPoint thoughtPoint;
        public bool adjacentTop;
        public bool adjacentBottom;
        public bool adjacentLeft;
        public bool adjacentRight;

        public ThoughtPointAdjacency(ThoughtPoint thoughtPoint)
        {
            this.thoughtPoint = thoughtPoint;
            this.adjacentTop = false;
            this.adjacentBottom = false;
            this.adjacentLeft = false;
            this.adjacentRight = false;
        }

        public ThoughtPointAdjacency(ThoughtPoint thoughtPoint, bool adjacentTop, bool adjacentBottom, bool adjacentLeft, bool adjacentRight)
        {
            this.thoughtPoint = thoughtPoint;
            this.adjacentTop = adjacentTop;
            this.adjacentBottom = adjacentBottom;
            this.adjacentLeft = adjacentLeft;
            this.adjacentRight = adjacentRight;
        }
    }
}

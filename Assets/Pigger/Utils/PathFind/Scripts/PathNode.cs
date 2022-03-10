using System;


namespace Pigger.Utils.PathFind
{
    [Serializable]
    public class PathNode
    {
        private int x;
        private int y;

        public int gCost;
        public int hCost;
        public int fCost;

        public int X { get => x; }
        public int Y { get => y; }

        public PathNode cameFromNode;
        public bool isWalkable;

        public PathNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }

}

using Pigger.Utils.Structs;
using System;


namespace Pigger.Utils.PathFind
{
    [Serializable]
    public class PathNode
    {
        //private int x;
        //private int y;

        public int gCost;
        public int hCost;
        public int fCost;

        //public int X { get => x; }
        //public int Y { get => y; }

        public PathNode cameFromNode;
        public bool isWalkable;
        public Point Point { get => point; }
        private Point point;


        public PathNode(int x, int y)
        {
            point = new Point(x, y);
            //this.x = x;
            //this.y = y;        
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        public void SetDefaultValues()
        {
            gCost = int.MaxValue;
            hCost = 0;
            CalculateFCost();
            cameFromNode = null;
        }
    }
}

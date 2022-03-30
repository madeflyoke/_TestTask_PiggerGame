using System;

namespace Pigger.Utils.Structs
{
    [Serializable]
    public struct Point
    {
        private int x;
        private int y;
        public int X { get => x; }
        public int Y { get => y; }

        public Point(int x, int y)
        {
            this.x=x;
            this.y=y;
        }
    }
}


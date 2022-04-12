using Pigger.Utils.Structs;
using UnityEngine;

namespace Pigger.Utils.Grid
{
    [RequireComponent(typeof(Collider2D))]
    public class GridCell : MonoBehaviour
    {
        public bool IsWalkable {get => isWalkable;
           
            private set 
            {
                isWalkable = value;
                //gameObject.layer = isWalkable? LayerMask.NameToLayer("Walkable"):  //layer if needed
                //    LayerMask.NameToLayer("NotWalkable");
                if (value == false)
                {
                    GetComponent<SpriteRenderer>().color = Color.red;
                }
            } 
        }
        public Point Point { get=> point; }
        private Point point;
        private Collider2D coll;
        private bool isWalkable;

        private void Awake()
        {
            coll = GetComponent<Collider2D>();
            coll.isTrigger = true;
        }

        public void Initialize(int x, int y, bool isWalkable)
        {
            point = new Point(x, y);
            IsWalkable = isWalkable;
        }
    }
}

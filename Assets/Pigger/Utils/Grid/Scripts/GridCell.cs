using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pigger.Utils.Grid
{
    [RequireComponent(typeof(Collider2D))]
    public class GridCell : MonoBehaviour
    {
        public int X { get => x; }
        public int Y { get => y; }
        public bool IsWalkable {get => isWalkable;
           
            private set 
            {
                isWalkable = value;
                //gameObject.layer = isWalkable? LayerMask.NameToLayer("Walkable"):  //if needed
                //    LayerMask.NameToLayer("NotWalkable");
                if (value == false)
                {
                    GetComponent<SpriteRenderer>().color = Color.red;
                }
            } 
        }

        private int x;
        private int y;
        private Collider2D coll;
        private bool isWalkable;
        private void Awake()
        {
            coll = GetComponent<Collider2D>();
            coll.isTrigger = true;
        }

        public void Initialize(int x, int y, bool isWalkable)
        {
            this.x = x;
            this.y = y;
            IsWalkable = isWalkable;
        }

    }
}

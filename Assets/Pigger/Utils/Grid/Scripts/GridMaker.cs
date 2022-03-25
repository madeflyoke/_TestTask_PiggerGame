using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pigger.Utils.PathFind;


namespace Pigger.Utils.Grid
{
    public class GridMaker : MonoBehaviour
    {
        private const float _stepX = 0.095f;  //individual grid map corrections
        private const float _offsetNextLine = 0.13f;

        [SerializeField] private AStarPathFinder pathFinder;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private Vector2 bottomLeftCornerPoint;
        [SerializeField] private GridCell cellPrefab;
        [SerializeField] private bool drawGizmos;
        public int Width { get => width; }
        public int Height { get => height; }
        public List<GridCell> gridCells { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            gridCells = new List<GridCell>();
            Vector2 startPos = bottomLeftCornerPoint;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 pos = startPos + new Vector2(x + (x * _stepX), y * 0.98f);
                    GridCell cellObject = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                    cellObject.Initialize(x, y,
                        Physics2D.OverlapCircle(pos, 0.2f, LayerMask.GetMask("Obstacle")) ? false : true);
                    gridCells.Add(cellObject);
                }
                startPos += new Vector2(_offsetNextLine, 0f);
            }            
        }

        public GridCell GetGridCell(int x, int y)
        {
            GridCell cell = gridCells.Where((s) => s.X == x && s.Y == y).FirstOrDefault();
            if (cell == null)
            {
                return null;
            }
            return cell;
        }

        public GridCell FindNearestCell(Vector2 worldPos)
        {
            GridCell cell;
            float searchRadius = 0.1f;
            while (true)
            {
                Collider2D coll = Physics2D.OverlapCircle(worldPos, radius: searchRadius, LayerMask.GetMask("Grid"));
                if (coll != null && coll.TryGetComponent(out cell))
                {
                    if (cell.IsWalkable)
                    {
                        break;
                    }
                }
                searchRadius += 0.05f; //prevent infinite loop 
                if (searchRadius > 200f)
                {
                    Debug.LogError("NEAREST CELL WAS NOT FOUND");
                    return null;
                }
            }
            if (cell != null)
            {
                return cell;
            }
            return null;
        }

        private void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                Vector2 startPos = bottomLeftCornerPoint;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Vector2 pos = startPos + new Vector2(x + (x * _stepX), y * 0.98f);
                        Gizmos.color = Physics2D.OverlapCircle(pos, 0.2f, LayerMask.GetMask("Obstacle")) ? Color.red : Color.green;
                        Gizmos.DrawWireSphere(pos, 0.4f);
                    }
                    startPos += new Vector2(_offsetNextLine, 0f);
                }
            }
        }
    }
}


using Pigger.Utils.Grid;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using Pigger.GamePlay.Units.MainCharacter;
using Cysharp.Threading.Tasks;

namespace Pigger.GamePlay.Points
{
    public class PointsSpawner : MonoBehaviour
    {
        [Inject] private GridMaker grid;
        [Inject] private PlayerController player;
        
        [SerializeField] private GameObject pointsPrefab;
        [SerializeField] private int spawnCount;

        private Dictionary<GridCell, GameObject> cellPoints; 
        private List<GridCell> walkableCells;
        public bool CanSpawn { get; set; }

        public void Start()
        {
            cellPoints = new Dictionary<GridCell, GameObject>();
            GridCell playerCell = grid.FindNearestCell(player.transform.position);
            walkableCells = grid.gridCells.Where((x)=>x.IsWalkable&&x!=playerCell).Select((x)=>x).ToList();

            for (int i = 0; i < spawnCount; i++)
            {
                int loopCheck = 0;
                while (true)
                {
                    int randomIndex = Random.Range(0, walkableCells.Count - 1);
                    loopCheck++;
                    if (loopCheck>=100)
                    {
                        Debug.LogError("CANT SPAWN POINTS ON GRID");
                        return;
                    }
                    if (cellPoints.ContainsKey(walkableCells[randomIndex]))
                    {
                        continue;
                    }
                    if (walkableCells[randomIndex] != null && walkableCells[randomIndex].IsWalkable == true)
                    {
                        GameObject point = Instantiate(pointsPrefab,
                            walkableCells[randomIndex].transform.position,
                            Quaternion.identity, transform);
                        cellPoints[walkableCells[randomIndex]] = point;
                        break;
                    }
                }                             
            }          
        }
    }
}


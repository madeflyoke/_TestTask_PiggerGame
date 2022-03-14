using Pigger.Utils.Grid;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Pigger.Utils.PathFind
{
    public class AStarPathFinder : MonoBehaviour
    {
        private const int _straightMoveCost = 10;
        private const int _diagonalMoveCost = 14;

        [SerializeField] private GridMaker grid;
        [SerializeField] private bool canDiagonalMove;
        private List<PathNode> openList;
        private List<PathNode> closedList;
        private List<PathNode> nodes;

        public List<Vector2> GetRandomWorldPath(Vector2 followerPos)
        {
            GridCell cellPos = grid.gridCells.Where(s => s.IsWalkable).
                ElementAt(Random.Range(0, grid.gridCells.Where(s => s.IsWalkable).Count()));
            GridCell nearestStartCell = grid.FindNearestCell(followerPos);           
            List<PathNode> path = FindPath(nearestStartCell.X, nearestStartCell.Y, cellPos.X, cellPos.Y);
            List<Vector2> newPath = NodePathToWorldPath(path);
            if (newPath == null)
            {
                Debug.LogError("PATH HAD NOT BEEN FOUND");
                return null;
            }
            for (int i = 0; i < newPath.Count - 1; i++)
            {
                Debug.DrawLine(new Vector2(newPath[i].x, newPath[i].y),
                    new Vector2(newPath[i + 1].x, newPath[i + 1].y), Color.red, 30f);
            }
            return newPath;
        }

        public List<Vector2> GetWorldPath(Vector2 followerPos, Vector2 destinationPos)
        {
            GridCell cellPosA = grid.FindNearestCell(followerPos);
            GridCell cellPosB = grid.FindNearestCell(destinationPos);
            if (cellPosA==null||cellPosB==null)
            {
                Debug.LogError("PATH HAD NOT BEEN FOUND");
                return null;
            }
            List<PathNode> path = FindPath(cellPosA.X, cellPosA.Y, cellPosB.X, cellPosB.Y);
            List<Vector2> newPath = NodePathToWorldPath(path);
            if (newPath==null||newPath.Count<=0)
            {
                Debug.LogError("PATH HAD NOT BEEN FOUND");
                return null;
            }
            if (newPath.Count == 1)
            {
                newPath.Add(followerPos);
                newPath.Reverse();
            }
            for (int i = 0; i < newPath.Count - 1; i++)
            {
                Debug.DrawLine(new Vector2(newPath[i].x, newPath[i].y),
                    new Vector2(newPath[i + 1].x, newPath[i + 1].y), Color.red, 30f);
            }
            return newPath;

        }
        private List<PathNode> FindPath(int startX, int startY, int endX, int endY)
        {
            closedList = new List<PathNode>();
            nodes = new List<PathNode>();
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    PathNode pathNode = new PathNode(x, y);
                    pathNode.isWalkable = grid.GetGridCell(x,y).IsWalkable ? true : false;
                    nodes.Add(pathNode);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.cameFromNode = null;
                }
            }
            PathNode startNode = GetNode(startX, startY);
            PathNode endNode = GetNode(endX, endY);
            openList = new List<PathNode>();
            openList.Add(startNode);
            startNode.gCost = 0;
            startNode.hCost = CalculateDistance(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if (currentNode.X == endNode.X && currentNode.Y == endNode.Y)
                {
                    return CalculatePath(endNode);
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neighbourNode))
                    {
                        continue;
                    }
                    if (neighbourNode.isWalkable == false)
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }
                    int tmpGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                    if (tmpGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tmpGCost;
                        neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();
                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }
            return null;
        }

        private List<Vector2> NodePathToWorldPath(List<PathNode> pathNodes)
        {
            List<Vector2> worldPosList = new List<Vector2>();
            foreach (PathNode node in pathNodes)
            {
                worldPosList.Add(grid.GetGridCell(node.X, node.Y).transform.position);
            }
            return worldPosList;

        }

        private PathNode GetNode(int x, int y)
        {
            return nodes.Where((s) => s.X == x && s.Y == y).First();
        }

        private List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            if (currentNode.X - 1 >= 0)
            {
                //Left
                neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y));
                if (canDiagonalMove)
                {
                    // LeftDown
                    if (currentNode.Y - 1 >= 0)
                    {
                        neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y - 1));
                    }
                    // LeftUp
                    if (currentNode.Y + 1 < grid.Height)
                    {
                        neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y + 1));
                    }
                }
            }

            if (currentNode.X + 1 < grid.Width)
            {
                //Right
                neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y));

                if (canDiagonalMove)
                {
                    //RightDown
                    if (currentNode.Y - 1 >= 0)
                    {
                        neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y - 1));
                    }
                    //RightUp
                    if (currentNode.Y + 1 < grid.Height)
                    {
                        neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y + 1));
                    }
                }
            }
            //Down
            if (currentNode.Y - 1 >= 0)
            {
                neighbourList.Add(GetNode(currentNode.X, currentNode.Y - 1));
            }
            //Up
            if (currentNode.Y + 1 < grid.Height)
            {
                neighbourList.Add(GetNode(currentNode.X, currentNode.Y + 1));
            }
            return neighbourList;
        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;
            while (currentNode.cameFromNode != null)
            {
                path.Add(currentNode.cameFromNode);
                currentNode = currentNode.cameFromNode;
            }
            path.Reverse();
            return path;
        }

        private int CalculateDistance(PathNode a, PathNode b)
        {
            int xDistance = Mathf.Abs(a.X - b.X);
            int yDistance = Mathf.Abs(a.Y - b.Y);
            int remain = Mathf.Abs(xDistance - yDistance);
            return _diagonalMoveCost * Mathf.Min(xDistance, yDistance) + _straightMoveCost * remain;
        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode node = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].fCost < node.fCost)
                {
                    node = pathNodeList[i];
                }
            }
            return node;
        }
    }
}


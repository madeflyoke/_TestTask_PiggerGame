using Cysharp.Threading.Tasks;
using Pigger.Utils.Grid;
using System;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;

namespace Pigger.GamePlay.Units.MainCharacter.Utils
{
    public class Bomb : MonoBehaviour
    {
        [Serializable]
        public struct ExplosionRay
        {
            public SpriteRenderer start;
            public SpriteRenderer mid;
            public SpriteRenderer end;
        }

        [SerializeField] private float explosionDelay;
        [SerializeField] private float explosionDuration;
        [SerializeField] private GameObject rays;
        [SerializeField] private ExplosionRay up;
        [SerializeField] private ExplosionRay down;
        [SerializeField] private ExplosionRay left;
        [SerializeField] private ExplosionRay right;
        [SerializeField] private SpriteRenderer innerExplosion;
        private GridMaker grid;
        private CancellationTokenSource cancellationSource;
        private PlayerController player;
        private List<ExplosionRay> explosionRays;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            explosionRays = new List<ExplosionRay>() { up, down, left, right };
            grid = FindObjectOfType<GridMaker>();
            player = FindObjectOfType<PlayerController>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            cancellationSource = new CancellationTokenSource();
            OffRays();
            gameObject.SetActive(false);
        }

        public async void SetExplosion()
        {
            GridCell currentCell = grid.FindNearestCell(player.transform.position);
            transform.position = currentCell.transform.position;
            spriteRenderer.enabled = true;
            gameObject.SetActive(true);
            //up
            int upStages = 0;
            GridCell startUpCell = grid.GetGridCell(currentCell.X, currentCell.Y + 1);
            if (startUpCell!=null&&startUpCell.IsWalkable)
            {
                upStages++;
                GridCell midUpCell = grid.GetGridCell(currentCell.X, currentCell.Y + 2);
                if (midUpCell!=null&&midUpCell.IsWalkable)
                {
                    upStages++;
                    GridCell endUpCell = grid.GetGridCell(currentCell.X, currentCell.Y + 3);
                    if (endUpCell!=null&&endUpCell.IsWalkable)
                    {
                        upStages++;
                    }
                }
            }
            //down
            int downStages = 0;
            GridCell startDownCell = grid.GetGridCell(currentCell.X, currentCell.Y - 1);
            if (startDownCell!=null&&startDownCell.IsWalkable)
            {
                downStages++;
                GridCell midDownCell = grid.GetGridCell(currentCell.X, currentCell.Y - 2);
                if (midDownCell!=null&&midDownCell.IsWalkable)
                {
                    downStages++;
                    GridCell endDownCell = grid.GetGridCell(currentCell.X, currentCell.Y - 3);
                    if (endDownCell!=null&&endDownCell.IsWalkable)
                    {
                        downStages++;
                    }
                }
            }
            //left
            int leftStages = 0;
            GridCell startLeftCell = grid.GetGridCell(currentCell.X - 1, currentCell.Y);
            if (startLeftCell != null && startLeftCell.IsWalkable)
            {
                leftStages++;
                GridCell midLeftCell = grid.GetGridCell(currentCell.X - 2, currentCell.Y);
                if (midLeftCell!=null&&midLeftCell.IsWalkable)
                {
                    leftStages++;
                    GridCell endLeftCell = grid.GetGridCell(currentCell.X - 3, currentCell.Y);
                    if (endLeftCell!=null&&endLeftCell.IsWalkable)
                    {
                        leftStages++;
                    }
                }
            }
            //right
            int rightStages = 0;
            GridCell startRightCell = grid.GetGridCell(currentCell.X + 1, currentCell.Y);
            if (startRightCell!=null&&startRightCell.IsWalkable)
            {
                rightStages++;
                GridCell midRightCell = grid.GetGridCell(currentCell.X + 2, currentCell.Y);
                if (midRightCell!=null&&midRightCell.IsWalkable)
                {
                    rightStages++;
                    GridCell endRightCell = grid.GetGridCell(currentCell.X + 3, currentCell.Y);
                    if (endRightCell!=null&&endRightCell.IsWalkable)
                    {
                        rightStages++;
                    }
                }
            }
            await UniTask.Delay((int)(explosionDelay * 1000), cancellationToken: cancellationSource.Token);
            spriteRenderer.enabled = false;
            SetRays(upStages, downStages, leftStages, rightStages);
            await UniTask.Delay((int)(explosionDuration * 1000), cancellationToken: cancellationSource.Token);
            OffRays();
            gameObject.SetActive(false);
        }

        private void SetRays(int upStages, int downStages, int leftStages, int rightStages)
        {
            Dictionary<ExplosionRay, float> raysStages = new Dictionary<ExplosionRay, float>();
            raysStages[up] = upStages;
            raysStages[down] = downStages;
            raysStages[left] = leftStages;
            raysStages[right] = rightStages;

            foreach (KeyValuePair<ExplosionRay, float> item in raysStages)
            {
                if (item.Value == 0)
                {
                    continue;
                }
                else if (item.Value == 1)
                {
                    item.Key.start.gameObject.SetActive(true);
                }
                else if (item.Value == 2)
                {
                    item.Key.start.gameObject.SetActive(true);
                    item.Key.mid.gameObject.SetActive(true);
                }
                else if (item.Value == 3)
                {
                    item.Key.start.gameObject.SetActive(true);
                    item.Key.mid.gameObject.SetActive(true);
                    item.Key.end.gameObject.SetActive(true);
                }
            }
            rays.SetActive(true);
        }

        private void OffRays()
        {
            rays.SetActive(false);
            foreach (var item in explosionRays)
            {
                item.start.gameObject.SetActive(false);
                item.mid.gameObject.SetActive(false);
                item.end.gameObject.SetActive(false);
            }
        }
    }
}


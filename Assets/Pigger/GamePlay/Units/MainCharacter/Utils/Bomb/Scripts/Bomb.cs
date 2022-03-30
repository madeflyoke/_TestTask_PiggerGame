using Cysharp.Threading.Tasks;
using Pigger.Utils.Grid;
using System;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using Zenject;
using DG.Tweening;

namespace Pigger.GamePlay.Units.MainCharacter.Utils
{
    public class Bomb : MonoBehaviour
    {
        [Inject] private PlayerController player;
        [Inject] private GridMaker grid;

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
        private CancellationTokenSource cancellationSource;
        private List<ExplosionRay> explosionRays;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb;

        private void Awake()
        {
            explosionRays = new List<ExplosionRay>() { up, down, left, right };
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
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
            GridCell startUpCell = grid.GetGridCell(currentCell.Point.X, currentCell.Point.Y + 1);
            if (startUpCell!=null&&startUpCell.IsWalkable)
            {
                upStages++;
                GridCell midUpCell = grid.GetGridCell(currentCell.Point.X, currentCell.Point.Y + 2);
                if (midUpCell!=null&&midUpCell.IsWalkable)
                {
                    upStages++;
                    GridCell endUpCell = grid.GetGridCell(currentCell.Point.X, currentCell.Point.Y + 3);
                    if (endUpCell!=null&&endUpCell.IsWalkable)
                    {
                        upStages++;
                    }
                }
            }
            //down
            int downStages = 0;
            GridCell startDownCell = grid.GetGridCell(currentCell.Point.X, currentCell.Point.Y - 1);
            if (startDownCell!=null&&startDownCell.IsWalkable)
            {
                downStages++;
                GridCell midDownCell = grid.GetGridCell(currentCell.Point.X, currentCell.Point.Y - 2);
                if (midDownCell!=null&&midDownCell.IsWalkable)
                {
                    downStages++;
                    GridCell endDownCell = grid.GetGridCell(currentCell.Point.X, currentCell.Point.Y - 3);
                    if (endDownCell!=null&&endDownCell.IsWalkable)
                    {
                        downStages++;
                    }
                }
            }
            //left
            int leftStages = 0;
            GridCell startLeftCell = grid.GetGridCell(currentCell.Point.X - 1, currentCell.Point.Y);
            if (startLeftCell != null && startLeftCell.IsWalkable)
            {
                leftStages++;
                GridCell midLeftCell = grid.GetGridCell(currentCell.Point.X - 2, currentCell.Point.Y);
                if (midLeftCell!=null&&midLeftCell.IsWalkable)
                {
                    leftStages++;
                    GridCell endLeftCell = grid.GetGridCell(currentCell.Point.X - 3, currentCell.Point.Y);
                    if (endLeftCell!=null&&endLeftCell.IsWalkable)
                    {
                        leftStages++;
                    }
                }
            }
            //right
            int rightStages = 0;
            GridCell startRightCell = grid.GetGridCell(currentCell.Point.X + 1, currentCell.Point.Y);
            if (startRightCell!=null&&startRightCell.IsWalkable)
            {
                rightStages++;
                GridCell midRightCell = grid.GetGridCell(currentCell.Point.X + 2, currentCell.Point.Y);
                if (midRightCell!=null&&midRightCell.IsWalkable)
                {
                    rightStages++;
                    GridCell endRightCell = grid.GetGridCell(currentCell.Point.X + 3, currentCell.Point.Y);
                    if (endRightCell!=null&&endRightCell.IsWalkable)
                    {
                        rightStages++;
                    }
                }
            }
            await UniTask.Delay((int)(explosionDelay * 1000), cancellationToken: cancellationSource.Token);
            spriteRenderer.enabled = false; //off bomb sprite
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
            rb.simulated = true; //enable 1 hit damage, turned off in EnemyDamageReciever script
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


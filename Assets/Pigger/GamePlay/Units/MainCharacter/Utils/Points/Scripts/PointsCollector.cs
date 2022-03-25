using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace Pigger.GamePlay.Units.MainCharacter.Utils.Points
{
    [RequireComponent(typeof(Collider2D))]
    public class PointsCollector : MonoBehaviour
    {
        [Inject] private PlayerController player;

        private const int maxPoints = 3;

        public event Action pointCollectedEvent;
        public event Action fullPointsCollectedEvent;
        public event Action pointsLostEvent;

        public int CurrentPoints { get; private set; }
        private List<GameObject> currentPointsObjects;
        private bool canCollect;

        private void Awake()
        {
            currentPointsObjects = new List<GameObject>();
        }

        private void Start()
        {
            canCollect = true;
        }

        private void OnEnable()
        {
            player.playerGetDamageEvent += LostPoints;
        }
        private void OnDisable()
        {
            player.playerGetDamageEvent -= LostPoints;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (canCollect==true&&collision.gameObject.layer == 13) //points
            {
                CurrentPoints++;
                pointCollectedEvent?.Invoke();
                currentPointsObjects.Add(collision.gameObject);
                collision.gameObject.SetActive(false);
                if (CurrentPoints>=maxPoints)
                {
                    CurrentPoints = maxPoints;
                    fullPointsCollectedEvent?.Invoke();
                    canCollect=false;
                }
            }
        }

        public void ResetPoints()
        {
            CurrentPoints = 0;
            canCollect = true;
            currentPointsObjects.Clear();
        }

        private void LostPoints()
        {
            if (CurrentPoints!=0)
            {
                foreach (var item in currentPointsObjects)
                {
                    Vector3 originalPos = item.transform.position;
                    item.transform.position += new Vector3(0f, 5f, 0f);
                    item.gameObject.layer = 0; //default layer
                    item.SetActive(true);
                    item.transform.DOMove(originalPos, 2f).SetEase(Ease.OutBounce).OnComplete(()=>
                    item.gameObject.layer = 13);//points layer to prevent "catch-in-air" point
                }
                pointsLostEvent?.Invoke();
                ResetPoints();
            }           
        }
    }
}


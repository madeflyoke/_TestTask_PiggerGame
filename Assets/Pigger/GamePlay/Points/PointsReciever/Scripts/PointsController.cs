using Pigger.GamePlay.Units.MainCharacter;
using System;
using UnityEngine;
using Zenject;

namespace Pigger.GamePlay.Points
{
    [RequireComponent(typeof(Collider2D))]
    public class PointsController : MonoBehaviour
    {
        [Inject] private PlayerController player;

        public event Action pointsRecievedEvent;
        public event Action allPointsEvent;

        [SerializeField] private int pointsToWin;
        public int RecievedPoints { get; private set; }
        public int PointsToWin { get=>pointsToWin;}

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == player.PointsCollector.gameObject)
            {
                RecievePoints(player.PointsCollector.CurrentPoints);
            }
        }

        private void RecievePoints(int points)
        {
            RecievedPoints += points;
            pointsRecievedEvent?.Invoke();
            if (RecievedPoints>=pointsToWin)
            {
                allPointsEvent?.Invoke();
                enabled = false;
                return;
            }
            player.PointsCollector.ResetPoints();
        }
    }
}

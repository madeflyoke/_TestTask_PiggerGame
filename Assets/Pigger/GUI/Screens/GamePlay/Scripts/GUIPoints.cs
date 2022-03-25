using Pigger.GamePlay.Points;
using Pigger.GamePlay.Units.MainCharacter;
using Pigger.GamePlay.Units.MainCharacter.Utils.Points;
using Pigger.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pigger.GUI.Screens.GamePlay
{
    public class GUIPoints : MonoBehaviour
    {
        [Inject] private PlayerController player;
        [Inject] private PointsController pointsController;

        [SerializeField] private Image firstPoint;
        [SerializeField] private Image secondPoint;
        [SerializeField] private Image thirdPoint;
        private Queue<GameObject> applePoints;
        private PointsCollector pointsCollector;
        
        private void Awake()
        {
            pointsCollector = player.PointsCollector;
            ResetGUIPoints();
        }

        private void OnEnable()
        {
            pointsCollector.pointCollectedEvent += SetGUIPoint;
            pointsController.pointsRecievedEvent += ResetGUIPoints;
            pointsCollector.pointsLostEvent += ResetGUIPoints;
        }
        private void OnDisable()
        {
            pointsCollector.pointCollectedEvent -= SetGUIPoint;
            pointsController.pointsRecievedEvent -= ResetGUIPoints;
            pointsCollector.pointsLostEvent -= ResetGUIPoints;
        }

        private void SetGUIPoint()
        {
            if (applePoints.Peek() == null||applePoints.Count==0)
            {
                ResetGUIPoints();
            }
            GameObject applePoint = applePoints.Dequeue();
            applePoint.SetActive(true);
        }

        private void ResetGUIPoints()
        {
            applePoints = new Queue<GameObject>();
            applePoints.Enqueue(firstPoint.gameObject);
            applePoints.Enqueue(secondPoint.gameObject);
            applePoints.Enqueue(thirdPoint.gameObject);
            foreach (var applePoint in applePoints)
            {
                applePoint.SetActive(false);
            }
        }
    }
}


using UnityEngine;
using TMPro;
using Pigger.GamePlay.Points;
using Zenject;
using Pigger.Managers;

namespace Pigger.GUI.Screens.GamePlay
{
    public class ProgressBar : MonoBehaviour
    {
        [Inject] private PointsController pointsController;

        private const string progressText = "PROGRESS: ";

        [SerializeField] private TMP_Text pointsField;

        private void Start()
        {
            SetProgressPoints();
        }

        private void OnEnable()
        {
            pointsController.pointsRecievedEvent += SetProgressPoints;
        }
        private void OnDisable()
        {
            pointsController.pointsRecievedEvent -= SetProgressPoints;
        }

        private void SetProgressPoints()
        {
            pointsField.text = progressText + pointsController.RecievedPoints.ToString()
                + "/" + pointsController.PointsToWin.ToString(); 
        }
    }
}


using Pigger.GamePlay.Units.MainCharacter;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pigger.GUI.Screens.GamePlay
{
    public class HealthBar : MonoBehaviour
    {
        [Inject] private PlayerController player;

        private const float heartPointsGap = 55f;

        private enum Side
        {
            Left,
            Right
        }

        [SerializeField] private GameObject healthPointPrefab;
        [SerializeField] private Transform healthPointsSpawnPoint;
        [SerializeField] private Side pointsSide;
        private Stack<GameObject> healthPoints;
        private float targetWidth;

        private void Awake()
        {
            targetWidth = GetComponentInParent<CanvasScaler>().referenceResolution.x;
        }

        private void Start()
        {
            healthPoints = new Stack<GameObject>();
            for (int i = 0; i < player.MaxHealth; i++)
            {
                float widthGap = (Screen.width * heartPointsGap) / targetWidth;
                Vector3 spawnOffset = i * (pointsSide == Side.Left ? new Vector3(widthGap, 0f, 0f) : new Vector3(-widthGap, 0f, 0f));
                GameObject hp = Instantiate(healthPointPrefab, healthPointsSpawnPoint.position + spawnOffset,
                    Quaternion.identity, this.transform);
                healthPoints.Push(hp);
            }
        }

        private void OnEnable()
        {
            player.playerGetDamageEvent += HealthPointsControl;
        }
        private void OnDisable()
        {
            player.playerGetDamageEvent -= HealthPointsControl;
        }

        private void HealthPointsControl()
        {
            if (healthPoints.Count != 0)
            {
                healthPoints.Pop().SetActive(false);
            }
        }
    }
}


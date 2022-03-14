using Pigger.GamePlay.Units.MainCharacter;
using System.Collections.Generic;
using UnityEngine;

namespace Pigger.GUI.Screens.GamePlay
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject healthPointPrefab;
        [SerializeField] private Transform healthPointsSpawnPoint;
        private Stack<GameObject> healthPoints;
        private PlayerController player;

        private void Awake()
        {
            player = FindObjectOfType<PlayerController>();
            healthPoints = new Stack<GameObject>();
            for (int i = 0; i < player.MaxHealth; i++)
            {
                GameObject hp = Instantiate(healthPointPrefab, healthPointsSpawnPoint.position+(i*new Vector3(60f,0f,0f)),
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
            if (healthPoints.Count!=0)
            {
                healthPoints.Pop().SetActive(false);
            }         
        }

    }
}


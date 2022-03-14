using System;
using UnityEngine;

namespace Pigger.GamePlay.Units.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyAttacker : MonoBehaviour
    {
        public event Action<bool> targetInAttackRangeEvent;

        [SerializeField] private float attackRange;
        private Collider2D attackCollider;

        private void Awake()
        {
            attackCollider = GetComponent<Collider2D>();
            attackCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 9)
            {
                targetInAttackRangeEvent?.Invoke(true);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 9)
            {
                targetInAttackRangeEvent?.Invoke(false);
            }
        }
    }
}


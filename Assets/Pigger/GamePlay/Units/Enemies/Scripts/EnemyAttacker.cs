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
        public bool canAttack { get; set; }

        private void Awake()
        {
            attackCollider = GetComponent<Collider2D>();
            attackCollider.isTrigger = true;
            canAttack = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (canAttack && collision.gameObject.layer == 9) //player
            {
                targetInAttackRangeEvent?.Invoke(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (canAttack && collision.gameObject.layer == 9)
            {
                targetInAttackRangeEvent?.Invoke(false);
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pigger.GamePlay.Units.Enemies
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class EnemySearcher : MonoBehaviour
    {
        public event Action<bool> targetInAttackRangeEvent;

        [SerializeField] private float viewRange;
        [SerializeField] private float chaseViewRange;
        public float ViewRange { get => viewRange; }
        public float ChaseViewRangeScale { get => chaseViewRange;}
        public CircleCollider2D searchCollider { get; private set; }
        private void Awake()
        {
            searchCollider = GetComponent<CircleCollider2D>();
            searchCollider.isTrigger = true;
            searchCollider.radius = viewRange;
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
            if (collision.gameObject.layer ==9)
            {
                targetInAttackRangeEvent?.Invoke(false);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, viewRange*transform.parent.transform.localScale.x);
        }
    }
}


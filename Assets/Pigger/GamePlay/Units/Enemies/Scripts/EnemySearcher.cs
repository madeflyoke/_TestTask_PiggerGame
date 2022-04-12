using System;
using UnityEngine;

namespace Pigger.GamePlay.Units.Enemies
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class EnemySearcher : MonoBehaviour
    {
        public event Action<bool> targetInViewRangeEvent;

        [SerializeField] private float viewRange;
        [SerializeField] private float chaseViewRange;
        public float ViewRange { get => viewRange; }
        public float ChaseViewRange { get => chaseViewRange; }
        public CircleCollider2D SearchCollider { get; private set; }
        public bool canSearch { get; set; }

        private void Awake()
        {
            SearchCollider = GetComponent<CircleCollider2D>();
            SearchCollider.isTrigger = true;
            SearchCollider.radius = viewRange;
            canSearch = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (canSearch && collision.gameObject.layer == 9) //player
            {
                targetInViewRangeEvent?.Invoke(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (canSearch && collision.gameObject.layer == 9)
            {
                targetInViewRangeEvent?.Invoke(false);
            }
        }
    }
}

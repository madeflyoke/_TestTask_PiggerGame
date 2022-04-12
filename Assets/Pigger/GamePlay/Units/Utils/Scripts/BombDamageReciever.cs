using UnityEngine;

namespace Pigger.GamePlay.Units.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public class BombDamageReciever : MonoBehaviour
    {
        private Unit unit;

        private void Awake()
        {
            unit = GetComponentInParent<Unit>();
        }

        private void OnTriggerEnter2D(Collider2D bombCollider)
        {
            if (bombCollider.gameObject.layer==15) //bomb
            {
                unit.GetDamage();
                bombCollider.attachedRigidbody.simulated = false; //prevent multiple hits, turned back on in the Bomb script
            }
        }
    }
}


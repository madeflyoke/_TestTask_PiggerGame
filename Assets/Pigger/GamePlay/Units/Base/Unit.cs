using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pigger.GamePlay.Units
{
    public class Unit : MonoBehaviour
    {
        protected enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }
        [Header("Rendering")]
        [SerializeField] private Sprite upMovementSprite;
        [SerializeField] private Sprite downMovementSprite;
        [SerializeField] private Sprite leftMovementSprite;
        [SerializeField] private Sprite rightMovementSprite;
        [Space]
        [SerializeField] private Direction startLookDirection;
        [Header("Stats")]
        [SerializeField] protected float defaultSpeed;

        protected Dictionary<Direction, Sprite> directionSprites;
        protected Direction currentDirection;
        protected SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            directionSprites = new Dictionary<Direction, Sprite>();
            directionSprites.Add(Direction.Left, leftMovementSprite);
            directionSprites.Add(Direction.Right, rightMovementSprite);
            directionSprites.Add(Direction.Up, upMovementSprite);
            directionSprites.Add(Direction.Down, downMovementSprite);
            spriteRenderer.sprite = directionSprites[startLookDirection];
            currentDirection = startLookDirection;
        }

        protected virtual void SetDirectionSprite(Vector2 direction) { }
            
        public virtual void GetDamage()
        {
          
        }

        protected virtual void Die()
        {
            Debug.Log("DIED: " + gameObject.name);
        }
    }
}

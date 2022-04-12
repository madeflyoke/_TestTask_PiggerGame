using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Pigger.GamePlay.Units
{
    public abstract class Unit : MonoBehaviour
    {
        protected enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }
        [Header("Rendering")]
        [SerializeField] protected Color damageColor;
        [SerializeField] private Sprite upMovementSprite;
        [SerializeField] private Sprite downMovementSprite;
        [SerializeField] private Sprite leftMovementSprite;
        [SerializeField] private Sprite rightMovementSprite;
        [Space]
        [SerializeField] private Direction startLookDirection;
        [Header("Stats")]
        [SerializeField] protected float defaultSpeed;
        [Tooltip("Custom (approximate) range")]
        [Range(1f, 5f)]
        [SerializeField] private int maxHealthPoints;

        public int MaxHealth { get => maxHealthPoints; }
        protected Dictionary<Direction, Sprite> defaultDirectionSprites;
        protected Direction currentDirection;
        protected SpriteRenderer spriteRenderer;
        protected int currentHealthPoints;
        protected float currentSpeed;
        protected Color defaultColor;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            defaultDirectionSprites = new Dictionary<Direction, Sprite>();
            defaultDirectionSprites.Add(Direction.Left, leftMovementSprite);
            defaultDirectionSprites.Add(Direction.Right, rightMovementSprite);
            defaultDirectionSprites.Add(Direction.Up, upMovementSprite);
            defaultDirectionSprites.Add(Direction.Down, downMovementSprite);
            spriteRenderer.sprite = defaultDirectionSprites[startLookDirection];

            currentDirection = startLookDirection;
            currentHealthPoints = maxHealthPoints;
            currentSpeed = defaultSpeed;
            defaultColor = spriteRenderer.color;
        }

        protected virtual void SetDirection(Vector2 direction) { }
         
        public virtual void GetDamage()
        {
            currentHealthPoints -= 1;
            if (currentHealthPoints <= 0)
            {
                Die();
                return;
            }
            spriteRenderer.DOColor(damageColor, 0.3f).OnComplete(() => spriteRenderer.DOColor(defaultColor, 0.3f));
        }

        protected virtual void Die()
        {
            Debug.Log("DIED: " + gameObject.name);
        }
    }
}

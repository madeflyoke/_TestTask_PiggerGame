using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pigger.GamePlay.Units.MainCharacter
{
    public class PlayerController : Unit
    {

        [SerializeField] private float movementSpeed;
        private Vector2 moveDirection;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            if (y!=0)
            {
                x = 0;
            }
            moveDirection = new Vector2(x, y);
            if (x!=0||y!=0)
            {
                SetDirectionSprite(moveDirection);
            }
        }

        protected override void SetDirectionSprite(Vector2 direction)
        {
            Direction prevDirection = currentDirection;

            if (direction.x > 0)
            {
                currentDirection = Direction.Right;
            }
            else if (direction.x < 0)
            {
                currentDirection = Direction.Left;
            }

            if (direction.y > 0f)
            {
                currentDirection = Direction.Up;
            }
            else if (direction.y < 0f)
            {
                currentDirection = Direction.Down;
            }

            if (prevDirection == currentDirection)
            {
                return;
            }
            spriteRenderer.sprite = directionSprites[currentDirection];
        }

        private void FixedUpdate()
        {
            transform.position += ((Vector3)moveDirection * movementSpeed * Time.fixedDeltaTime);
        }

    }
}


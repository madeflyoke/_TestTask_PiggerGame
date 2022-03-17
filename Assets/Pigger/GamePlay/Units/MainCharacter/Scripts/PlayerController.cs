using UnityEngine;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using Pigger.GamePlay.Units.MainCharacter.Utils;

namespace Pigger.GamePlay.Units.MainCharacter
{
    public class PlayerController : Unit
    {
        public event Action playerDiedEvent;
        public event Action playerGetDamageEvent;
     
        [SerializeField] private ParticleSystem deathEffect;
        [Header("Bomb")]
        [SerializeField] private Bomb bomb;

        private Vector2 moveDirection;
        private CancellationTokenSource cancellationSource;

        protected override void Awake()
        {
            base.Awake();
            cancellationSource = new CancellationTokenSource();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                bomb.SetExplosion();
            }

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            if (y != 0)
            {
                x = 0;
            }
            moveDirection = new Vector2(x, y);
            if (x != 0 || y != 0)
            {
                SetDirection(moveDirection);
            }
        }

        protected override void SetDirection(Vector2 direction)
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
            spriteRenderer.sprite = defaultDirectionSprites[currentDirection];
        }

        private void FixedUpdate()
        {
            transform.position += ((Vector3)moveDirection * defaultSpeed * Time.fixedDeltaTime);
        }

        public override void GetDamage()
        {
            playerGetDamageEvent?.Invoke();
            base.GetDamage();          
        }

        protected async override void Die()
        {
            base.Die();
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            await UniTask.Delay(4000, cancellationToken: cancellationSource.Token);
            playerDiedEvent?.Invoke();
        }
    }
}


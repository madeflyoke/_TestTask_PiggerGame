using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using Pigger.GamePlay.Units.MainCharacter.Utils;
using Pigger.GamePlay.Units.MainCharacter.Utils.Points;

namespace Pigger.GamePlay.Units.MainCharacter
{
    public class PlayerController : Unit
    {
        public event Action playerDiedEvent;
        public event Action playerGetDamageEvent;

        [SerializeField] private float damagedSpeedMultiplier;
        [SerializeField] private float damagedSpeedDuration;
        [Header("Carrying points speed")]
        [SerializeField] private float easySlowSpeedMultiplier;
        [SerializeField] private float middleSlowSpeedMultiplier;
        [SerializeField] private float heavySlowSpeedMultiplier;
        [Header("Utils")]
        [SerializeField] private Bomb bomb;
        [SerializeField] private float bombCooldown;
        [SerializeField] private PointsCollector pointsCollector;
        [SerializeField] private ParticleSystem deathEffect;
        [SerializeField] private Joystick inputJoystick;
        [SerializeField] private float inputDeadZone;
        public PointsCollector PointsCollector { get => pointsCollector; }
        public float BombCooldown { get => bombCooldown; }

        private Vector2 moveDirection;
        private CancellationTokenSource cancellationSource;
        private bool canMove;

        protected override void Awake()
        {
            base.Awake();
            cancellationSource = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            pointsCollector.pointCollectedEvent += FullPointsSpeedControl;
            inputJoystick.joystickIsActiveEvent += InputHandler;
        }
        private void OnDisable()
        {
            pointsCollector.pointCollectedEvent -= FullPointsSpeedControl;
            inputJoystick.joystickIsActiveEvent -= InputHandler;
        }

        private void Update()
        {
            if (canMove)
            {
                SetMoveDirection();
            }           
        }

        private void FixedUpdate()
        {
            if (canMove)
            {
                transform.position += ((Vector3)moveDirection * currentSpeed * Time.fixedDeltaTime);
            }
        }
        public void SetBomb()
        {
            bomb.SetExplosion();
        }
        public override void GetDamage()
        {
            playerGetDamageEvent?.Invoke();
            base.GetDamage();
            DamagedSpeedControl();
        }

        protected override void SetDirection(Vector2 direction)
        {
            Direction prevDirection = currentDirection;

            if (direction.x > inputJoystick.DeadZone)
            {
                currentDirection = Direction.Right;
            }
            else if (direction.x < inputJoystick.DeadZone)
            {
                currentDirection = Direction.Left;
            }

            if (direction.y > inputJoystick.DeadZone)
            {
                currentDirection = Direction.Up;
            }
            else if (direction.y < inputJoystick.DeadZone)
            {
                currentDirection = Direction.Down;
            }

            if (prevDirection == currentDirection)
            {
                return;
            }
            spriteRenderer.sprite = defaultDirectionSprites[currentDirection];
        }

        protected async override void Die()
        {
            cancellationSource.Cancel();
            cancellationSource = new CancellationTokenSource();
            base.Die();
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            await UniTask.Delay(4000); //end game doesnt need cancelation token? 
            playerDiedEvent?.Invoke();
        }

        private async void DamagedSpeedControl()
        {
            cancellationSource.Cancel();
            cancellationSource = new CancellationTokenSource();
            currentSpeed = defaultSpeed * damagedSpeedMultiplier;
            await UniTask.Delay((int)(damagedSpeedDuration * 1000), cancellationToken: cancellationSource.Token);
            currentSpeed = defaultSpeed;
        }

        private async void FullPointsSpeedControl()
        {
            cancellationSource.Cancel();
            cancellationSource = new CancellationTokenSource();
            currentSpeed = defaultSpeed * (pointsCollector.CurrentPoints == 1 ? easySlowSpeedMultiplier
             : (pointsCollector.CurrentPoints == 2 ? middleSlowSpeedMultiplier : heavySlowSpeedMultiplier));
            await UniTask.WaitWhile((() => pointsCollector.CurrentPoints != 0), cancellationToken: cancellationSource.Token);
            currentSpeed = defaultSpeed;
        }

        private void InputHandler(bool canMove)
        {
            this.canMove = canMove;
        }

        private void SetMoveDirection()
        {
            float x = inputJoystick.Horizontal;
            float y = inputJoystick.Vertical;

            if (x <= inputDeadZone && x >= -inputDeadZone)
            {
                x = 0f;
            }
            if (y > inputDeadZone + 0.4f || y < -inputDeadZone - 0.4f) //Y-corrections 
            {
                y = y < 0 ? -1f : 1f;
                x = 0f;
            }
            else
            {
                y = 0;
            }
            moveDirection = new Vector2(x, y);
            if (x != 0f || y != 0f)
            {
                SetDirection(moveDirection);
            }
        }
    }
}


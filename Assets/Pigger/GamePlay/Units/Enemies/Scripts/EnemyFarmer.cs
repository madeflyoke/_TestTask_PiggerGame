using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Pigger.GamePlay.Units.Enemies
{
    public class EnemyFarmer : BaseEnemy
    {
        [Header("Stun")]
        [SerializeField] private Sprite upStunSprite;
        [SerializeField] private Sprite downStunSprite;
        [SerializeField] private Sprite leftStunSprite;
        [SerializeField] private Sprite rightStunSprite;
        [SerializeField] private float stunDuration;
        private Dictionary<Direction, Sprite> stunDirectionSprites;

        protected override void Awake()
        {
            base.Awake();
            stunDirectionSprites = new Dictionary<Direction, Sprite>();
            stunDirectionSprites.Add(Direction.Left, leftStunSprite);
            stunDirectionSprites.Add(Direction.Right, rightStunSprite);
            stunDirectionSprites.Add(Direction.Up, upStunSprite);
            stunDirectionSprites.Add(Direction.Down, downStunSprite);           
        }

        protected override void SetState(EnemyState state)
        {          
            if (state == EnemyState.Stunned)
            {
                currentState = EnemyState.Stunned;
                Stun();
                return;
            }
            base.SetState(state);
        }

        protected override void SetSprite(Direction direction, EnemyState state)
        {
            if (state == EnemyState.Stunned)
            {
                spriteRenderer.sprite = stunDirectionSprites[currentDirection];
                return;
            }
            base.SetSprite(direction, state);
        }

        public override void GetDamage()
        {
            if (currentState == EnemyState.Stunned)
            {
                return;
            }
            base.GetDamage();
        }

        protected override void Die()
        {
            base.Die();
            SetState(EnemyState.Stunned);
        }

        private async void Stun()
        {
            cancellationSource.Cancel();
            cancellationSource = new CancellationTokenSource();
            attacker.canAttack = false;
            searcher.canSearch = false;
            SetSprite(currentDirection, currentState);
            await UniTask.Delay((int)stunDuration * 1000, cancellationToken: cancellationSource.Token);
            attacker.canAttack = true;
            searcher.canSearch = true;
            currentHealthPoints = MaxHealth;
            SetState(EnemyState.Patrol);
        }
    }
}


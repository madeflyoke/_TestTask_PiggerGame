using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Pigger.GamePlay.Units.MainCharacter;
using Pigger.Utils.PathFind;
using System.Collections.Generic;

namespace Pigger.GamePlay.Units.Enemies
{
    public abstract class BaseEnemy : Unit
    {
        public enum EnemyState
        {
            None,
            Patrol,
            Chasing,
            Attack
        }
        [SerializeField] private float chaseSpeedScale;
        [SerializeField] private int idleStandingTime;
        [Space]
        [SerializeField] private AStarPathFinder pathFinder;
        [Header("Attack")]
        [SerializeField] private float attackRate;
        //[SerializeField] private float attackDamage; //while 1-hit game it doesnt need

        private EnemySearcher searcher;
        private EnemyAttacker attacker;
        private PlayerController player;
        private EnemyState currentState;
        private List<Vector2> path;
        private Vector2 prevPosition;
        private float prevAttackTime;
        private CancellationTokenSource cancellationSource;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            searcher = GetComponentInChildren<EnemySearcher>();
            attacker = GetComponentInChildren<EnemyAttacker>();
            prevPosition = transform.position;
            cancellationSource = new CancellationTokenSource();
            player = FindObjectOfType<PlayerController>();
        }

        private void Start()
        {
            SetState(EnemyState.Patrol);
        }

        private void FixedUpdate()
        {
            if (prevPosition != (Vector2)transform.position)
            {
                SetDirectionSprite(prevPosition);
                prevPosition = transform.position;
            }
        }

        private void OnEnable()
        {
            searcher.targetInViewRangeEvent += ChaseStateHandler;
            attacker.targetInAttackRangeEvent += AttackStateHandler;
        }
        private void OnDisable()
        {
            searcher.targetInViewRangeEvent -= ChaseStateHandler;
            attacker.targetInAttackRangeEvent -= AttackStateHandler;
        }

        protected override void SetDirectionSprite(Vector2 affectPosition)
        {
            if (Mathf.Abs(transform.position.x - affectPosition.x) >
                Mathf.Abs(transform.position.y - affectPosition.y))
            {
                if (transform.position.x > affectPosition.x)
                {
                    currentDirection = Direction.Right;
                }
                else if (transform.position.x < affectPosition.x)
                {
                    currentDirection = Direction.Left;
                }
            }
            else
            {
                if (transform.position.y > affectPosition.y)
                {
                    currentDirection = Direction.Up;
                }
                else if (transform.position.y < affectPosition.y)
                {
                    currentDirection = Direction.Down;
                }
            }
            spriteRenderer.sprite = directionSprites[currentDirection];
        }

        private async void Patrol()
        {
            searcher.SearchCollider.radius = searcher.ViewRange;
            cancellationSource.Cancel();
            cancellationSource = new CancellationTokenSource();
            while (currentState == EnemyState.Patrol)
            {
                path = pathFinder.GetRandomWorldPath(transform.position);
                while (CheckPath(path) == false)
                {
                    Debug.LogWarning("Searching for path...");
                    await UniTask.Delay(500, cancellationToken: cancellationSource.Token);
                    path = pathFinder.GetRandomWorldPath(transform.position);
                }
                while (path.Count > 0)
                {
                    while (Vector2.Distance(transform.position, path[0]) > 0.05f)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, path[0],
                            Time.deltaTime * defaultSpeed);
                        await UniTask.Yield(cancellationSource.Token);
                    }
                    path.RemoveAt(0);
                    await UniTask.Yield(cancellationSource.Token);
                }
                path = null;
                await UniTask.Delay(idleStandingTime * 1000, cancellationToken: cancellationSource.Token);
            }
        }

        private async void Chase()
        {
            cancellationSource.Cancel();
            cancellationSource = new CancellationTokenSource();
            path = pathFinder.GetWorldPath(transform.position, player.transform.position);
            while (CheckPath(path) == false)
            {
                Debug.LogWarning("Searching for path...");
                await UniTask.Delay(500, cancellationToken: cancellationSource.Token);
                path = pathFinder.GetWorldPath(transform.position, player.transform.position);
            }
            searcher.SearchCollider.radius = searcher.ChaseViewRange;
            float speed = defaultSpeed * chaseSpeedScale;
            while (currentState == EnemyState.Chasing)
            {
                if (Vector2.Distance(transform.position, path[0]) > 0.1f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, path[0],
                           Time.deltaTime * speed);
                }
                else   //repeat find path after reached the node
                {
                    path = pathFinder.GetWorldPath(transform.position, player.transform.position);
                    while (CheckPath(path) == false)
                    {
                        Debug.LogWarning("Searching for path...");
                        await UniTask.Delay(500, cancellationToken: cancellationSource.Token);
                        path = pathFinder.GetWorldPath(transform.position, player.transform.position);
                    }
                }
                await UniTask.Yield(cancellationToken: cancellationSource.Token);
            }
            while (path.Count > 0)  //when player TriggerExit it should complete the path
            {
                while (Vector2.Distance(transform.position, path[0]) > 0.05f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, path[0],
                        Time.deltaTime * speed);
                    await UniTask.Yield(cancellationSource.Token);
                }
                path.RemoveAt(0);
                await UniTask.Yield(cancellationSource.Token);
            }
            await UniTask.Delay(idleStandingTime * 1000, cancellationToken: cancellationSource.Token);
            path = null;
            SetState(EnemyState.Patrol);
        }

        private bool CheckPath(List<Vector2> incomePath)
        {
            if (incomePath == null)
            {
                Debug.LogWarning("PATH INVALID");
                return false;
            }
            incomePath.RemoveAt(0); //if start from 2nd position to not return 
            return true;
        }

        private async void Attack()
        {
            cancellationSource.Cancel();
            cancellationSource = new CancellationTokenSource();
            while (currentState == EnemyState.Attack)
            {
                if (Time.unscaledTime - (1 / attackRate) > prevAttackTime)
                {
                    player.GetDamage(/*attackDamage*/);
                    prevAttackTime = Time.unscaledTime;
                }
                await UniTask.Yield(cancellationSource.Token);
            }
        }

        private void SetState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Patrol:
                    currentState = EnemyState.Patrol;
                    Patrol();
                    break;
                case EnemyState.Chasing:
                    currentState = EnemyState.Chasing;
                    Chase();
                    break;
                case EnemyState.Attack:
                    currentState = EnemyState.Attack;
                    Attack();
                    break;
                case EnemyState.None:
                    currentState = EnemyState.None;
                    break;
            }
        }

        private void ChaseStateHandler(bool inViewRange)
        {
            if (inViewRange == true)
            {
                SetState(EnemyState.Chasing);
            }
            else
            {
                if (player.gameObject.activeInHierarchy == false) //check if player is dead when TriggerExit                       
                {
                    SetState(EnemyState.Patrol);
                    return;
                }
                SetState(EnemyState.None);
            }
        }

        private void AttackStateHandler(bool inViewRange)  
        {
            if (inViewRange == true)
            {
                SetState(EnemyState.Attack);
            }
            else
            {
                if (player.gameObject.activeInHierarchy == false) //same as ChaseStateHandler
                {
                    SetState(EnemyState.Patrol);
                    return;
                }
                SetState(EnemyState.Chasing);
            }
        }     
    }
}


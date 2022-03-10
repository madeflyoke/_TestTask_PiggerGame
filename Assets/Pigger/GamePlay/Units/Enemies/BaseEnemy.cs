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
        private enum EnemyState
        {
            None,
            Patrol,
            Chasing,
            Attack
        }
        [SerializeField] private AStarPathFinder pathFinder;
        [Header("Stats")]
        [SerializeField] private float defaultSpeed;
        [SerializeField] private float chaseSpeedScale;
        [SerializeField] private int idleStandingTime;
        [Header("Attack")]
        [SerializeField] private float attackRate;
        [SerializeField] private Transform attackPivot;
        [SerializeField] private float attackRange;
        private EnemyState currentState;
        private CancellationTokenSource cancellationSource;
        private PlayerController player;
        private EnemySearcher searcher;
        private Vector2 prevPosition;
        private List<Vector2> path;
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            prevPosition = transform.position;
            searcher = GetComponentInChildren<EnemySearcher>();
            cancellationSource = new CancellationTokenSource();
            player = FindObjectOfType<PlayerController>();
        }

        private void Start()
        {
            SetState(EnemyState.Patrol);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                ChaseStateHandler(true);
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                ChaseStateHandler(false);
            }
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
            searcher.targetInAttackRangeEvent += ChaseStateHandler;
        }
        private void OnDisable()
        {
            searcher.targetInAttackRangeEvent -= ChaseStateHandler;
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

        protected async void Patrol()
        {
            searcher.searchCollider.radius = searcher.ViewRange;
            cancellationSource.Cancel();
            cancellationSource = new CancellationTokenSource();
            while (currentState == EnemyState.Patrol)
            {
                path = pathFinder.GetRandomWorldPath(transform.position);
                if (CheckPath(path)==false)
                {
                    while (CheckPath(path)==false)
                    {
                        Debug.LogWarning("Searching for path...");
                        await UniTask.Delay(500, cancellationToken: cancellationSource.Token);
                        path = pathFinder.GetRandomWorldPath(transform.position);
                    }
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
            if (CheckPath(path)==false)
            {
                while (CheckPath(path) == false)
                {
                    Debug.LogWarning("Searching for path...");
                    await UniTask.Delay(500, cancellationToken: cancellationSource.Token);
                    path = pathFinder.GetWorldPath(transform.position, player.transform.position);
                }
            }
            searcher.searchCollider.radius = searcher.ChaseViewRangeScale;
            float speed = defaultSpeed * chaseSpeedScale;
            while (currentState == EnemyState.Chasing)
            {
                if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
                {
                    SetState(EnemyState.Attack);
                    return;
                }
                if (Vector2.Distance(transform.position, path[0]) > 0.1f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, path[0],
                           Time.deltaTime * speed);
                }
                else   //repeat find path after reached the node
                {
                    Debug.Log("GET NEXT PATH");
                    path = pathFinder.GetWorldPath(transform.position, player.transform.position);
                    if (CheckPath(path) == false)
                    {
                        while (CheckPath(path) == false)
                        {
                            Debug.LogWarning("Searching for path...");
                            await UniTask.Delay(500, cancellationToken: cancellationSource.Token);
                            path = pathFinder.GetRandomWorldPath(transform.position);
                        }
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
            if (incomePath.Count<=1)
            {
                Debug.LogWarning("SELF-POS PATH");
                path = null;
                return false;
            }
            incomePath.RemoveAt(0);
            return true;
        }

        private async void Attack()
        {
            cancellationSource.Cancel();
            cancellationSource = new CancellationTokenSource();
            float prevAttackTime = 0;
            while (currentState == EnemyState.Attack &&
                Vector2.Distance(transform.position, player.transform.position) <= attackRange)
            {
                if (Time.unscaledTime - (1 / attackRate) > prevAttackTime)
                {
                    Debug.Log("ATTACK");
                    prevAttackTime = Time.unscaledTime;
                }
                await UniTask.Yield(cancellationSource.Token);
            }
            SetState(EnemyState.Chasing);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPivot.position, attackRange);
        }

        private void ChaseStateHandler(bool inAttackRange)
        {
            if (inAttackRange == true)
            {
                SetState(EnemyState.Chasing);
            }
            else
            {
                SetState(EnemyState.None);
            }
        }

    }
}


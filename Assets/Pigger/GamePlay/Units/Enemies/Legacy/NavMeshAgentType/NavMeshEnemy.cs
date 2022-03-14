//using UnityEngine;
//using UnityEngine.AI;
//using Cysharp.Threading.Tasks;
//using System.Threading;
//using Pigger.GamePlay.Units.MainCharacter;
//using System.Collections.Generic;

//namespace Pigger.GamePlay.Units.Enemies
//{
//    public abstract class NavMeshEnemy : Unit
//    {
//        private enum EnemyState
//        {
//            Patrol,
//            Chasing,
//            Attack
//        }
//        [SerializeField] private float defaultSpeedScale;
//        [SerializeField] private float chaseSpeedScale;
//        [SerializeField] private int idleStandingTime;
//        [SerializeField] private Vector2 patrolBasePosition;
//        [SerializeField] private float patrolRange;
//        [SerializeField] private float attackRate;
//        [SerializeField] private Transform attackPivot;
//        [SerializeField] private float attackRange;
//        private NavMeshAgent agent;
//        private EnemyState currentState;
//        private CancellationTokenSource cancellationSource;
//        private Vector2 gizmosRandomPoint;
//        private PlayerController player;
//        //private EnemySearcher searcher;
//        private float defaultSpeed;

//        protected override void Awake()
//        {
//            base.Awake();
//            Initialize();
//        }

//        private void Initialize()
//        {
//            searcher = GetComponentInChildren<EnemySearcher>();
//            cancellationSource = new CancellationTokenSource();
//            agent = GetComponent<NavMeshAgent>();
//            player = FindObjectOfType<PlayerController>();
//            agent.updateUpAxis = false;
//            agent.updateRotation = false;
//            agent.speed *= defaultSpeedScale;
//            defaultSpeed = agent.speed;
//        }

//        private void Start()
//        {
//            SetState(EnemyState.Patrol);
//        }
//        private void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.Z))
//            {
//                cancellationSource.Cancel();
//            }
//        }

//        //private void OnEnable()
//        //{
//        //    searcher.targetFoundEvent += ChaseStateHandler;
//        //}
//        //private void OnDisable()
//        //{
//        //    searcher.targetFoundEvent -= ChaseStateHandler;
//        //}

//        protected override void SetDirectionSprite(Vector2 affectPosition)
//        {
//            if (transform.position.x > affectPosition.x)
//            {
//                currentDirection = Direction.Right;
//            }
//            else if (transform.position.x < affectPosition.x)
//            {
//                currentDirection = Direction.Left;
//            }

//            if (transform.position.y > affectPosition.y + 0.1f)
//            {
//                currentDirection = Direction.Up;
//            }
//            else if (transform.position.y < affectPosition.y - 0.1f) //updown sensitivity
//            {
//                currentDirection = Direction.Down;
//            }

//            spriteRenderer.sprite = directionSprites[currentDirection];
//        }

//        protected async void Patrol()
//        {
//            agent.speed = defaultSpeed;
//            while (currentState == EnemyState.Patrol)
//            {
//                if (GetRandomPoint(out NavMeshPath path) == true)
//                {
//                    agent.SetPath(path);
//                    Vector2 prevPosition = transform.position;
//                    while (agent.hasPath)
//                    {
//                        if (Vector2.Distance(prevPosition, transform.position) >= 0.2f) //check-rate
//                        {
//                            SetDirectionSprite(prevPosition);
//                            prevPosition = transform.position;
//                        }
//                        await UniTask.Yield(cancellationSource.Token);
//                    }
//                    await UniTask.Delay(idleStandingTime * 1000, cancellationToken: cancellationSource.Token);
//                }
//                await UniTask.Yield(cancellationSource.Token);
//            }
//        }

//        private async void Chase()
//        {
//            agent.speed = defaultSpeed;
//            agent.speed *= chaseSpeedScale;
//            searcher.searchCollider.radius = searcher.ChaseViewRangeScale;
//            Vector2 prevPosition = transform.position;
//            while (searcher.searchCollider.IsTouchingLayers(1 << 9))//player
//            {
//                NavMeshPath path = new NavMeshPath();
//                if (NavMesh.SamplePosition(player.transform.position,
//                    out NavMeshHit hit, 3f, NavMesh.AllAreas))
//                {
//                    if (agent.CalculatePath(hit.position, path))
//                    {
//                        agent.SetPath(path);
//                    }
//                }
//                if (Vector2.Distance(prevPosition, transform.position) >= 0.05f) //check-rate
//                {
//                    SetDirectionSprite(prevPosition);
//                    prevPosition = transform.position;
//                }
//                if (Vector2.Distance(player.transform.position, transform.position) <= attackRange)
//                {
//                    SetState(EnemyState.Attack);
//                }
//                await UniTask.Delay(250, cancellationToken: cancellationSource.Token);
//            }
//            while (agent.hasPath)
//            {
//                await UniTask.Yield();
//            }
//            await UniTask.Delay(idleStandingTime * 1000, cancellationToken: cancellationSource.Token);
//            searcher.searchCollider.radius = searcher.ViewRange;
//            SetState(EnemyState.Patrol);
//        }


//        private async void Attack()
//        {
//            agent.ResetPath();
//            while (Vector2.Distance(player.transform.position, transform.position) <= attackRange)
//            {
//                Debug.Log("SMASH!");
//                if (Vector2.Distance(player.transform.position, transform.position) >= 0.5f) //check-rate
//                {
//                    //Debug.Log("PLAYER: "+player.transform.position +"MY: "+ transform.position);
//                    SetDirectionSprite(transform.position+player.transform.position);
//                }
//                await UniTask.Delay((int)((1 / attackRate) * 1000), cancellationToken: cancellationSource.Token);
//            }
//            Debug.Log("OUT_OF_ATTACk");
//            SetState(EnemyState.Chasing);
//        }

//        private void SetState(EnemyState state)
//        {
//            cancellationSource.Cancel();
//            cancellationSource = new CancellationTokenSource();
//            switch (state)
//            {
//                case EnemyState.Patrol:
//                    currentState = EnemyState.Patrol;
//                    Patrol();
//                    break;
//                case EnemyState.Chasing:
//                    currentState = EnemyState.Chasing;
//                    Chase();
//                    break;
//                case EnemyState.Attack:
//                    currentState = EnemyState.Attack;
//                    Attack();
//                    break;
//            }
//        }

//        private bool GetRandomPoint(out NavMeshPath navPath)
//        {
//            if (NavMesh.SamplePosition(patrolBasePosition + Random.insideUnitCircle * patrolRange,
//                out NavMeshHit hit, 1f, NavMesh.AllAreas))
//            {
//                if (Vector2.Distance(transform.position, hit.position) <= 2f)
//                {
//                    navPath = null;
//                    return false;
//                }
//                gizmosRandomPoint = hit.position;
//                navPath = new NavMeshPath();
//                agent.CalculatePath(hit.position, navPath);
//                if (navPath.status == NavMeshPathStatus.PathComplete)
//                {
//                    return true;
//                }
//                return false;
//            }
//            else
//            {
//                navPath = null;
//                return false;
//            }
//        }

//        private void OnDrawGizmos()
//        {
//            Gizmos.color = Color.green;
//            Gizmos.DrawWireSphere(patrolBasePosition, patrolRange);
//            Gizmos.color = Color.red;
//            Gizmos.DrawWireSphere(attackPivot.position, attackRange);
//            if (gizmosRandomPoint != Vector2.zero)
//            {
//                Gizmos.color = Color.red;
//                Gizmos.DrawSphere(gizmosRandomPoint, 0.1f);
//            }
//        }

//        private void ChaseStateHandler()
//        {
//            SetState(EnemyState.Chasing);
//        }
//    }
//}


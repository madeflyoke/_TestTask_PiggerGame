using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Pigger.GamePlay.Units.MainCharacter;
using Zenject;
using Pigger.GamePlay.Units.MainCharacter.Utils.Points;

namespace Pigger.GamePlay.Points
{
    public class ArrowPointer : MonoBehaviour
    {
        [Inject] private PlayerController player;
        [Inject] private PointsController controller;

        [SerializeField] private GameObject pointer;
        [SerializeField] private Transform target;
        private CancellationTokenSource cancellationSource;
        private PointsCollector collector;
        
        private Vector2 defaultPointerPosition;
        private Quaternion defaultPointerRotation;
            
        private void Awake()
        {
            cancellationSource = new CancellationTokenSource();
            collector = player.PointsCollector;
            defaultPointerPosition = transform.position;
            defaultPointerRotation = transform.rotation;
            OffPointer();
        }

        private void OnEnable()
        {
            collector.fullPointsCollectedEvent += SetPointer;
            controller.pointsRecievedEvent += OffPointer;
        }
        private void OnDisable()
        {
            collector.fullPointsCollectedEvent -= SetPointer;
            controller.pointsRecievedEvent -= OffPointer;
        }

        private void OffPointer()
        {
            cancellationSource.Cancel();
            cancellationSource = new CancellationTokenSource();
            pointer.SetActive(false);
        }

        public async void SetPointer()
        {
            pointer.SetActive(true);
            while (true)
            {
                if (player.gameObject.activeInHierarchy==false)
                {
                    return;
                }
                Vector2 targetViewPos = Camera.main.WorldToViewportPoint(target.position);        
                if (targetViewPos.x >= 0 && targetViewPos.x <= 1 && targetViewPos.y >= 0 && targetViewPos.y <= 1)
                {                 
                    transform.SetPositionAndRotation(defaultPointerPosition, defaultPointerRotation);
                }
                else
                {
                    Vector2 viewPortPos = targetViewPos;
                    if (targetViewPos.x<0f)
                    {
                        viewPortPos.x = 0.05f;
                    }
                    if (targetViewPos.x>1f)
                    {
                        viewPortPos.x = 0.95f;
                    }
                    if (targetViewPos.y<0f)
                    {
                        viewPortPos.y = 0.05f;
                    }
                    if (targetViewPos.y>1f)
                    {
                        viewPortPos.y = 0.95f;
                    }
                    Vector2 newPos = Camera.main.ViewportToWorldPoint(viewPortPos);
                    transform.position = newPos;
                    transform.up = -(target.position - transform.position);
                }
                await UniTask.Delay(250, cancellationToken: cancellationSource.Token);
            }
        }
    }      
}


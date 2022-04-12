using Pigger.GamePlay.Points;
using UnityEngine;
using Zenject;

public class PointsControllerInstaller : MonoInstaller
{
    [SerializeField] private PointsController pointsController;
    public override void InstallBindings()
    {
        Container.BindInstance(pointsController).AsSingle().NonLazy();
    }
}
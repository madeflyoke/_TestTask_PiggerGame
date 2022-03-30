using Pigger.Utils.Grid;
using Pigger.Utils.PathFind;
using UnityEngine;
using Zenject;

public class GridPathInstaller : MonoInstaller
{
    [SerializeField] private GridMaker grid;
    [SerializeField] private AStarPathFinder pathFinder;
    public override void InstallBindings()
    {
        Container.BindInstance(grid).AsSingle().NonLazy();
        Container.BindInstance(pathFinder).AsSingle().NonLazy();
           
        Container.QueueForInject(grid);
        Container.QueueForInject(pathFinder);
    }
}
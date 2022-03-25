using Pigger.Utils.Grid;
using UnityEngine;
using Zenject;

public class GridMakerInstaller : MonoInstaller
{
    [SerializeField] private GridMaker grid;
    public override void InstallBindings()
    {
        Container.Bind<GridMaker>().FromInstance(grid).AsSingle().NonLazy();
    }
}
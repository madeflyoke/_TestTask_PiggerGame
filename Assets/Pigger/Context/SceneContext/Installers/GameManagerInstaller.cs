using Pigger.Managers;
using UnityEngine;
using Zenject;

public class GameManagerInstaller : MonoInstaller
{
    [SerializeField] private GameManager gameManager;
    public override void InstallBindings()
    {
        Container.BindInstance(gameManager).AsSingle().NonLazy();
    }
}
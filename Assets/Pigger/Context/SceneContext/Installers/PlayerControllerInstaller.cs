using Pigger.GamePlay.Units.MainCharacter;
using UnityEngine;
using Zenject;

public class PlayerControllerInstaller : MonoInstaller
{
    [SerializeField] private PlayerController playerController;
    public override void InstallBindings()
    {
        Container.BindInstance(playerController).AsSingle().NonLazy();
    }
}
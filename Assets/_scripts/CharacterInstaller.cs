using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ZenjectBasedController.Character.Settings;
using ZenjectBasedController.Handler;
using ZenjectBasedController.State;

namespace ZenjectBasedController.Character.Installer
{
    public class CharacterInstaller : MonoInstaller
    {
        [SerializeField]
        ModelSettings _modelSettings = null;
        public override void InstallBindings()
        {
            Container.Bind<CharacterModel>().AsSingle()
                .WithArguments(_modelSettings.Rigidbody);

            Container.BindInterfacesAndSelfTo<CharacterInputHandler>().AsSingle();
            Container.Bind<RayCastHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterMoveHandler>().AsSingle();
            // Container.BindInterfacesAndSelfTo<PlayerDamageHandler>().AsSingle();
            // Container.BindInterfacesTo<PlayerDirectionHandler>().AsSingle();
            // Container.BindInterfacesTo<PlayerShootHandler>().AsSingle();

            Container.Bind<CharacterInputState>().AsSingle();

            // Container.BindInterfacesTo<PlayerHealthWatcher>().AsSingle();
        }
    }
}
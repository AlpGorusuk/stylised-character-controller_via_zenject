using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ZenjectBasedController.Character.Settings;
using ZenjectBasedController.Signals;
using ZenjectBasedController.Handler;

namespace ZenjectBasedController.Character.Installer
{
    public class CharacterInstaller : MonoInstaller
    {
        [SerializeField]
        ModelSettings _modelSettings = null;
        public override void InstallBindings()
        {
            Container.Bind<CharacterModel>().AsSingle()
                .WithArguments(_modelSettings.Rigidbody, _modelSettings.Transform);

            Container.BindInterfacesAndSelfTo<CharacterInputHandler>().AsSingle();
            Container.Bind<RayCastHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterMoveHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterHeightHandler>().AsSingle();
            Container.Bind<CharacterHeightHandler.HeightSettings>().AsSingle();
            Container.Bind<CharacterMoveHandler.CharacterJumpSettings>().AsSingle();
            // Container.BindInterfacesTo<PlayerDirectionHandler>().AsSingle();
            // Container.BindInterfacesTo<PlayerShootHandler>().AsSingle();

            // Container.BindInterfacesTo<PlayerHealthWatcher>().AsSingle();
            InstallSignals();
        }

        void InstallSignals()
        {
            // Every scene that uses signals needs to install the built-in installer SignalBusInstaller
            // Or alternatively it can be installed at the project context level (see docs for details)
            SignalBusInstaller.Install(Container);

            // Signals can be useful for game-wide events that could have many interested parties
            Container.DeclareSignal<JumpSignal>();
            Container.BindSignal<JumpSignal>()
            .ToMethod(() => Debug.Log("Jump!"));

            Container.DeclareSignal<MoveSignal>();
            Container.BindSignal<MoveSignal>()
            .ToMethod(() => Debug.Log("Move!"));
        }
    }
}
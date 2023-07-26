using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ZenjectBasedController.Handler;

namespace ZenjectBasedController.Settings
{
    [CreateAssetMenu(menuName = "StylisedCharacter/Movement Settings")]
    public class CharacterMovementSettingsSO : ScriptableObjectInstaller<CharacterMovementSettingsSO>
    {
        public CharacterMoveHandler.CharacterMoveSettings characterMovementSettings;

        public override void InstallBindings()
        {
            Container.BindInstance(characterMovementSettings).IfNotBound();
        }
    }
}

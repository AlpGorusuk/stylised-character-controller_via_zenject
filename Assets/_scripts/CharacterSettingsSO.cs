using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ZenjectBasedController.Handler;

namespace ZenjectBasedController.Settings
{
    [CreateAssetMenu(menuName = "StylisedCharacter/Character Settings")]
    public class CharacterSettingsSO : ScriptableObjectInstaller<CharacterSettingsSO>
    {
        public RayCastHandler.RayCastHandlerSettings rayCastHandlerSettings;
        [Serializable]
        public class RaycastSettings
        {
            public RayCastHandler.RayCastHandlerSettings RayCastHandlerSettings;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(rayCastHandlerSettings).IfNotBound();
        }
    }
}
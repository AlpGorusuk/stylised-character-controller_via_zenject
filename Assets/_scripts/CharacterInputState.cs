using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZenjectBasedController.State
{
    public class CharacterInputState
    {
        public Vector3 MoveContext
        {
            get;
            set;
        }
        public Vector3 jumpContext
        {
            get;
            set;
        }
    }
}
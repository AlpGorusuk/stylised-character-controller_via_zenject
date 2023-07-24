using UnityEngine;

namespace ZenjectBasedController.Signals
{
    public class JumpSignal
    {
    }
    public class MoveSignal
    {
        public Vector3 MoveVector
        {
            get;
            set;
        }
    }
}
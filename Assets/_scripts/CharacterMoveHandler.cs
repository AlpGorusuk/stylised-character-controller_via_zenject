using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ZenjectBasedController.Settings;
using ZenjectBasedController.State;

namespace ZenjectBasedController.Handler
{
    public class CharacterMoveHandler : IFixedTickable
    {
        readonly CharacterInputState _inputState;
        readonly CharacterMovementSettings _characterMovementSettings;
        readonly RayCastHandler _rayCastHandler;
        readonly CharacterModel _characterModel;
        public CharacterMoveHandler(
            CharacterInputState inputState,
            CharacterMovementSettings characterMovementSettings,
            CharacterModel characterModel,
            RayCastHandler rayCastHandler
            )
        {
            _characterMovementSettings = characterMovementSettings;
            _inputState = inputState;
            _characterModel = characterModel;
            _rayCastHandler = rayCastHandler;
        }
        public void FixedTick()
        {
            CharacterMove();
        }
        /// <summary>
        /// Apply forces to move the character up to a maximum acceleration, with consideration to acceleration graphs.
        /// </summary>
        private void CharacterMove()
        {
            RaycastHit rayHit = _rayCastHandler.RaycastToGround(_characterModel.Position);
            Vector3 m_UnitGoal = _inputState.MoveContext;
            Vector3 _m_GoalVel = _inputState._m_GoalVel;
            Vector3 unitVel = _m_GoalVel.normalized;
            Vector3 _rbVel = _characterModel.GetRigidbodyVelocity();

            float velDot = Vector3.Dot(m_UnitGoal, unitVel);
            Rigidbody hitBody = rayHit.rigidbody;
            float accel = _characterMovementSettings._acceleration * _characterMovementSettings._accelerationFactorFromDot.Evaluate(velDot);
            Vector3 goalVel = m_UnitGoal * _characterMovementSettings._maxSpeed;
            Vector3 otherVel = Vector3.zero;
            _m_GoalVel = Vector3.MoveTowards(_m_GoalVel,
                                            goalVel,
                                            accel * Time.fixedDeltaTime);
            Vector3 neededAccel = (_m_GoalVel - _rbVel) / Time.fixedDeltaTime;
            float maxAccel = _characterMovementSettings._maxAccelForce * _characterMovementSettings._maxAccelerationForceFactorFromDot.Evaluate(velDot);
            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
            float _rbMass = _characterModel.GetRigidbodyMass();
            Vector3 _scaledVec = Vector3.Scale(neededAccel * _rbMass, _characterMovementSettings._moveForceScale);
            float _yScale = _characterModel.GetLocalScale.y;
            Vector3 _expPos = _characterModel.Position + new Vector3(0f, _yScale * _characterMovementSettings._leanFactor, 0f);
            _characterModel.AddForceAtPosition(_scaledVec, _expPos);
        }

        [Serializable]
        public class CharacterMovementSettings
        {
            [Header("Height Spring:")]
            public float _rideHeight = 1.75f; // rideHeight: desired distance to ground (Note, this is distance from the original raycast position (currently centre of transform)). 
            public float _leniancyRideHeight = 1.3f; // 1.3f allows for greater leniancy (as the value will oscillate about the rideHeight).
            public float _rideSpringStrength = 50f; // rideSpringStrength: strength of spring. (?)
            public float _rideSpringDamper = 5f; // rideSpringDampener: dampener of spring. (?)
            public Oscillator _squashAndStretchOcillator;

            [Header("Upright Spring:")]
            public float _uprightSpringStrength = 40f;
            public float _uprightSpringDamper = 5f;

            [Header("Movement:")]
            public float _maxSpeed = 8f;
            public float _acceleration = 200f;
            public float _maxAccelForce = 150f;
            public float _leanFactor = 0.25f;
            public AnimationCurve _accelerationFactorFromDot;
            public AnimationCurve _maxAccelerationForceFactorFromDot;
            public Vector3 _moveForceScale = new Vector3(1f, 0f, 1f);

            [Header("Jump:")]
            public float _jumpForceFactor = 10f;
            public float _riseGravityFactor = 5f;
            public float _fallGravityFactor = 10f; // typically > 1f (i.e. 5f).
            public float _lowJumpFactor = 2.5f;
            public float _jumpBuffer = 0.15f; // Note, jumpBuffer shouldn't really exceed the time of the jump.
            public float _coyoteTime = 0.25f;
        }

    }
}

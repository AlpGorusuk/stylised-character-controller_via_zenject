using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ZenjectBasedController.Signals;

namespace ZenjectBasedController.Handler
{
    public class CharacterMoveHandler : IInitializable, IFixedTickable, IDisposable
    {
        readonly CharacterMoveSettings _characterMoveSettings;
        readonly CharacterJumpSettings _characterJumpSettings;
        readonly RayCastHandler _rayCastHandler;
        readonly CharacterModel _characterModel;
        readonly SignalBus _signalBus;
        public CharacterMoveHandler(
            CharacterMoveSettings characterMoveSettings,
            CharacterJumpSettings characterJumpSettings,
            CharacterModel characterModel,
            RayCastHandler rayCastHandler,
            SignalBus signalBus
            )
        {
            _characterMoveSettings = characterMoveSettings;
            _characterJumpSettings = characterJumpSettings;
            _characterModel = characterModel;
            _rayCastHandler = rayCastHandler;
            _signalBus = signalBus;
        }
        public void Initialize()
        {
            _signalBus.Subscribe<JumpSignal>(OnJumpSignal);
            _signalBus.Subscribe<MoveSignal>(OnMoveSignal);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<JumpSignal>(OnJumpSignal);
            _signalBus.Unsubscribe<MoveSignal>(OnMoveSignal);
        }
        public void FixedTick()
        {
            (RaycastHit rayHit, bool rayHitGround) = _rayCastHandler.RaycastToGround(_characterModel.Position);
            JumpSettings(rayHit, rayHitGround);
            CharacterMove(rayHit, rayHitGround);
            CharacterJump(rayHit, rayHitGround);
        }
        private void JumpSettings(RaycastHit rayHit, bool rayHitGround)
        {
            bool grounded = _rayCastHandler.CheckIfGrounded(rayHitGround, rayHit);
            if (grounded == true)
            {
                _characterJumpSettings._timeSinceUngrounded = 0f;
            }
            else
                _characterJumpSettings._timeSinceUngrounded += Time.fixedDeltaTime;
        }
        /// <summary>
        /// Apply forces to move the character up to a maximum acceleration, with consideration to acceleration graphs.
        /// </summary>
        private void CharacterMove(RaycastHit rayHit, bool rayHitGround)
        {
            Vector3 m_UnitGoal = _characterMoveSettings._moveVector;
            Vector3 _m_GoalVel = Vector3.zero;
            Vector3 unitVel = _m_GoalVel.normalized;
            Vector3 _rbVel = _characterModel.RigidbodyVelocity;

            float velDot = Vector3.Dot(m_UnitGoal, unitVel);
            Rigidbody hitBody = rayHit.rigidbody;
            float accel = _characterMoveSettings._acceleration * _characterMoveSettings._accelerationFactorFromDot.Evaluate(velDot);
            Vector3 goalVel = m_UnitGoal * _characterMoveSettings._maxSpeed;
            Vector3 otherVel = Vector3.zero;
            _m_GoalVel = Vector3.MoveTowards(_m_GoalVel,
                                            goalVel,
                                            accel * Time.fixedDeltaTime);
            Vector3 neededAccel = (_m_GoalVel - _rbVel) / Time.fixedDeltaTime;
            float maxAccel = _characterMoveSettings._maxAccelForce * _characterMoveSettings._maxAccelerationForceFactorFromDot.Evaluate(velDot);
            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
            float _rbMass = _characterModel.GetRigidbodyMass();
            Vector3 _scaledVec = Vector3.Scale(neededAccel * _rbMass, _characterMoveSettings._moveForceScale);
            float _yScale = _characterModel.GetLocalScale.y;
            Vector3 _expPos = _characterModel.Position + new Vector3(0f, _yScale * _characterMoveSettings._leanFactor, 0f);
            _characterModel.AddRbForceAtPosition(_scaledVec, _expPos);
        }
        void OnMoveSignal(MoveSignal args)
        {
            _characterMoveSettings._moveVector = args.MoveVector;
        }
        /// <summary>
        /// Apply force to cause the character to perform a single jump, including coyote time and a jump input buffer.
        /// </summary>
        /// <param name="jumpInput">The player jump input.</param>
        /// <param name="grounded">Whether or not the player is considered grounded.</param>
        /// <param name="rayHit">The rayHit towards the platform.</param>
        private void CharacterJump(RaycastHit rayHit, bool rayHitGround)
        {
            Vector3 _gravitationalForce = _characterModel.GetGravitationalForce();
            if (_characterModel.RigidbodyVelocity.y != 0 && !rayHitGround)
            {
                // Increase downforce for a sudden plummet.
                Vector3 _force = _gravitationalForce * (_characterMoveSettings._riseGravityFactor);
                _characterModel.AddForce(_force);
            }

            if (_characterJumpSettings._timeSinceUngrounded < _characterMoveSettings._coyoteTime) //BURADA BUTTONDAN JUMP READY TRUE OLCAK
            {
                if (_characterJumpSettings._jumpReady)
                {
                    _characterJumpSettings._jumpReady = false;
                    // _shouldMaintainHeight = false;
                    _characterJumpSettings._isJumping = true;
                    _characterModel.RigidbodyVelocity = new Vector3(_characterModel.RigidbodyVelocity.x, 0f, _characterModel.RigidbodyVelocity.z);

                    if (rayHit.distance != 0) // i.e. if the ray has hit
                    {
                        _characterModel.Position = new Vector3(_characterModel.Position.x, _characterModel.Position.y - (rayHit.distance - _characterMoveSettings._rideHeight), _characterModel.Position.z);
                    }
                    _characterModel.AddForce(Vector3.up * _characterMoveSettings._jumpForceFactor, ForceMode.Impulse);
                }
            }
        }
        /// <summary>
        /// For Jump Signal
        /// </summary>
        private void OnJumpSignal()
        {
            _characterJumpSettings._isJumping = false;
            _characterJumpSettings._jumpReady = true;
        }

        [Serializable]
        public class CharacterMoveSettings
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
            public Vector3 _moveVector { get; set; }

            [Header("Jump:")]
            public float _jumpForceFactor = 10f;
            public float _riseGravityFactor = 5f;
            public float _fallGravityFactor = 10f; // typically > 1f (i.e. 5f).
            public float _lowJumpFactor = 2.5f;
            public float _jumpBuffer = 0.15f; // Note, jumpBuffer shouldn't really exceed the time of the jump.
            public float _coyoteTime = 0.25f;
        }
        [Serializable]
        public class CharacterJumpSettings
        {
            public float _timeSinceUngrounded = 0f;
            public bool _jumpReady = true;
            public bool _isJumping = false;
        }
    }
}

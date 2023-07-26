using System;
using UnityEngine;
using Zenject;
using ZenjectBasedController.Handler;

namespace ZenjectBasedController
{
    public class CharacterHeightHandler : IFixedTickable
    {
        readonly CharacterModel _characterModel;
        readonly RayCastHandler _rayCastHandler;
        readonly RayCastHandler.RayCastHandlerSettings _rayCastHandlerSettings;
        readonly CharacterMoveHandler.CharacterMoveSettings _characterMovementSettings;
        readonly HeightSettings _heightSettings;
        public CharacterHeightHandler(

            CharacterModel characterModel,
            RayCastHandler rayCastHandler,
            HeightSettings heightSettings,
            CharacterMoveHandler.CharacterMoveSettings characterMoveSettings,
            RayCastHandler.RayCastHandlerSettings rayCastHandlerSettings
            )
        {
            _characterModel = characterModel;
            _rayCastHandler = rayCastHandler;
            _heightSettings = heightSettings;
            _characterMovementSettings = characterMoveSettings;
            _rayCastHandlerSettings = rayCastHandlerSettings;
        }

        public void FixedTick()
        {
            (RaycastHit rayHit, bool rayHitGround) = _rayCastHandler.RaycastToGround(_characterModel.Position);
            MaintainUpright(_characterMovementSettings._moveVector, rayHit);
            MaintainHeight(rayHit, rayHitGround);
        }

        /// <summary>
        /// Determines the relative velocity of the character to the ground beneath,
        /// Calculates and applies the oscillator force to bring the character towards the desired ride height.
        /// Additionally applies the oscillator force to the squash and stretch oscillator, and any object beneath.
        /// </summary>
        /// <param name="rayHit">Information about the RaycastToGround.</param>
        private void MaintainHeight(RaycastHit rayHit, bool rayHitGround)
        {
            if (!rayHitGround || (_characterModel.RigidbodyVelocity.y != 0 && !rayHitGround)) { return; }
            Vector3 vel = _characterModel.RigidbodyVelocity;
            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = rayHit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.velocity;
            }
            float rayDirVel = Vector3.Dot(_rayCastHandlerSettings._rayDir, vel);
            float otherDirVel = Vector3.Dot(_rayCastHandlerSettings._rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;
            float currHeight = rayHit.distance - _characterMovementSettings._rideHeight;
            float springForce = (currHeight * _characterMovementSettings._rideSpringStrength) - (relVel * _characterMovementSettings._rideSpringDamper);
            Vector3 maintainHeightForce = -_characterModel.GetGravitationalForce() + springForce * Vector3.down;
            Vector3 oscillationForce = springForce * Vector3.down;
            _characterModel.AddForce(maintainHeightForce);
            // _squashAndStretchOcillator.ApplyForce(oscillationForce);
            //Debug.DrawLine(transform.position, transform.position + (_rayDir * springForce), Color.yellow);

            // Apply force to objects beneath
            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(-maintainHeightForce, rayHit.point);
            }
        }

        /// <summary>
        /// Adds torque to the character to keep the character upright, acting as a torsional oscillator (i.e. vertically flipped pendulum).
        /// </summary>
        /// <param name="yLookAt">The input look rotation.</param>
        /// <param name="rayHit">The rayHit towards the platform.</param>
        private void MaintainUpright(Vector3 yLookAt, RaycastHit rayHit = new RaycastHit())
        {
            _heightSettings._uprightTargetRot = CalculateTargetRotation(yLookAt);

            Quaternion currentRot = _characterModel.Rotation;
            Quaternion toGoal = MathsUtils.ShortestRotation(_heightSettings._uprightTargetRot, currentRot);

            Vector3 rotAxis;
            float rotDegrees;

            toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
            rotAxis.Normalize();

            float rotRadians = rotDegrees * Mathf.Deg2Rad;

            _characterModel.AddTorque((rotAxis * (rotRadians * _characterMovementSettings._uprightSpringStrength)) - (_characterModel.RigidbodyAngularVelocity * _characterMovementSettings._uprightSpringDamper));
        }
        /// <summary>
        /// Determines the desired y rotation for the character, with account for platform rotation.
        /// </summary>
        /// <param name="yLookAt">The input look rotation.</param>
        /// <param name="rayHit">The rayHit towards the platform.</param>
        private Quaternion CalculateTargetRotation(Vector3 yLookAt)
        {
            if (yLookAt != Vector3.zero)
            {
                _heightSettings._lastTargetVec = yLookAt;
            }
            return Quaternion.LookRotation(_heightSettings._lastTargetVec, Vector3.up);
        }
        public class HeightSettings
        {
            public Quaternion _uprightTargetRot = Quaternion.identity; // Adjust y value to match the desired direction to face.
            public Vector3 _lastTargetVec = Vector3.zero;
        }
    }
}

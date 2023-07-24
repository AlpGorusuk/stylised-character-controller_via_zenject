using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenjectBasedController.Handler;

namespace ZenjectBasedController
{
    public class CharacterHeightHandler
    {
        readonly CharacterModel _characterModel;
        readonly CharacterMoveHandler.CharacterMoveSettings _characterMovementSettings;
        readonly RayCastHandler.RayCastHandlerSettings rayCastHandlerSettings;
        public CharacterHeightHandler(

            CharacterModel characterModel
            )
        {
            _characterModel = characterModel;
        }

        /// <summary>
        /// Determines the relative velocity of the character to the ground beneath,
        /// Calculates and applies the oscillator force to bring the character towards the desired ride height.
        /// Additionally applies the oscillator force to the squash and stretch oscillator, and any object beneath.
        /// </summary>
        /// <param name="rayHit">Information about the RaycastToGround.</param>
        private void MaintainHeight(RaycastHit rayHit)
        {
            Vector3 vel = _characterModel.RigidbodyVelocity;
            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = rayHit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.velocity;
            }
            float rayDirVel = Vector3.Dot(rayCastHandlerSettings._rayDir, vel);
            float otherDirVel = Vector3.Dot(rayCastHandlerSettings._rayDir, otherVel);

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
        // private void MaintainUpright(Vector3 yLookAt, RaycastHit rayHit = new RaycastHit())
        // {
        //     CalculateTargetRotation(yLookAt, rayHit);

        //     Quaternion currentRot = transform.rotation;
        //     Quaternion toGoal = MathsUtils.ShortestRotation(_uprightTargetRot, currentRot);

        //     Vector3 rotAxis;
        //     float rotDegrees;

        //     toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        //     rotAxis.Normalize();

        //     float rotRadians = rotDegrees * Mathf.Deg2Rad;

        //     _rb.AddTorque((rotAxis * (rotRadians * _uprightSpringStrength)) - (_rb.angularVelocity * _uprightSpringDamper));
        // }
        /// <summary>
        /// Determines the desired y rotation for the character, with account for platform rotation.
        /// </summary>
        /// <param name="yLookAt">The input look rotation.</param>
        /// <param name="rayHit">The rayHit towards the platform.</param>
        // private void CalculateTargetRotation(Vector3 yLookAt, RaycastHit rayHit = new RaycastHit())
        // {
        //     if (didLastRayHit)
        //     {
        //         _lastTargetRot = _uprightTargetRot;
        //         try
        //         {
        //             _platformInitRot = transform.parent.rotation.eulerAngles;
        //         }
        //         catch
        //         {
        //             _platformInitRot = Vector3.zero;
        //         }
        //     }
        //     if (rayHit.rigidbody == null)
        //     {
        //         didLastRayHit = true;
        //     }
        //     else
        //     {
        //         didLastRayHit = false;
        //     }

        //     if (yLookAt != Vector3.zero)
        //     {
        //         _uprightTargetRot = Quaternion.LookRotation(yLookAt, Vector3.up);
        //         _lastTargetRot = _uprightTargetRot;
        //         try
        //         {
        //             _platformInitRot = transform.parent.rotation.eulerAngles;
        //         }
        //         catch
        //         {
        //             _platformInitRot = Vector3.zero;
        //         }
        //     }
        //     else
        //     {
        //         try
        //         {
        //             Vector3 platformRot = transform.parent.rotation.eulerAngles;
        //             Vector3 deltaPlatformRot = platformRot - _platformInitRot;
        //             float yAngle = _lastTargetRot.eulerAngles.y + deltaPlatformRot.y;
        //             _uprightTargetRot = Quaternion.Euler(new Vector3(0f, yAngle, 0f));
        //         }
        //         catch
        //         {

        //         }
        //     }
        // }
    }
}

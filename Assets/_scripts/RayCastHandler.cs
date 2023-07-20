using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ZenjectBasedController.Settings;

namespace ZenjectBasedController.Handler
{
    public class RayCastHandler
    {
        readonly CharacterMoveHandler.CharacterMovementSettings _characterMovementSettings;
        readonly RayCastHandlerSettings _rayCastHandlerSettings;
        public RayCastHandler(
            RayCastHandlerSettings rayCastHandlerSettings
            )
        {
            _rayCastHandlerSettings = rayCastHandlerSettings;
        }
        public bool CheckIfGrounded(bool rayHitGround, RaycastHit rayHit)
        {
            bool grounded;
            float rideHeight = _characterMovementSettings._rideHeight;
            float _leniancyRideHeight = _characterMovementSettings._leniancyRideHeight;
            if (rayHitGround == true)
            {
                grounded = rayHit.distance <= rideHeight * _leniancyRideHeight;
            }
            else
            {
                grounded = false;
            }
            return grounded;
        }

        /// <summary>
        /// Perfom raycast towards the ground.
        /// </summary>
        /// <returns>Whether the ray hit the ground, and information about the ray.</returns>
        public RaycastHit RaycastToGround(Vector3 _pos)
        {
            RaycastHit rayHit;
            Vector3 pos = _pos;
            Vector3 rayDir = _rayCastHandlerSettings._rayDir;
            Ray rayToGround = new Ray(_pos, rayDir);
            float rayToGroundLength = _rayCastHandlerSettings._rayToGroundLength;
            Physics.Raycast(rayToGround, out rayHit, rayToGroundLength, _rayCastHandlerSettings._terrainLayer.value);
            Debug.DrawRay(_pos, rayDir * rayToGroundLength, Color.blue);
            return (rayHit);
        }
        [Serializable]
        public class RayCastHandlerSettings
        {
            [Header("Raycast Settings:")]
            public bool _adjustInputsToCameraAngle = false;
            public LayerMask _terrainLayer;
            public Vector3 _rayDir = Vector3.down;
            public float _rayToGroundLength = 3f; // rayToGroundLength: max distance of raycast to ground (Note, this should be greater than the CharacterMovementSettings._rideHeight).
        }
    }
}
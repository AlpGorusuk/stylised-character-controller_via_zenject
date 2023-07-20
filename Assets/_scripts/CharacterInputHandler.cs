using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using ZenjectBasedController.State;

namespace ZenjectBasedController.Handler
{
    public class CharacterInputHandler : IInitializable, IDisposable
    {
        readonly CharacterInputState _inputState;
        private WASD _playerInputAction;
        public void Initialize()
        {
            _playerInputAction = new WASD();
            _playerInputAction.Enable();

            _playerInputAction.Player.Move.performed += MoveInputAction;
            _playerInputAction.Player.Jump.performed += JumpInputAction;
        }

        public void Dispose()
        {
            _playerInputAction.Dispose();

            _playerInputAction.Player.Move.performed -= MoveInputAction;
            _playerInputAction.Player.Jump.performed -= JumpInputAction;
        }

        public CharacterInputHandler(CharacterInputState inputState)
        {
            _inputState = inputState;
        }
        /// <summary>
        /// Reads the player movement input.
        /// </summary>
        /// <param name="context">The move input's context.</param>
        public void MoveInputAction(InputAction.CallbackContext context)
        {
            Vector2 _context = context.ReadValue<Vector2>();
            Vector3 _moveContext = new Vector3(_context.x, 0, _context.y);
            _inputState.MoveContext = _moveContext;
            Debug.Log(_moveContext);
        }
        /// <summary>
        /// Reads the player jump input.
        /// </summary>
        /// <param name="context">The jump input's context.</param>
        public void JumpInputAction(InputAction.CallbackContext context)
        {
            float jumpContext = context.ReadValue<float>();
            _inputState.jumpContext = new Vector3(0, jumpContext, 0);
            Debug.Log(context.ReadValue<float>());

            // if (context.started) // button down
            // {
            //     _timeSinceJumpPressed = 0f;
            // }
        }
    }
}

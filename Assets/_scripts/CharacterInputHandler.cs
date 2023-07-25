using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using ZenjectBasedController.Signals;

namespace ZenjectBasedController.Handler
{
    public class CharacterInputHandler : IInitializable, IDisposable
    {
        private WASD _playerInputAction;
        readonly SignalBus _signalBus;
        public void Initialize()
        {
            _playerInputAction = new WASD();
            _playerInputAction.Enable();

            _playerInputAction.Player.Move.performed += MoveInputAction;
            _playerInputAction.Player.Move.canceled += CanceledMoveInputAction;
            _playerInputAction.Player.Jump.performed += JumpInputAction;
        }

        public void Dispose()
        {
            _playerInputAction.Dispose();

            _playerInputAction.Player.Move.performed -= MoveInputAction;
            _playerInputAction.Player.Move.canceled -= CanceledMoveInputAction;

            _playerInputAction.Player.Jump.performed -= JumpInputAction;
        }

        public CharacterInputHandler(
            SignalBus signalBus
            )
        {
            _signalBus = signalBus;
        }
        /// <summary>
        /// Reads the player movement input.
        /// </summary>
        /// <param name="context">The move input's context.</param>
        public void MoveInputAction(InputAction.CallbackContext context)
        {
            Vector2 _context = context.ReadValue<Vector2>();
            Vector3 _moveContext = new Vector3(_context.x, 0, _context.y);

            MoveSignal moveSignal = new MoveSignal() { MoveVector = _moveContext };
            _signalBus.Fire(moveSignal);
        }
        public void CanceledMoveInputAction(InputAction.CallbackContext context)
        {
            MoveSignal moveSignal = new MoveSignal() { MoveVector = Vector3.zero };
            _signalBus.Fire(moveSignal);
        }
        /// <summary>
        /// Reads the player jump input.
        /// </summary>
        /// <param name="context">The jump input's context.</param>
        public void JumpInputAction(InputAction.CallbackContext context)
        {
            _signalBus.Fire<JumpSignal>();
        }
    }
}

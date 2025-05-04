using System;
using PJH.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace PJH.Input
{
    [CreateAssetMenu(menuName = "SO/PlayerInput")]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions, Controls.IUIActions
    {
        public event Action EvasionEvent;
        public event Action AttackEvent;
        public event Action InteractEvent;
        public event Action EscEvent;
        public event Action<bool> Skill1Event;
        public event Action<bool> Skill2Event;
        public event Action<bool> Skill3Event;
        public event Action<bool> ChargeAttackEvent;
        public event Action<Vector2> MovementEvent;
        public event Action<int> ChangeWeaponEvent;
        public Vector2 Movement { get; private set; }
        public Vector2 MousePosition { get; private set; }
        private Vector3 _beforeMouseWorldPos;

        private Controls _controls;
        public Controls Controls => _controls;

        private bool _isPrevChargeAttack,
            _isSkill1Holding,
            _isSkill2Holding,
            _isSkill3Holding;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
                _controls.UI.SetCallbacks(this);
            }

            EnablePlayerInput(true);
            EnableUIInput(true);
        }

        private void OnDisable()
        {
            EnablePlayerInput(false);
            EnableUIInput(false);
        }

        public void ResetControl()
        {
            EvasionEvent = null;
            AttackEvent = null;
            InteractEvent = null;
            EscEvent = null;
            Skill1Event = null;
            Skill2Event = null;
            Skill3Event = null;
            ChargeAttackEvent = null;
            MovementEvent = null;
            ChangeWeaponEvent = null;
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            Movement = context.ReadValue<Vector2>();
            MovementEvent?.Invoke(Movement);
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            MousePosition = context.ReadValue<Vector2>();
        }

        public void OnEvasion(InputAction.CallbackContext context)
        {
            if (context.performed)
                EvasionEvent?.Invoke();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (context.interaction is HoldInteraction)
                {
                    _isPrevChargeAttack = true;
                    ChargeAttackEvent?.Invoke(true);
                }

                if (context.interaction is PressInteraction)
                {
                    AttackEvent?.Invoke();
                }
            }

            if (_isPrevChargeAttack && context.canceled)
            {
                _isPrevChargeAttack = false;
                ChargeAttackEvent?.Invoke(false);
            }
        }

        public void OnChangeWepoan_1(InputAction.CallbackContext context)
        {
            if (context.performed)
                ChangeWeaponEvent?.Invoke(0);
        }

        public void OnChangeWepoan_2(InputAction.CallbackContext context)
        {
            if (context.performed)
                ChangeWeaponEvent?.Invoke(1);
        }

        public void OnChangeWepoan_3(InputAction.CallbackContext context)
        {
            if (context.performed)
                ChangeWeaponEvent?.Invoke(2);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
                InteractEvent?.Invoke();
        }

        public void OnSkill1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (context.interaction is HoldInteraction)
                {
                    _isSkill1Holding = true;
                    Skill1Event?.Invoke(true);
                }

                if (context.interaction is PressInteraction)
                {
                    Skill1Event?.Invoke(false);
                }
            }

            if (_isSkill1Holding && context.canceled)
            {
                _isSkill1Holding = false;
                Skill1Event?.Invoke(false);
            }
        }

        public void OnSkill2(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (context.interaction is HoldInteraction)
                {
                    _isSkill2Holding = true;
                    Skill2Event?.Invoke(true);
                }

                if (context.interaction is PressInteraction)
                {
                    Skill2Event?.Invoke(false);
                }
            }

            if (_isSkill2Holding && context.canceled)
            {
                _isSkill2Holding = false;

                Skill2Event?.Invoke(false);
            }
        }

        public void OnSkill3(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (context.interaction is HoldInteraction)
                {
                    _isSkill3Holding = true;

                    Skill3Event?.Invoke(true);
                }

                if (context.interaction is PressInteraction)
                {
                    Skill3Event?.Invoke(false);
                }
            }

            if (_isSkill3Holding && context.canceled)
            {
                _isSkill3Holding = false;

                Skill3Event?.Invoke(false);
            }
        }

        public void OnESC(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                EscEvent?.Invoke();
            }
        }

        public Vector3 GetWorldMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(MousePosition);

            if (Physics.Raycast(ray, out var hitInfo, Camera.main.farClipPlane, Define.MLayerMask.WhatIsGround))
            {
                _beforeMouseWorldPos = hitInfo.point;
            }

            return _beforeMouseWorldPos;
        }

        public void EnablePlayerInput(bool enabled)
        {
            if (enabled)
                _controls.Player.Enable();
            else
                _controls.Player.Disable();
        }

        public void EnableUIInput(bool enabled)
        {
            if (enabled)
                _controls.UI.Enable();
            else
                _controls.UI.Disable();
        }
    }
}
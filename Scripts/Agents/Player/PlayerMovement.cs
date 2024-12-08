using System;
using UnityEngine;


namespace PJH.Agent.Player
{
    using Core;

    public class PlayerMovement : MonoBehaviour, MInterface.IAgentComponent
    {
        [SerializeField] private float _gravity = -9.8f, _rotationSpeed;
        [SerializeField] private int _reduceStaminaWhenEvasion;

        private CharacterController _characterController;
        public CharacterController CharacterController => _characterController;
        public bool IsGround => _characterController.isGrounded;
        public bool IsEvasion { get; private set; }
        public bool CanMove { get; set; }

        public event Action<Vector3> MovementEvent;
        public event Action EvasionEvent;

        private Player _player;
        private Vector3 _movement;
        private float _yVelocity;
        private PlayerAnimator _animatorCompo;

        public void Initialize(Agent agent)
        {
            CanMove = true;
            _player = agent as Player;
            _characterController = GetComponent<CharacterController>();
            _animatorCompo = _player.GetCompo<PlayerAnimator>();
        }

        private void HandleAnimatorMoveEvent(Vector3 deltaPosition)
        {
            _movement = deltaPosition;
            Vector3 input = _player.PlayerInput.Movement;
            if (IsEvasion)
            {
                Vector3 dir = Vector3.zero;
                if (input.sqrMagnitude > 0)
                {
                    dir = Quaternion.Euler(0, 45f, 0) * new Vector3(input.x, 0, input.y);
                }

                if (_player.PlayerInput.Movement.sqrMagnitude > 0)
                {
                    Vector3 velocity = dir;
                    velocity.y = 0;
                    Quaternion look = Quaternion.LookRotation(velocity);
                    Quaternion rot =
                        Quaternion.Slerp(transform.rotation, look, _rotationSpeed * Time.fixedDeltaTime);
                    transform.rotation = rot;
                }
            }

            _movement.y = _yVelocity;
        }

        private void HandleEvasionEvent()
        {
            if (!CanEvasion()) return;
            IsEvasion = true;
            _player.CurrentStamina -= _reduceStaminaWhenEvasion;
            EvasionEvent?.Invoke();
        }

        private bool CanEvasion()
        {
            if (_player.IsHitting || !CanMove || _player.IsUsingSkill) return false;

            var attackCompo = _player.GetCompo<PlayerAttack>();
            return _player.CurrentStamina > 0 && !IsEvasion && !attackCompo.IsAttackCharging;
        }

        private void HandleEndEvasionEvent()
        {
            IsEvasion = false;
        }


        public void AfterInit()
        {
            _player.HealthCompo.ApplyDamagedEvent += HandleApplyDamagedEvent;
            _player.HealthCompo.DeathEvent += HandleDeathEvent;

            _player.PlayerInput.EvasionEvent += HandleEvasionEvent;
            _animatorCompo.AnimatorMoveEvent += HandleAnimatorMoveEvent;
            _animatorCompo.EndEvasionEvent += HandleEndEvasionEvent;
            _animatorCompo.EndComboEvent += HandleEndComboEvent;
            _player.GetCompo<PlayerAttack>().AttackEvent += HandleAttackEvent;
        }

        private void HandleDeathEvent()
        {
            Destroy(_characterController);
        }

        private void HandleApplyDamagedEvent(int damage)
        {
            HandleEndEvasionEvent();
        }

        private void HandleEndComboEvent()
        {
            IsEvasion = false;
        }

        private void HandleAttackEvent(int comboCount)
        {
            IsEvasion = false;
        }


        public void Dispose()
        {
            _player.HealthCompo.ApplyDamagedEvent -= HandleApplyDamagedEvent;
            _player.HealthCompo.DeathEvent -= HandleDeathEvent;

            _player.PlayerInput.EvasionEvent -= HandleEvasionEvent;
            _animatorCompo.AnimatorMoveEvent -= HandleAnimatorMoveEvent;
            _animatorCompo.EndEvasionEvent -= HandleEndEvasionEvent;
            _animatorCompo.EndComboEvent -= HandleEndComboEvent;

            _player.GetCompo<PlayerAttack>().AttackEvent -= HandleAttackEvent;
        }

        private void FixedUpdate()
        {
            if (!_characterController) return;
            CalculateMove();
            ApplyGravity();
            ApplyRotation();
            CharacterMove();
        }

        private void CalculateMove()
        {
            if (!CanMove || _animatorCompo.IsRootMotion) return;
            Vector2 moveInput = _player.PlayerInput.Movement;

            _movement = Quaternion.Euler(0, 45f, 0) * new Vector3(moveInput.x, 0, moveInput.y);

            MovementEvent?.Invoke(_movement);

            float speed = 0;
            speed = _player.GetStat().moveSpeed.GetValue();

            _movement *= speed * Time.fixedDeltaTime;
        }


        private void ApplyGravity()
        {
            if (IsGround && _yVelocity < 0)
                _yVelocity = -.02f;
            else
                _yVelocity += _gravity * Time.fixedDeltaTime;

            _movement.y = _yVelocity;
        }

        private void ApplyRotation()
        {
            if (!CanMove || _animatorCompo.IsRootMotion) return;

            if (_player.PlayerInput.Movement.sqrMagnitude > 0)
            {
                Vector3 velocity = _movement;
                velocity.y = 0;
                Quaternion look = Quaternion.LookRotation(velocity);
                Quaternion rot =
                    Quaternion.Slerp(transform.rotation, look, _rotationSpeed * Time.fixedDeltaTime);
                transform.rotation = rot;
            }
        }

        private void CharacterMove()
        {
            if (!CanMove) return;
            _characterController.Move(_movement);
        }
    }
}
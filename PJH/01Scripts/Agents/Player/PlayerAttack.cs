using System;
using DG.Tweening;
using Game.Events;
using PJH.EquipmentSkillSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Agent.Player
{
    using Core;

    public class PlayerAttack : SerializedMonoBehaviour, MInterface.IAgentComponent
    {
        public event Action<int> AttackEvent;
        public event Action<bool> ChargeAttackEvent;

        public bool IsAttacking { get; private set; }
        public bool IsPowerfulAttacking { get; private set; }
        public bool IsAttackCharging { get; private set; }
        [SerializeField] private GameEventChannelSO _gameEventChannel;

        private bool _comboPossible;
        private int _comboCount;
        private Player _player;


        private Sequence _raiseChargeAttackReachMaxTimeEventSequence;


        private float _currentReComboableTime;
        private bool _reCombo;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
        }

        public void AfterInit()
        {
            _player.PlayerInput.AttackEvent += HandleAttackEvent;
            _player.PlayerInput.ChargeAttackEvent += HandleChargeChargeAttackEvent;
            _player.GetCompo<PlayerMovement>().EvasionEvent += HandleEvasionEvent;
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.GetHitEvent += HandleGetHitEvent;
            animatorCompo.EndComboEvent += EndCombo;
            animatorCompo.ComboPossibleEvent += ComboPossible;
        }

        private void HandleGetHitEvent(bool isHit)
        {
            EndCombo();
        }


        private void HandleChargeChargeAttackEvent(bool isAttackCharging)
        {
            float maxChargeAttackTime = 0;
            var weapon = _player.GetCompo<PlayerEquipmentController>().GetWeapon();
            if (weapon is not MInterface.IAttackChargeable) return;
            Vector3 worldMousePosition = _player.PlayerInput.GetWorldMousePosition();
            transform.DOLookAt(worldMousePosition, .2f, AxisConstraint.Y);
            if (isAttackCharging)
            {
                var hammerPassiveSkill = weapon.EquipmentSkillSystemCompo.GetSkill<HammerPassiveSkill>();
                if (!hammerPassiveSkill.AttemptUseSkill()) return;
                if (!CanAttack()) return;
                ChargeAttackEvent?.Invoke(isAttackCharging);
                maxChargeAttackTime = hammerPassiveSkill.maxChargeAttackTime;
                _raiseChargeAttackReachMaxTimeEventSequence = DOTween.Sequence();

                _raiseChargeAttackReachMaxTimeEventSequence.Append(DOVirtual.DelayedCall(maxChargeAttackTime, () =>
                {
                    var evt = GameEvents.ChargeAttackReachMaxTime;
                    _gameEventChannel.RaiseEvent(evt);
                }));

                IsPowerfulAttacking = true;
                HandleAttackEvent();
            }
            else
            {
                ChargeAttackEvent?.Invoke(false);

                if (_raiseChargeAttackReachMaxTimeEventSequence != null &&
                    _raiseChargeAttackReachMaxTimeEventSequence.IsActive())
                    _raiseChargeAttackReachMaxTimeEventSequence.Kill();
            }

            var evt = GameEvents.ChargeAttack;
            evt.isAttackCharging = isAttackCharging;
            evt.maxChargeAttackTime = maxChargeAttackTime;
            _gameEventChannel.RaiseEvent(evt);
            IsAttackCharging = isAttackCharging;
        }

        private void HandleEvasionEvent()
        {
            if (!IsAttacking) return;
            EndCombo();
        }

        private void HandleAttackEvent()
        {
            if (!CanAttack()) return;
            _comboPossible = false;
            IsAttacking = true;
            _currentReComboableTime = Time.time + _player.GetStat().reComoboableTime.GetValue();

            var maxComboCount = _player.GetCompo<PlayerEquipmentController>().GetWeapon().maxComboCount;

            AttackEvent?.Invoke(_comboCount);
            Vector3 worldMousePosition = _player.PlayerInput.GetWorldMousePosition();
            transform.DOLookAt(worldMousePosition, .2f, AxisConstraint.Y);
            _comboCount = (_comboCount + 1) % maxComboCount;
            if (_comboCount == 0) _reCombo = true;
            else _reCombo = false;
        }

        private bool CanAttack()
        {
            var equipmentCompo = _player.GetCompo<PlayerEquipmentController>();
            var weapon = equipmentCompo.GetWeapon();
            if (_reCombo && Time.time < _currentReComboableTime)
                return false;
            if (_player.IsHitting || _player.IsUsingSkill)
                return false;
            var movementCompo = _player.GetCompo<PlayerMovement>();
            if (movementCompo.IsEvasion)
                return false;
            if ((IsAttacking && !_comboPossible))
                return false;
            return weapon != null;
        }

        public void Dispose()
        {
            _player.PlayerInput.AttackEvent -= HandleAttackEvent;
            _player.PlayerInput.ChargeAttackEvent -= HandleChargeChargeAttackEvent;
            _player.GetCompo<PlayerMovement>().EvasionEvent -= HandleEvasionEvent;
            var animatorCompo = _player.GetCompo<PlayerAnimator>();

            animatorCompo.GetHitEvent -= HandleGetHitEvent;
            animatorCompo.EndComboEvent -= EndCombo;
            animatorCompo.ComboPossibleEvent -= ComboPossible;
        }

        private void EndCombo()
        {
            var weapon = _player.GetCompo<PlayerEquipmentController>()?.GetWeapon();
            (weapon as MInterface.IMeleeWeapon)
                ?.EnableMeleeTrail(false);
            IsAttacking = false;
            IsPowerfulAttacking = false;
            _comboCount = 0;
            var movementCompo = _player.GetCompo<PlayerMovement>();
            if (!movementCompo.IsEvasion)
            {
                _player.GetCompo<PlayerIK>().LookAnimatorCompo.SwitchLooking(true, .1f);
            }
        }

        private void ComboPossible()
        {
            _comboPossible = true;

            if (!IsAttackCharging)
            {
                IsPowerfulAttacking = false;
            }
        }
    }
}
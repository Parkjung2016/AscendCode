using System;
using DG.Tweening;
using Game.Events;
using PJH.Agent.Animation;
using PJH.Manager;
using UnityEngine;

namespace PJH.Agent.Player
{
    using Equipment.Weapon;

    public class PlayerAnimator : AgentAnimator
    {
        [SerializeField] private AnimParamSO _velocityParam,
            _evasionParam,
            _attackParam,
            comboCountParam,
            _attackSpeedParam,
            _getHitParam,
            _deadParam,
            _skillParam,
            _skillCountParam,
            _skillSpeedParam;


        public event Action<bool> GetHitEvent;
        public event Action<bool> EnableBodyTrailEffectEvent;
        public event Action<bool> EnableMeleeTrailEvent;

        public event Action GroundSlamFeedbackEvent;
        public event Action PowerfulGroundSlamFeedbackEvent;
        public event Action DetectWeaponColliderEvent;
        public event Action WeaponShootEvent;
        public event Action EndEvasionEvent;
        public event Action EndComboEvent;
        public event Action ComboPossibleEvent;
        public event Action PlayLunarSlashEffectEvent;
        public event Action FootstepEvent;
        public event Action WeaponAttackSoundEvent;
        public event Action EndUseSkillEvent;
        public event Action PlayWrathfulStrikeSkillFeedbackEvent;
        public event Action PlayZoneOfAgonySkillEffectEvent;
        public event Action ShotBarusSkillArrowEvent;
        public event Action ShotAusolSkillSlowAnimEvent;
        public event Action ShotAusolSkillArrowEvent;
        public event Action PlayPhantomStrikeSlashSoundEvent;
        public event Action ShotVolleySkillArrowEvent;

        private Player _player;

        private int _currentLayerIndex;

        private Sequence _animatorSpeedSeq;


        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);

            _player = agent as Player;
        }

        public override void AfterInit()
        {
            var moveCompo = _player.GetCompo<PlayerMovement>();
            moveCompo.MovementEvent += HandleMovementEvent;
            moveCompo.EvasionEvent += HandleEvasion;
            var attackCompo = _player.GetCompo<PlayerAttack>();
            attackCompo.AttackEvent += HandleAttackEvent;
            attackCompo.ChargeAttackEvent += HandleChargeAttackEvent;


            var equipmentCompo = _player.GetCompo<PlayerEquipmentController>();
            equipmentCompo.WeaponChangeEvent += HandleWeaponChangeStart;


            _player.HealthCompo.ApplyDamagedEvent += HandleDamagedEvent;
            _player.HealthCompo.DeathEvent += HandleDeathEvent;
        }


        private void HandleChargeAttackEvent(bool isAttackHolding)
        {
            if (_animatorSpeedSeq != null && _animatorSpeedSeq.IsActive()) _animatorSpeedSeq.Kill();
            if (isAttackHolding)
            {
                _animatorSpeedSeq = DOTween.Sequence();
                _animatorSpeedSeq.Append(DOTween.To(() => AnimatorCompo.speed, x => AnimatorCompo.speed = x, 0, 1f));
            }
            else
            {
                AnimatorCompo.speed = 1;
            }
        }


        public override void Dispose()
        {
            var moveCompo = _player.GetCompo<PlayerMovement>();
            moveCompo.MovementEvent -= HandleMovementEvent;
            moveCompo.EvasionEvent -= HandleEvasion;
            var attackCompo = _player.GetCompo<PlayerAttack>();
            attackCompo.AttackEvent -= HandleAttackEvent;

            attackCompo.ChargeAttackEvent -= HandleChargeAttackEvent;


            var equipmentCompo = _player.GetCompo<PlayerEquipmentController>();
            equipmentCompo.WeaponChangeEvent -= HandleWeaponChangeStart;

            _player.HealthCompo.ApplyDamagedEvent -= HandleDamagedEvent;
            _player.HealthCompo.DeathEvent -= HandleDeathEvent;
        }

        private void PlayHitAnimation(bool isPlay)
        {
            if (_player.HealthCompo.IsDead) return;

            if (isPlay)
            {
                SetRootMotion(true);
                SetParam(_getHitParam);
            }
            else
            {
                SetRootMotion(false);
            }
        }

        public void EndEvasion()
        {
            EndEvasionEvent?.Invoke();
            SetRootMotion(false);
        }

        public void CallEndCombo()
        {
            bool isNextAnimationAttack = AnimatorCompo.GetNextAnimatorStateInfo(_currentLayerIndex).IsTag("Attack");
            if (isNextAnimationAttack) return;
            SetRootMotion(false);
            EndComboEvent?.Invoke();
        }

        public void CallComboPossible()
        {
            if (_player.HealthCompo.IsDead) return;

            var movementCompo = _player.GetCompo<PlayerMovement>();
            if (movementCompo.IsEvasion) return;
            ComboPossibleEvent?.Invoke();
        }

        public void CallFootstep()
        {
            if (_player.HealthCompo.IsDead) return;

            var isAttacking = _player.GetCompo<PlayerAttack>().IsAttacking;
            var isEvasion = _player.GetCompo<PlayerMovement>().IsEvasion;
            if (_player.PlayerInput.Movement.sqrMagnitude > 0 || isAttacking || isEvasion || _player.IsUsingSkill)
                FootstepEvent?.Invoke();
        }

        public void CallPlayZoneOfAgonySkillEffect()
        {
            PlayZoneOfAgonySkillEffectEvent?.Invoke();
        }

        public void CallShotVolleySkillArrowEvent()
        {
            ShotVolleySkillArrowEvent?.Invoke();
        }

        public void CallShotBarusSkillArrow()
        {
            ShotBarusSkillArrowEvent?.Invoke();
        }

        public void CallShotAusolSkillSlowAnim()
        {
            ShotAusolSkillSlowAnimEvent?.Invoke();
        }

        public void CallShotAusolSkillArrow()
        {
            ShotAusolSkillArrowEvent?.Invoke();
        }

        public void CallPlayPhantomStrikeSlashSound()
        {
            PlayPhantomStrikeSlashSoundEvent?.Invoke();
        }

        public void CallEndUseSkill()
        {
            EndUseSkillEvent?.Invoke();
        }

        public void CallPlayWrathfulStrikeSkillFeedback()
        {
            PlayWrathfulStrikeSkillFeedbackEvent?.Invoke();
        }

        public void CallEndGetHit()
        {
            GetHitEvent?.Invoke(false);
            PlayHitAnimation(false);
        }

        public void CallPlayLunarSlashEffect()
        {
            PlayLunarSlashEffectEvent?.Invoke();
        }

        public void CallPlayGroundImpactSound()
        {
            Game.Core.SoundManager.Instance.PlayGroundImpactSound(_player.transform.position);
        }

        public void CallEnableBodyTrailEffect(bool enabled) => EnableBodyTrailEffectEvent?.Invoke(enabled);

        public void CallEnableMeleeTrail(bool enabled)
        {
            EnableMeleeTrailEvent?.Invoke(enabled);
        }


        public void CallPlayGroundSlamFeedback()
        {
            if (_player.HealthCompo.IsDead) return;

            var isPowerfulAttacking = _player.GetCompo<PlayerAttack>().IsPowerfulAttacking;
            if (isPowerfulAttacking)
                PowerfulGroundSlamFeedbackEvent?.Invoke();
            else
                GroundSlamFeedbackEvent?.Invoke();
        }


        public void CallWeaponShoot() => WeaponShootEvent?.Invoke();

        public void CallDetectWeaponCollider() => DetectWeaponColliderEvent?.Invoke();
        public void CallPlayWeaponAttackSound() => WeaponAttackSoundEvent?.Invoke();

        public void PlaySkillAnimation(int skillIdx)
        {
            float skillSpeed = _player.GetStat().skillSpeed[skillIdx].GetValue();
            SetParam(_skillSpeedParam, skillSpeed);
            SetParam(_skillCountParam, skillIdx);
            SetParam(_skillParam);
        }

        private void HandleDamagedEvent(int damage)
        {
            if (_player.HealthCompo.IsDead) return;

            var attackCompo = _player.GetCompo<PlayerAttack>();
            if (attackCompo.IsAttacking || _player.IsUsingSkill) return;
            GetHitEvent?.Invoke(true);
            PlayHitAnimation(true);
        }

        private void HandleDeathEvent()
        {
            SetRootMotion(true);
            SetParam(_deadParam);
            var playerIK = _player.GetCompo<PlayerIK>();
            playerIK.LegsAnimatorCompo.enabled = false;
            playerIK.LeaningAnimator.enabled = false;
            playerIK.LookAnimatorCompo.enabled = false;
        }

        private void HandleAttackEvent(int comboCount)
        {
            float attackSpeed = _player.GetStat().attackSpeed[comboCount].GetValue();
            SetParam(_attackSpeedParam, attackSpeed);
            SetRootMotion(true);
            SetParam(_attackParam);
            SetParam(comboCountParam, comboCount);
        }

        private void HandleWeaponChangeStart(PlayerWeapon weapon)
        {
            int layerIdx = weapon.animationLayer;
            SwitchAnimationLayer(layerIdx);
        }

        private void HandleEvasion()
        {
            SetRootMotion(true);
            SetParam(_evasionParam);
        }

        public void HandleMovementEvent(Vector3 movement)
        {
            float dampTime = .1f;

            SetParam(_velocityParam, movement.sqrMagnitude, dampTime, Time.deltaTime);
        }

        private void SwitchAnimationLayer(int layerIdx)
        {
            for (int i = 0; i < AnimatorCompo.layerCount; i++)
            {
                AnimatorCompo.SetLayerWeight(i, 0);
            }

            _currentLayerIndex = layerIdx;
            AnimatorCompo.SetLayerWeight(layerIdx, 1);
        }
    }
}
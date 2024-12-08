using DG.Tweening;
using FMODUnity;
using MoreMountains.Feedbacks;
using PJH.Agent.Player;
using PJH.Combat;
using PJH.Core;
using UnityEngine;

namespace PJH.EquipmentSkillSystem
{
    public class SmiteRushSkill : EquipmentSkill
    {
        [SerializeField] private float _detectEnemyRange;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private int _power;
        [SerializeField] private float _enemyKnockBackPower;
        [SerializeField] private float _enemyKnockBackDuration;
        [SerializeField] private AnimationCurve _jumpToTargetAnimationCurve;
        [SerializeField] private EventReference _hammerRaiseEventReference;
        private bool _isUsingSkill;
        private MInterface.IEnemy _closetEnemy;
        private MMF_Player _feedback;

        private float _currentTime;

        public override void Init(Player player, Equipment.Equipment equipment)
        {
            base.Init(player, equipment);
            _feedback = transform.GetComponentInChildren<MMF_Player>();
        }

        public override void UseSkill(bool isHolding)
        {
            base.UseSkill(isHolding);
            _player.IsInvincibility = true;

            _isUsingSkill = true;
            var movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.CanMove = false;
            _player.GetCompo<PlayerIK>().LegsAnimatorCompo.User_FadeToDisabled(.1f);
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.HandleMovementEvent(Vector3.forward * 4);
            animatorCompo.EndUseSkillEvent += HandleEndUseSkillEvent;
        }

        public override bool AttemptUseSkill(bool isHolding = false)
        {
            if (_cooldownTimer <= 0 && skillEnabled)
            {
                _closetEnemy = FindClosestEnemy(_player.transform, _detectEnemyRange);
                if (_closetEnemy == null) return false;
            }

            return base.AttemptUseSkill(isHolding);
        }

        private void HandleEndUseSkillEvent()
        {
            var movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.CanMove = true;
            movementCompo.CharacterController.enabled = true;
            _player.IsInvincibility = false;


            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            _isUsingSkill = false;

            animatorCompo.EndUseSkillEvent -= HandleEndUseSkillEvent;
        }

        private void FixedUpdate()
        {
            if (_closetEnemy == null)
                return;
            var movementCompo = _player.GetCompo<PlayerMovement>();
            if (!_isUsingSkill || !movementCompo.CharacterController.enabled) return;

            Vector3 dir = (_closetEnemy.GameObject.transform.position - _player.transform.position);

            float distance = dir.sqrMagnitude;
            dir.Normalize();
            if (distance <= 13f)
            {
                movementCompo.CharacterController.enabled = false;
                RuntimeManager.PlayOneShot(_hammerRaiseEventReference, _player.transform.position);
                PlayAnimation();
                JumpToTarget(_closetEnemy.GameObject.transform);
                _closetEnemy = null;
            }

            if (!movementCompo.CharacterController.enabled) return;
            movementCompo.CharacterController
                .Move(dir * (_moveSpeed * Time.fixedDeltaTime));
            dir.y = 0;
            Quaternion look = Quaternion.LookRotation(dir);
            Quaternion rot = Quaternion.Slerp(_player.transform.rotation, look, 5f * Time.fixedDeltaTime);
            _player.transform.rotation = rot;
        }

        private void PlayAnimation()
        {
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.PlaySkillAnimation(0);
        }

        private void JumpToTarget(Transform target)
        {
            _closetEnemy = null;
            Vector3 startPos = _player.transform.position;
            Vector3 endPos = target.position;
            Vector3[] path = new Vector3[]
            {
                startPos,
                Vector3.Lerp(startPos, endPos, .5f),
                endPos
            };

            Sequence seq = DOTween.Sequence();
            seq.Append(
                _player.transform.DOPath(path, 1f, PathType.CatmullRom).SetEase(_jumpToTargetAnimationCurve));
            seq.AppendCallback(() =>
            {
                _player.GetCompo<PlayerIK>().LegsAnimatorCompo.User_FadeEnabled(.1f);
                _feedback?.PlayFeedbacks();
                var enemies = FindEnemiesInRange(_player.transform, 10);
                foreach (var enemy in enemies)
                {
                    Vector3 dir = -enemy.GameObject.transform.forward;
                    CombatData combatData = new();
                    combatData.knockBackPower = _enemyKnockBackPower;
                    combatData.knockBackDir = dir;
                    combatData.knockBackDuration = _enemyKnockBackDuration;
                    combatData.damage = _power;
                    enemy.GameObject.GetComponent<Health>().ApplyDamage(combatData);
                }
            });
        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreMountains.Feedbacks;
using PJH.EquipmentSkillSystem;
using Sirenix.OdinInspector;
using TrailsFX;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace PJH.Equipment.Weapon
{
    using Agent.Player;
    using Combat;
    using Core;

    public class Hammer : PlayerWeapon, MInterface.IMeleeWeapon, MInterface.IHandRIKEnableable,
        MInterface.IAttackChargeable
    {
        public Transform HandRIKTargetTrm { get; private set; }

        [field: SerializeField]
        [field: TabGroup("Detect")]
        public Vector3 DetectColliderSize { get; private set; }

        [field: SerializeField]
        [field: TabGroup("Detect")]
        public Vector3 DetectColliderPos { get; private set; }

        public TrailEffect TrailEffect { get; private set; }
        public MMF_Player HitFeedbackPlayer { get; private set; }


        private Transform _detectWeaponColliderPivotTrm;
        private Collider[] _detectColliders;
        private ParticleSystem _attackChargeEffect;
        private Sequence _attackChargeEffectSequence;

        private float _startAttackChargingTime;

        private int _powerfulAttackAdditionalDamage;
        private MMF_Player _groundSlamFeedback, _powerfulGroundSlamFeedback;
        private MMF_Player _powerfulHitFeedback;

        public override void Init(Agent.Agent owner)
        {
            base.Init(owner);
            HitFeedbackPlayer = transform.Find("Feedbacks/HitFeedback").GetComponent<MMF_Player>();

            Transform feedbacksTrm = transform.Find("Feedbacks");
            _groundSlamFeedback = feedbacksTrm.Find("GroundSlamFeedback")?.GetComponent<MMF_Player>();
            _powerfulGroundSlamFeedback = feedbacksTrm.Find("PowerfulGroundSlamFeedback")?.GetComponent<MMF_Player>();
            _powerfulHitFeedback = feedbacksTrm.Find("PowerfulHitFeedback").GetComponent<MMF_Player>();

            _attackChargeEffect = transform.Find("ChargeEffect").GetComponent<ParticleSystem>();
            HandRIKTargetTrm = transform.Find("HandRIKTarget");
            _detectWeaponColliderPivotTrm = _player.transform.Find("Model/DetectWeaponColliderPivot");
            _detectColliders = new Collider[3];
            TrailEffect = transform.Find("Model").GetComponent<TrailEffect>();
        }

        public override void Equip(Action CallBack = null)
        {
            base.Equip(CallBack);
            EnableMeleeTrail(false);
            EnableHandLIK(true);
            _player.GetCompo<PlayerIK>().SetHandRIKTarget(HandRIKTargetTrm);

            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.GroundSlamFeedbackEvent += HandlePlayGroundSlamFeedback;
            animatorCompo.PowerfulGroundSlamFeedbackEvent += HandlePlayPowerfulGroundSlamFeedback;
            animatorCompo.DetectWeaponColliderEvent += DetectWeaponCollider;
            animatorCompo.EnableMeleeTrailEvent += EnableMeleeTrail;
            _player.GetCompo<PlayerAttack>().ChargeAttackEvent += HandleChargeAttackEvent;
        }

        private async void HandlePlayGroundSlamFeedback()
        {
            var legsAnimatorCompo = _player.GetCompo<PlayerIK>().LegsAnimatorCompo;
            legsAnimatorCompo.enabled = false;
            await UniTask.WaitForEndOfFrame();
            _groundSlamFeedback?.PlayFeedbacks();
            legsAnimatorCompo.enabled = true;
        }

        private async void HandlePlayPowerfulGroundSlamFeedback()
        {
            var legsAnimatorCompo = _player.GetCompo<PlayerIK>().LegsAnimatorCompo;
            legsAnimatorCompo.enabled = false;
            await UniTask.WaitForEndOfFrame();
            _powerfulGroundSlamFeedback?.PlayFeedbacks();
            legsAnimatorCompo.enabled = true;
        }

        private void HandleChargeAttackEvent(bool isAttackCharging)
        {
            var passiveSkill = EquipmentSkillSystemCompo.GetSkill<HammerPassiveSkill>();
            var duration = passiveSkill.maxChargeAttackTime;
            var additionalPower = passiveSkill.additionalPower;
            if (_attackChargeEffectSequence != null && _attackChargeEffectSequence.IsActive())
                _attackChargeEffectSequence.Kill();
            if (isAttackCharging)
            {
                _attackChargeEffect.Play();
                _attackChargeEffectSequence = DOTween.Sequence();
                _attackChargeEffectSequence.Append(
                    _attackChargeEffect.transform.DOScale(Vector3.one, duration + 1));

                _startAttackChargingTime = Time.time;
            }
            else
            {
                _attackChargeEffect.Stop();
                _attackChargeEffect.transform.localScale = Vector3.one;

                float timeDiff = Time.time - _startAttackChargingTime;
                float lerpResult = Mathf.Min(1, timeDiff / duration);
                int currentPower = (int)_player.GetStat().power.GetDefaultValue();
                int finalPower = (int)Mathf.Lerp(currentPower, currentPower + additionalPower, lerpResult);
                _powerfulAttackAdditionalDamage = finalPower;
            }
        }

        public void EnableMeleeTrail(bool enabled)
        {
            if (_player.HealthCompo.IsDead) return;
            var isEvasion = _player.GetCompo<PlayerMovement>().IsEvasion;
            TrailEffect.active = !isEvasion && enabled;
        }

        public void DetectWeaponCollider()
        {
            _detectWeaponColliderPivotTrm.localPosition = DetectColliderPos;
            int cnt = Physics.OverlapBoxNonAlloc(_detectWeaponColliderPivotTrm.position, DetectColliderSize * .5f,
                _detectColliders, _player.transform.rotation,
                Define.MLayerMask.WhatIsEnemy);
            var playerAttackCompo = _player.GetCompo<PlayerAttack>();
            if (cnt > 0)
            {
                int power = (int)_player.GetStat().power.GetValue();
                for (int i = 0; i < cnt; i++)
                {
                    if (_detectColliders[i].transform.TryGetComponent(out MInterface.IDamageable damageable))
                    {
                        Vector3 hitPoint = _detectColliders[i].transform.position;


                        if (playerAttackCompo.IsPowerfulAttacking)
                        {
                            power = _powerfulAttackAdditionalDamage;
                            _powerfulHitFeedback?.PlayFeedbacks();
                        }
                        else
                        {
                            HitFeedbackPlayer?.PlayFeedbacks();
                        }

                        CombatData combatData = new CombatData()
                        {
                            hitPoint = hitPoint,
                            damage = power,
                            damageCategory = Define.EDamageCategory.Normal
                        };
                        damageable.ApplyDamage(combatData);
                    }
                }
            }
        }

        public void EnableHandLIK(bool enabled)
        {
            _player.GetCompo<PlayerIK>().EnableHandRIK(enabled);
        }

        public override void UnEquip(Action CallBack = null)
        {
            EnableHandLIK(false);

            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.PowerfulGroundSlamFeedbackEvent -= HandlePlayPowerfulGroundSlamFeedback;
            animatorCompo.GroundSlamFeedbackEvent -= HandlePlayGroundSlamFeedback;
            animatorCompo.DetectWeaponColliderEvent -= DetectWeaponCollider;
            animatorCompo.EnableMeleeTrailEvent -= EnableMeleeTrail;
            _player.GetCompo<PlayerAttack>().ChargeAttackEvent -= HandleChargeAttackEvent;
            base.UnEquip(CallBack);
        }

        public override bool UsePassiveSkill()
        {
            return EquipmentSkillSystemCompo.GetSkill<HammerPassiveSkill>().AttemptUseSkill();
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Transform playerTrm = transform.root.transform;

            if (!_detectWeaponColliderPivotTrm)
                _detectWeaponColliderPivotTrm = playerTrm.transform.Find("Model/DetectWeaponColliderPivot");

            Gizmos.matrix = playerTrm.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawCube(DetectColliderPos, DetectColliderSize * 2);
            Gizmos.color = Color.white;
        }
#endif
    }
}
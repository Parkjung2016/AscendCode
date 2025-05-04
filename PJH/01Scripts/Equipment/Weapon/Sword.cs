using System;
using MoreMountains.Feedbacks;
using PJH.Agent.Player;
using PJH.Combat;
using PJH.Core;
using PJH.EquipmentSkillSystem;
using Sirenix.OdinInspector;
using TrailsFX;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace PJH.Equipment.Weapon
{
    public class Sword : PlayerWeapon, MInterface.IMeleeWeapon
    {
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


        public override void Init(Agent.Agent owner)
        {
            base.Init(owner);
            HitFeedbackPlayer = transform.Find("Feedbacks/HitFeedback").GetComponent<MMF_Player>();
            _detectWeaponColliderPivotTrm = _player.transform.Find("Model/DetectWeaponColliderPivot");
            _detectColliders = new Collider[3];

            TrailEffect = transform.Find("Model").GetComponent<TrailEffect>();
        }

        public void EnableMeleeTrail(bool enabled)
        {
            if (_player.HealthCompo.IsDead) return;
            var isEvasion = _player.GetCompo<PlayerMovement>().IsEvasion;
            TrailEffect.active = !isEvasion && enabled;
        }

        public override void Equip(Action CallBack = null)
        {
            base.Equip(CallBack);
            EnableMeleeTrail(false);

            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.EnableMeleeTrailEvent += EnableMeleeTrail;
            animatorCompo.DetectWeaponColliderEvent += DetectWeaponCollider;
        }

        public override void UnEquip(Action CallBack = null)
        {
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.EnableMeleeTrailEvent -= EnableMeleeTrail;
            animatorCompo.DetectWeaponColliderEvent -= DetectWeaponCollider;

            base.UnEquip(CallBack);
        }

        public override bool UsePassiveSkill()
        {
            return EquipmentSkillSystemCompo.GetSkill<SwordPassiveSkill>().AttemptUseSkill();
        }

        public void DetectWeaponCollider()
        {
            _detectWeaponColliderPivotTrm.localPosition = DetectColliderPos;
            int cnt = Physics.OverlapBoxNonAlloc(_detectWeaponColliderPivotTrm.position, DetectColliderSize * 2,
                _detectColliders, _player.transform.rotation, Define.MLayerMask.WhatIsEnemy);
            if (cnt > 0)
            {
                int power = (int)_player.GetStat().power.GetValue();
                for (int i = 0; i < cnt; i++)
                {
                    if (_detectColliders[i].TryGetComponent(out MInterface.IDamageable health))
                    {
                        Vector3 hitPoint = _detectColliders[i].transform.position;
                        CombatData combatData = new CombatData()
                        {
                            hitPoint = hitPoint,
                            knockBackDir = _player.transform.forward,
                            knockBackDuration = _knockBackDuration,
                            knockBackPower = _knockBackPower,
                            damage = power,
                            damageCategory = Define.EDamageCategory.Normal
                        };
                        HitFeedbackPlayer?.PlayFeedbacks();
                        health.ApplyDamage(combatData);

                        foreach (var attribute in _attributes)
                        {
                            health.SetAilment(attribute.ailment, attribute.duration, attribute.damage);
                        }


                        UsePassiveSkill();
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Transform playerTrm = transform.root.transform;

            if (!_detectWeaponColliderPivotTrm)
                _detectWeaponColliderPivotTrm = playerTrm.transform.Find("Model/DetectWeaponColliderPivot");

            Gizmos.matrix = playerTrm.localToWorldMatrix;
            Gizmos.color = Color.red;
            _detectWeaponColliderPivotTrm.localPosition = DetectColliderPos;
            Gizmos.DrawCube(DetectColliderPos, DetectColliderSize*2);
            Gizmos.color = Color.white;
        }
#endif
    }
}
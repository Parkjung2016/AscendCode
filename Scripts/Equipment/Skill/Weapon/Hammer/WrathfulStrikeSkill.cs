using DG.Tweening;
using FMODUnity;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using PJH.Agent.Player;
using PJH.Combat;
using UnityEngine;

namespace PJH.EquipmentSkillSystem
{
    public class WrathfulStrikeSkill : EquipmentSkill
    {
        [SerializeField] private int _power;
        [SerializeField] private float _shockedTime;
        [SerializeField] private EventReference _wooshEventReference;
        private MMF_Player _feedback;

        public override void Init(Agent.Player.Player player, Equipment.Equipment equipment)
        {
            base.Init(player, equipment);
            _feedback = GetComponentInChildren<MMF_Player>();
            _feedback.GetFeedbackOfType<MMF_SpawnEffect>().spawnPosition = player.transform;
            player.GetCompo<PlayerAnimator>().PlayWrathfulStrikeSkillFeedbackEvent +=
                HandlePlayWrathfulStrikeSkillFeedback;
        }

        private void OnDestroy()
        {
            if (_player == null) return;
            if (_player.HealthCompo.IsDead) return;

            _player.GetCompo<PlayerAnimator>().PlayWrathfulStrikeSkillFeedbackEvent -=
                HandlePlayWrathfulStrikeSkillFeedback;
        }

        private void HandlePlayWrathfulStrikeSkillFeedback()
        {
            _feedback.PlayFeedbacks();

            var enemies = FindEnemiesInRange(_player.transform, 10);
            foreach (var iEnemy in enemies)
            {
                CombatData combatData = new();
                combatData.damage = _power;
                if (iEnemy is Enemy enemy)
                {
                    enemy.HealthCompo.ApplyDamage(combatData);
                    enemy.BehaviourTree.DisableBehavior();
                    enemy.MovementCompo.RigidbodyCompo.isKinematic = true;
                    enemy.NavAgent.enabled = false;
                }
            }

            DOVirtual.DelayedCall(_shockedTime, () =>
            {
                foreach (var iEnemy in enemies)
                {
                    if (iEnemy is Enemy enemy)
                    {
                        enemy.NavAgent.enabled = true;
                        enemy.MovementCompo.RigidbodyCompo.isKinematic = false;

                        enemy.BehaviourTree.EnableBehavior();
                    }
                }
            });
        }

        public override void UseSkill(bool isHolding)
        {
            base.UseSkill(isHolding);
            _player.IsInvincibility = true;

            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.SetRootMotion(true);
            animatorCompo.PlaySkillAnimation(2);
            RuntimeManager.PlayOneShot(_wooshEventReference, _player.transform.position);
            animatorCompo.EndUseSkillEvent += HandleEndUseSkillEvent;
        }

        private void HandleEndUseSkillEvent()
        {
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.SetRootMotion(false);
            _player.IsInvincibility = false;

            animatorCompo.EndUseSkillEvent -= HandleEndUseSkillEvent;
        }
    }
}
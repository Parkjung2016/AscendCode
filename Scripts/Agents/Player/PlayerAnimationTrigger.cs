using System;
using UnityEngine;


namespace PJH.Agent.Player
{
    public class PlayerAnimationTrigger : MonoBehaviour
    {
        private PlayerAnimator _animator;

        private void Awake()
        {
            _animator = GetComponent<PlayerAnimator>();
        }

        private void EndEvasion() => _animator.EndEvasion();

        private void EndCombo() => _animator.CallEndCombo();

        private void WeaponShoot() => _animator.CallWeaponShoot();

        private void DetectWeaponCollider() => _animator.CallDetectWeaponCollider();

        private void ComboPossible() => _animator.CallComboPossible();

        private void EnableBodyTrailEffect(int enabled) =>
            _animator.CallEnableBodyTrailEffect(Convert.ToBoolean(enabled));

        private void EnableMeleeTrail(int enabled) => _animator.CallEnableMeleeTrail(Convert.ToBoolean(enabled));


        private void PlayGroundSlamFeedback() => _animator.CallPlayGroundSlamFeedback();

        private void Footstep() => _animator.CallFootstep();
        private void EndUseSkill() => _animator.CallEndUseSkill();
        private void PlayWrathfulStrikeSkillFeedback() => _animator.CallPlayWrathfulStrikeSkillFeedback();
        private void EndGetHit() => _animator.CallEndGetHit();
        private void PlayLunarSlasEffect() => _animator.CallPlayLunarSlashEffect();
        private void PlayGroundImpactSound() => _animator.CallPlayGroundImpactSound();
        private void PlayWeaponAttackSound() => _animator.CallPlayWeaponAttackSound();
        private void PlayZoneOfAgonySkillEffect() => _animator.CallPlayZoneOfAgonySkillEffect();
        private void ShotVolleySkillArrow() => _animator.CallShotVolleySkillArrowEvent();
        private void ShotBarusSkillArrow() => _animator.CallShotBarusSkillArrow();
        private void ShotAusolSkillSlowAnim() => _animator.CallShotAusolSkillSlowAnim();
        private void ShotAusolSkillArrow() => _animator.CallShotAusolSkillArrow();
        private void PlayPhantomStrikeSlashSound() => _animator.CallPlayPhantomStrikeSlashSound();
    }
}
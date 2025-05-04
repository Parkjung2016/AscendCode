using System;
using Game.Events;
using MoreMountains.Feedbacks;
using PJH.Combat;
using TrailsFX;
using UnityEngine;

namespace PJH.Core
{
    public static class MInterface
    {
        public interface IAgentComponent
        {
            public void Initialize(Agent.Agent agent);
            public void AfterInit();
            public void Dispose();
        }

        public interface IDamageable
        {
            public void ApplyDamage(CombatData combatData);
            public void SetAilment(Ailment ailment, float duration, int damage);
        }

        public interface IMeleeWeapon
        {
            public Vector3 DetectColliderSize { get; }
            public Vector3 DetectColliderPos { get; }
            public TrailEffect TrailEffect { get; }
            public MMF_Player HitFeedbackPlayer { get; }

            public void EnableMeleeTrail(bool enabled);
            public void DetectWeaponCollider();
        }

        public interface IRangedWeapon
        {
            public void Shoot();
            public Transform ShootPointTrm { get; set; }

            public GameEventChannelSO SpawnEventChannel { get; set; }
        }

        [Serializable]
        public struct InteractInfo
        {
            public Sprite infoSprite;
        }

        public interface IInteractable
        {
            public InteractInfo InteractInfo { get; set; }
            public void Interact();
        }

        public interface IHandRIKEnableable
        {
            public Transform HandRIKTargetTrm { get; }
            public void EnableHandLIK(bool enabled);
        }

        public interface IAttackChargeable
        {
        }

        public interface IEnemy
        {
            public GameObject GameObject { get; }
        }
    }
}
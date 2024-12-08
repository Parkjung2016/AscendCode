using System.Collections.Generic;
using FMODUnity;
using MoreMountains.Feedbacks;
using PJH.Combat;
using PJH.Equipment.Weapon;
using PJH.Manager;
using UnityEngine;

namespace PJH.Core
{
    public class LunarSlashEffectPlayer : PoolEffectPlayer
    {
        [SerializeField] private Transform[] _changeableSizeTransforms;
        [SerializeField] private EventReference _slashEventReference;
        [SerializeField] private int _maxDetectCount = 15;
        [HideInInspector] public WeaponAttribute attribute;
        [HideInInspector] public int power;
        private ParticleSystem _ps;

        private List<ParticleSystem.Particle> _enterParticles = new();

        private MMF_Player _feedback;

        private void Awake()
        {
            _feedback = GetComponentInChildren<MMF_Player>();
            _ps = GetComponent<ParticleSystem>();
        }

        public void ChangeSize(float size)
        {
            Vector3 scale = Vector3.one * size;
            for (int i = 0; i < _changeableSizeTransforms.Length; i++)
            {
                _changeableSizeTransforms[i].transform.localScale = scale;
            }
        }

        public override void ResetItem()
        {
            base.ResetItem();
            MInterface.IEnemy[] enemies = EnemySpawnManager.Instance.EnemyList.ToArray();
            for (int i = 0; i < _ps.trigger.colliderCount; i++)
            {
                _ps.trigger.RemoveCollider(i);
            }

            foreach (var enemy in enemies)
            {
                _ps.trigger.AddCollider(enemy.GameObject.GetComponent<Collider>());
            }
        }

        public override void PlayEffects(Vector3 position, Quaternion rotation)
        {
            base.PlayEffects(position, rotation);
            RuntimeManager.PlayOneShot(_slashEventReference, position);
        }

        private void OnParticleTrigger()
        {
            int count = _ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, _enterParticles,
                out var enterData);

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < enterData.GetColliderCount(i); j++)
                {
                    var col = enterData.GetCollider(i, j);
                    if (col.TryGetComponent(out MInterface.IDamageable health))
                    {
                        CombatData combatData = new CombatData();
                        combatData.damage = power;
                        combatData.damageCategory = Define.EDamageCategory.Normal;
                        combatData.hitPoint = col.transform.position;
                        health.ApplyDamage(combatData);
                        health.SetAilment(attribute.ailment, attribute.duration, attribute.damage);

                        _feedback.PlayFeedbacks();
                    }
                }
            }
        }
    }
}
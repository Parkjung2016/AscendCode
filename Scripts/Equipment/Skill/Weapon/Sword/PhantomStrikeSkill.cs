using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using Game.Events;
using MoreMountains.Feedbacks;
using PJH.Agent.Player;
using PJH.Combat;
using PJH.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.EquipmentSkillSystem
{
    public class PhantomStrikeSkill : EquipmentSkill
    {
        [TabGroup("Info")] [SerializeField] private float _dashDistance;

        [TabGroup("Info")] [SerializeField] private float _dashDuration;

        [TabGroup("Info")] [SerializeField] private int _skillPower = 3;

        [TabGroup("Detect")] [SerializeField] private float _detectWidth;

        [TabGroup("Detect")] [SerializeField] private int _maxDetectCount = 3;

        [TabGroup("Effect")] [SerializeField] private GameEventChannelSO _dashEffectSpawnChannel;

        [TabGroup("Effect")] [SerializeField] private PoolTypeSO _skillEffectPoolType, _skillHitImpactEffectPoolType;

        [TabGroup("Sound")] [SerializeField] private EventReference _skillSoundEventReference;
        [TabGroup("Sound")] [SerializeField] private EventReference _impactSoundEventReference;
        private Collider[] _detectColliders;
        private MMF_Player _hitFeedback;

        private void Awake()
        {
            _hitFeedback = transform.Find("HitFeedback").GetComponent<MMF_Player>();
            _detectColliders = new Collider[_maxDetectCount];
        }

        private void Start()
        {
            _player.GetCompo<PlayerAnimator>().PlayPhantomStrikeSlashSoundEvent +=
                HandlePlayPhantomStrikeSlashSoundEvent;
        }

        private void HandlePlayPhantomStrikeSlashSoundEvent()
        {
            RuntimeManager.PlayOneShot(_skillSoundEventReference, _player.transform.position);
        }

        public override async void UseSkill(bool isHolding)
        {
            if (isHolding) return;
            base.UseSkill(isHolding);
            _player.IsInvincibility = true;
            _player.GetCompo<PlayerAnimator>().PlaySkillAnimation(0);
            var ikCompo = _player.GetCompo<PlayerIK>();
            ikCompo.LegsAnimatorCompo.enabled = false;
            await UniTask.WaitForEndOfFrame();
            var movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.CanMove = false;
            movementCompo.CharacterController.enabled = false;

            Vector3 worldMousePos = _player.PlayerInput.GetWorldMousePosition();
            Vector3 dir = worldMousePos - _player.transform.position;
            dir.y = 0;
            dir.Normalize();
            Vector3 startPlayerPos = _player.transform.position;
            Vector3 targetPos = startPlayerPos + dir * _dashDistance;
            Vector3 moveDir = targetPos - startPlayerPos;
            if (Physics.Raycast(startPlayerPos, moveDir.normalized, out RaycastHit hit, moveDir.magnitude,
                    Define.MLayerMask.WhatIsWall))
            {
                targetPos = hit.point;
            }

            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(.45f);
            seq.Append(_player.transform.DOMove(targetPos, _dashDuration));
            seq.Join(_player.transform.DOLookAt(worldMousePos, _dashDuration / 2f, AxisConstraint.Y).OnComplete(() =>
            {
                PlayEffect(_skillEffectPoolType, startPlayerPos, _player.transform.rotation);
                DetectEnemy(startPlayerPos);
            }));

            seq.AppendInterval(.3f);
            seq.AppendCallback(() =>
            {
                _player.GetCompo<PlayerMovement>().CanMove = true;
                movementCompo.CharacterController.enabled = true;
                _player.IsInvincibility = false;

                ikCompo.LegsAnimatorCompo.enabled = true;
            });
        }

        private void PlayEffect(PoolTypeSO effectType, Vector3 position, Quaternion rot)
        {
            var evt = SpawnEvents.EffectSpawn;
            evt.effectType = effectType;
            evt.position = position;
            evt.rotation = rot;
            _dashEffectSpawnChannel.RaiseEvent(evt);
        }

        private void OnDestroy()
        {
            _player.GetCompo<PlayerAnimator>().PlayPhantomStrikeSlashSoundEvent -=
                HandlePlayPhantomStrikeSlashSoundEvent;
        }

        private void DetectEnemy(Vector3 position)
        {
            Vector3 centerPos = position + _player.transform.forward * (_dashDistance / 2);
            int cnt = Physics.OverlapBoxNonAlloc(
                centerPos, new Vector3(_detectWidth, .6f, _dashDistance), _detectColliders,
                _player.transform.rotation, Define.MLayerMask.WhatIsEnemy);

            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    if (_detectColliders[i].TryGetComponent(out MInterface.IDamageable health))
                    {
                        Vector3 hitPoint = _detectColliders[i].ClosestPointOnBounds(transform.position);

                        CombatData combatData = new()
                        {
                            damageCategory = Define.EDamageCategory.Normal,
                            damage = _skillPower,
                            hitPoint = hitPoint
                        };
                        PlayEffect(_skillHitImpactEffectPoolType, hitPoint, Quaternion.Euler(-90, 0, 0));
                        RuntimeManager.PlayOneShot(_impactSoundEventReference, hitPoint);
                        _hitFeedback?.PlayFeedbacks();
                        health.ApplyDamage(combatData);
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Transform playerTrm = transform.root;

            Gizmos.color = Color.green;
            Gizmos.matrix = playerTrm.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.forward * (_dashDistance / 2), new Vector3(_detectWidth, .6f, _dashDistance));
            Gizmos.color = Color.white;
        }
#endif
    }
}
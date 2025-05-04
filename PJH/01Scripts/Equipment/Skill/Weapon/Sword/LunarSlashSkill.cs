using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Game.Events;
using PJH.Agent.Player;
using PJH.Equipment.Weapon;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace PJH.EquipmentSkillSystem
{
    public class LunarSlashSkill : EquipmentSkill
    {
        [SerializeField] private float _maxHoldTime;

        [SerializeField] private GameEventChannelSO _gameEventChannel;
        [SerializeField] private GameEventChannelSO _lunarSlashEffectSpawnChannel;
        [SerializeField, Range(.1f, .5f)] private float _minEffectSize = .5f;
        [SerializeField, Range(1f, 2f)] private float _maxEffectSize = 1;
        [SerializeField] private int _minPower;
        [SerializeField] private int _maxPower;
        [SerializeField] public WeaponAttribute _attribute;


        [SerializeField] private EventReference _slashEventReference, _chargingEventReference;

        private EventInstance _chargingEventInstance;
        private float _startHoldTime = -1;
        private Sequence _sequence;

        public override void Init(Agent.Player.Player player, Equipment.Equipment equipment)
        {
            base.Init(player, equipment);
            _player.GetCompo<PlayerAnimator>().PlayLunarSlashEffectEvent += HandlePlayLunarSlashEffectEvent;
        }

        private void OnDestroy()
        {
            if (_player.HealthCompo.IsDead) return;
            _player.GetCompo<PlayerAnimator>().PlayLunarSlashEffectEvent -= HandlePlayLunarSlashEffectEvent;
        }

        public override void UseSkill(bool isHolding)
        {
            base.UseSkill(isHolding);
            _player.IsInvincibility = true;

            if (isHolding)
            {
                _chargingEventInstance = RuntimeManager.CreateInstance(_chargingEventReference);
                _chargingEventInstance.set3DAttributes(_player.transform.To3DAttributes());
                _chargingEventInstance.start();
                _player.GetCompo<PlayerIK>().LookAnimatorCompo.SwitchLooking(false);
                var evt = GameEvents.ChargeAttack;
                evt.isAttackCharging = true;
                evt.maxChargeAttackTime = _maxHoldTime;
                _gameEventChannel.RaiseEvent(evt);

                _sequence = DOTween.Sequence();
                _sequence.Append(DOVirtual.DelayedCall(_maxHoldTime, () =>
                {
                    var evt = GameEvents.ChargeAttackReachMaxTime;
                    _gameEventChannel.RaiseEvent(evt);
                }));
                Vector3 worldMousePos = _player.PlayerInput.GetWorldMousePosition();

                _player.transform.DOLookAt(worldMousePos, .2f, AxisConstraint.Y);

                PlayAnimation();
                var animatorCompo = _player.GetCompo<PlayerAnimator>();

                _sequence.Join(DOTween.To(() => animatorCompo.AnimatorCompo.speed,
                    x => animatorCompo.AnimatorCompo.speed = x,
                    0, 1.4f));
                _startHoldTime = Time.time;
            }
            else
            {
                if (_sequence != null && _sequence.IsActive()) _sequence.Kill();
                var evt = GameEvents.ChargeAttack;
                evt.isAttackCharging = false;
                _gameEventChannel.RaiseEvent(evt);
                var animatorCompo = _player.GetCompo<PlayerAnimator>();

                animatorCompo.AnimatorCompo.speed = 1;

                if (_startHoldTime == -1)
                {
                    PlayAnimation();
                }
                else
                {
                    _chargingEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
                    _chargingEventInstance.release();
                }

                Vector3 worldMousePos = _player.PlayerInput.GetWorldMousePosition();
                _player.transform.DOLookAt(worldMousePos, .2f, AxisConstraint.Y);
                _player.GetCompo<PlayerIK>().LookAnimatorCompo.SwitchLooking(true);
            }
        }

        private void HandlePlayLunarSlashEffectEvent()
        {
            RuntimeManager.PlayOneShot(_slashEventReference, _player.transform.position);

            if (_startHoldTime == -1)
            {
                PlayEffect(_minEffectSize, _minPower);
            }
            else
            {
                float timeDiff = Time.time - _startHoldTime;
                float lerpResult = Mathf.Min(1, timeDiff / _maxHoldTime);
                float size = Mathf.Lerp(_minEffectSize, _maxEffectSize, lerpResult);
                int correctedPower = (int)Mathf.Lerp(_minPower, _maxPower, lerpResult);
                PlayEffect(size, correctedPower);
                _startHoldTime = -1;
            }

            _player.IsInvincibility = false;
        }

        private void PlayAnimation()
        {
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.PlaySkillAnimation(1);
            animatorCompo.SetRootMotion(true);
        }

        private void PlayEffect(float size, int power)
        {
            var evt = SpawnEvents.LunarSlashEffectSpawn;
            evt.position = _player.transform.position + Vector3.up;
            evt.rotation = _player.transform.rotation;
            evt.size = size;
            evt.power = power;
            evt.attribute = _attribute;
            _lunarSlashEffectSpawnChannel.RaiseEvent(evt);
        }
    }
}
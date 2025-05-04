using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using PJH.Agent.Player;
using PJH.Combat;
using PJH.Core;
using PJH.Manager;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace PJH.EquipmentSkillSystem
{
    public class ZoneOfAgonySkill : EquipmentSkill
    {
        private PoolManagerSO _poolManager;
        [SerializeField] private float _timeMaintained;
        [SerializeField] private float _damageTimeInterval;
        [SerializeField] private int _power;
        [SerializeField] private PoolTypeSO _skillEffectType;
        [SerializeField] private float _detectEnemyRange;
        [SerializeField] private EventReference _zoneSoundEventReference;
        private PoolEffectPlayer _effectPlayer;
        private EventInstance _zoneSoundEventInstance;
        private float _currentDamageTime;
        private bool _isUsingSkill;

        public override void Init(Agent.Player.Player player, Equipment.Equipment equipment)
        {
            base.Init(player, equipment);
            _poolManager = Managers.Addressable.Load<PoolManagerSO>("PoolManager");

            var animatorCompo = _player.GetCompo<PlayerAnimator>();

            animatorCompo.PlayZoneOfAgonySkillEffectEvent += HandlePlayZoneOfAgonySkillEffectEvent;
            transform.SetParent(_player.transform, false);
        }

        protected override void Update()
        {
            base.Update();
            if (!_isUsingSkill) return;

            if (_currentDamageTime <= _damageTimeInterval)
            {
                _currentDamageTime += Time.deltaTime;
            }
            else
            {
                _currentDamageTime = 0;
                var enemies = FindEnemiesInRange(_player.transform, _detectEnemyRange);

                foreach (var enemy in enemies)
                {
                    CombatData combatData = new CombatData();
                    combatData.damage = _power;
                    combatData.hitPoint = enemy.GameObject.transform.position;
                    combatData.damageCategory = Define.EDamageCategory.Normal;

                    enemy.GameObject.GetComponent<Health>().ApplyDamage(combatData);
                }
            }
        }

        private void OnDestroy()
        {
            if (_player == null) return;
            if (_player.HealthCompo.IsDead) return;

            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.PlayZoneOfAgonySkillEffectEvent -= HandlePlayZoneOfAgonySkillEffectEvent;
        }

        public override void UseSkill(bool isHolding)
        {
            base.UseSkill(isHolding);
            _isUsingSkill = false;
            _zoneSoundEventInstance = RuntimeManager.CreateInstance(_zoneSoundEventReference);
            RuntimeManager.AttachInstanceToGameObject(_zoneSoundEventInstance, _player.transform);
            _zoneSoundEventInstance.start();
            var movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.CanMove = false;
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.PlaySkillAnimation(1);
            animatorCompo.EndUseSkillEvent += HandleEndUseSkillEvent;
        }

        private async void HandlePlayZoneOfAgonySkillEffectEvent()
        {
            _isUsingSkill = true;

            _effectPlayer = (_poolManager.Pop(_skillEffectType) as PoolEffectPlayer);
            _effectPlayer.transform.SetParent(_player.transform);
            _effectPlayer.transform.localPosition = Vector3.up * .3f;
            _effectPlayer.PlayEffects();
            var cancellationToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitForSeconds(_timeMaintained, cancellationToken: cancellationToken);
            _zoneSoundEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            _zoneSoundEventInstance.release();
            _effectPlayer.StopEffects();
            _isUsingSkill = false;
        }

        private void HandleEndUseSkillEvent()
        {
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.EndUseSkillEvent -= HandleEndUseSkillEvent;
            var movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.CanMove = true;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var center = transform.root.transform.position;
            Gizmos.DrawWireSphere(center, _detectEnemyRange);
        }
#endif
    }
}
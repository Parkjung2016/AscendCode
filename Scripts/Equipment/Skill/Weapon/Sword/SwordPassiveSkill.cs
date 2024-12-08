using FMODUnity;
using PJH.Manager;
using UnityEngine;


namespace PJH.EquipmentSkillSystem
{
    public class SwordPassiveSkill : EquipmentSkill
    {
        [SerializeField] private int _increaseStamina;
        [SerializeField] private PoolTypeSO _recoverStaminaPoolType;
        [SerializeField] private EventReference _recoverStaminaEventReference;
        private PoolManagerSO _poolManager;

        public override void Init(Agent.Player.Player player, Equipment.Equipment equipment)
        {
            base.Init(player, equipment);
            _poolManager = Managers.Addressable.Load<PoolManagerSO>("PoolManager");
        }

        public override void UseSkill(bool isHolding)
        {
            if (_player.CurrentStamina >= _player.MaxStamina) return;
            _player.CurrentStamina += _increaseStamina;
            var effectPlayer = _poolManager.Pop(_recoverStaminaPoolType) as PoolEffectPlayer;
            RuntimeManager.PlayOneShot(_recoverStaminaEventReference, _player.transform.position);
            effectPlayer.PlayEffects(_player.transform.position, Quaternion.Euler(-90, 0, 0));
        }
    }
}
using Cysharp.Threading.Tasks;
using FMODUnity;
using PJH.Equipment.Weapon;
using UnityEngine;

namespace PJH.Agent.Player
{
    public class PlayerSound : AgentSound
    {
        [SerializeField] private EventReference _footstepSoundEventReference,
            _evasionSoundEventReference,
            _getSkillEventReference;

        private Player _player;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _player = agent as Player;
        }

        public override void AfterInit()
        {
            base.AfterInit();
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.FootstepEvent += HandleFootstepEvent;
            var movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.EvasionEvent += HandleEvasionEvent;
            var equipmentControllerCompo = _player.GetCompo<PlayerEquipmentController>();
            equipmentControllerCompo.WeaponChangeEvent += HandleWeaponChangeEvent;
        }

        private void HandleWeaponChangeEvent(PlayerWeapon weapon)
        {
            weapon.EquipmentSkillSystemCompo.GetSkillEvent += HandleGetSkillEvent;
        }

        private void HandleGetSkillEvent()
        {
            RuntimeManager.PlayOneShot(_getSkillEventReference, _player.transform.position);
        }

        private async void Start()
        {
            await UniTask.DelayFrame(2);
            var equipmentController = _player.GetCompo<PlayerEquipmentController>();
        }

        private void HandleEvasionEvent()
        {
            RuntimeManager.PlayOneShot(_evasionSoundEventReference, transform.position);
        }


        public void HandleFootstepEvent()
        {
            RuntimeManager.PlayOneShot(_footstepSoundEventReference, transform.position);
        }

        public override void Dispose()
        {
            base.Dispose();
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.FootstepEvent -= HandleFootstepEvent;
            var movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.EvasionEvent -= HandleEvasionEvent;
            var equipmentControllerCompo = _player.GetCompo<PlayerEquipmentController>();
            equipmentControllerCompo.WeaponChangeEvent -= HandleWeaponChangeEvent;
        }
    }
}
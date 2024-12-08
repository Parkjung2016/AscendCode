using System;
using Cysharp.Threading.Tasks;
using Game.Events;
using PJH.Agent.Player;
using PJH.Core;
using PJH.EquipmentSkillSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Equipment.Weapon
{
    public class Bow : PlayerWeapon, MInterface.IRangedWeapon
    {
        [field: SerializeField]
        [field: TabGroup("Spawn Info")]
        public PoolTypeSO ArrowPrefab { get; set; }

        [field: SerializeField]
        [field: TabGroup("Spawn Info")]
        public GameEventChannelSO SpawnEventChannel { get; set; }

        [SerializeField] [TabGroup("Spawn Info")]
        private float _arrowSpeed;

        public Transform ShootPointTrm { get; set; }

        public override void Init(Agent.Agent owner)
        {
            base.Init(owner);
            ShootPointTrm = transform.Find("ShootPoint");
        }

        public override void Equip(Action CallBack = null)
        {
            base.Equip(CallBack);
            _player.GetCompo<PlayerAnimator>().WeaponShootEvent += Shoot;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.B))
            {
                int point = 1;
                EquipmentSkillSystemCompo.SkillUpgrade<BowPassiveSkill>(ref point);
            }
        }

        public override void UnEquip(Action CallBack = null)
        {
            _player.GetCompo<PlayerAnimator>().WeaponShootEvent -= Shoot;
            base.UnEquip(CallBack);
        }

        public override bool UsePassiveSkill()
        {
            return EquipmentSkillSystemCompo.GetSkill<BowPassiveSkill>().AttemptUseSkill();
        }

        public async void Shoot()
        {
            var legsAnimatorCompo = _player.GetCompo<PlayerIK>().LegsAnimatorCompo;
            legsAnimatorCompo.enabled = false;
            await UniTask.WaitForEndOfFrame();
            var evt = SpawnEvents.PlayerArrowSpawn;
            evt.position = ShootPointTrm.position;
            evt.direction = _player.transform.forward;
            evt.speed = _arrowSpeed;
            evt.owner = this;
            evt.damage = (int)_player.GetStat().power.GetValue();
            SpawnEventChannel.RaiseEvent(evt);

            legsAnimatorCompo.enabled = true;
        }
    }
}
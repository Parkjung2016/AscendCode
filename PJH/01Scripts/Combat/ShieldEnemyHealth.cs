using System;
using PJH.Agent.Player;
using PJH.Equipment.Weapon;
using PJH.Manager;
using PJH.Scene;

namespace PJH.Combat
{
    public class ShieldEnemyHealth : Health
    {
        private ShieldEnemy _shieldEnemy;
        private Player _player;
        public event Action BlockedHitEvent;

        protected override void Awake()
        {
            base.Awake();
            _shieldEnemy = GetComponent<ShieldEnemy>();
            SetMaxHealth((int)_shieldEnemy.GetStat().MaxHealth.GetValue());
        }

        protected override void Start()
        {
            base.Start();
            _player = (Managers.Scene.CurrentScene as GameScene).Player;
        }

        public override void ApplyDamage(CombatData combatData)
        {
            bool isGuard = _shieldEnemy.Guard &&
                        _shieldEnemy.CalculateHitPointDir() == HitDir.Front;
            var playerWeapon = _player.GetCompo<PlayerEquipmentController>().GetWeapon();
            if (!isGuard || playerWeapon is Hammer)
            {
                base.ApplyDamage(combatData);
            }
            else
            {
                combatData.damage = 0;
                this.combatData = combatData;

                ShowPopupDamageText();
            }
        }
    }
}
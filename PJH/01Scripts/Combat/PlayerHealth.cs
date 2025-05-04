using PJH.Agent.Player;

namespace PJH.Combat
{
    public class PlayerHealth : Health
    {
        private Player _player;

        protected override void Awake()
        {
            base.Awake();
            _player = GetComponent<Player>();
        }

        public override void ApplyDamage(CombatData combatData)
        {
            if (_player.IsInvincibility) return;
            base.ApplyDamage(combatData);
        }
    }
}
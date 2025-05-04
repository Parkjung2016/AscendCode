using Game.Events;
using TrailsFX;
using UnityEngine;

namespace PJH.Agent.Player
{
    public class PlayerEffectPlayer : AgentEffectPlayer
    {
        [SerializeField] private GameEventChannelSO _effectSpawnEventChannel;
        private TrailEffect[] _trailEffects;
        private Player _player;


        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _player = agent as Player;

            _trailEffects = GetComponentsInChildren<TrailEffect>();
        }

        public override void AfterInit()
        {
            base.AfterInit();
            EnableTrialEffect(false);
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.EnableBodyTrailEffectEvent += EnableTrialEffect;
        }

        public override void Dispose()
        {
            base.Dispose();
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.EnableBodyTrailEffectEvent -= EnableTrialEffect;
        }

        public void EnableTrialEffect(bool enabled)
        {
            foreach (var trailEffect in _trailEffects)
            {
                trailEffect.active = enabled;
            }
        }
    }
}
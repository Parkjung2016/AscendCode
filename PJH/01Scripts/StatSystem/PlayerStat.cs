using System.Collections.Generic;
using UnityEngine;


namespace PJH.Stat
{
    [CreateAssetMenu(menuName = "SO/Stat/Player Stat", fileName = "Player Stat")]
    public class PlayerStat : AgentStat
    {
        public Stat moveSpeed;
        public Stat reComoboableTime;
        public Stat[] attackSpeed;
        public Stat[] skillSpeed;
        public Stat power;

        protected Dictionary<EPlayerStatType, Stat> _statDictionary;

        private void OnEnable()
        {
            RegisterStat(out _statDictionary);
        }

        public void AddModifier(EPlayerStatType type, int value)
        {
            _statDictionary[type].AddModifier(value);
        }

        public void RemoveModifier(EPlayerStatType type, int value)
        {
            _statDictionary[type].AddModifier(value);
        }
    }
}
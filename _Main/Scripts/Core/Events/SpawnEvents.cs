using PJH.Equipment.Weapon;
using UnityEngine;

namespace Game.Events
{
    public static class SpawnEvents
    {
        public static readonly PlayerArrowSpawn PlayerArrowSpawn = new();
        public static readonly EffectSpawn EffectSpawn = new();
        public static readonly LunarSlashEffectSpawn LunarSlashEffectSpawn = new();
    }


    public class PlayerArrowSpawn : GameEvent
    {
        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public int damage;
        public Bow owner;
    }

    public class EffectSpawn : GameEvent
    {
        public PoolTypeSO effectType;
        public Vector3 position;
        public Quaternion rotation;
    }

    public class LunarSlashEffectSpawn : GameEvent
    {
        public Vector3 position;
        public Quaternion rotation;
        public float size;
        public int power;
        public WeaponAttribute attribute;
    }
}
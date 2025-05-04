using PJH.Core;
using UnityEngine;

namespace PJH.Combat
{
    public struct CombatData
    {
        public Vector3 hitPoint;
        public int damage;
        public Vector3 knockBackDir;
        public float knockBackPower;
        public float knockBackDuration;
        public Define.EDamageCategory damageCategory;
    }
}
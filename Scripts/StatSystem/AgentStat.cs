using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PJH.Core;
using UnityEngine;


namespace PJH.Stat
{
    public class AgentStat : ScriptableObject
    {

        protected Agent.Agent _owner;

        public virtual void SetOwner(Agent.Agent owner)
        {
            _owner = owner;
        }

        protected void RegisterStat<T>(out Dictionary<T, Stat> statDictionary) where T : Enum
        {
            statDictionary = new Dictionary<T, Stat>();
            Type agentStatType = GetType();

            foreach (T statEnum in Enum.GetValues(typeof(T)))
            {
                try
                {
                    string fieldName = LowerFirstChar(statEnum.ToString());
                    FieldInfo statField = agentStatType.GetField(fieldName);
                    Stat statInstance = statField.GetValue(this) as Stat;

                    statDictionary.Add(statEnum, statInstance);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        private string LowerFirstChar(string input) => char.ToLower(input[0]) + input.Substring(1);

        public virtual void IncreaseStatFor(float modifyValue, float duration, Stat statToModify)
        {
            _owner.StartCoroutine(StatModifyCoroutine(modifyValue, duration, statToModify));
        }

        protected IEnumerator StatModifyCoroutine(float value, float duration, Stat statToModify)
        {
            statToModify.AddModifier(value);
            yield return YieldCache.WaitForSeconds(duration);
            statToModify.RemoveModifier(value);
        }

        // public int GetDamage()
        // {
        //     return damage.GetValue();
        // }
        //
        // public bool CanEvasion()
        // {
        //     return IsHitPercent(evasion.GetValue());
        // }

        // public int ArmoredDamage(int incomingDamage)
        // {
        //     return Mathf.Max(1, incomingDamage - armor.GetValue());
        // }

        // public bool IsCritical(ref int incomingDamage)
        // {
        //     if (IsHitPercent(criticalChance.GetValue()))
        //     {
        //         float percent = GetIntToPercent(criticalDamage.GetValue());
        //         incomingDamage = Mathf.CeilToInt(incomingDamage * percent);
        //         return true;
        //     }
        //
        //     return false;
        // }


        protected float GetIntToPercent(int statValue) => statValue * .0001f;
    }
}
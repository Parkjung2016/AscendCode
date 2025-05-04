using System;
using System.Collections.Generic;
using DamageNumbersPro;
using PJH.Core;
using UnityEngine;

namespace PJH.Manager
{
    public class PopupTextManager
    {
        private Dictionary<Define.EDamageCategory, DamageNumber> _damageTexts;

        public bool showablePopupText = true;

        public void Init()
        {
            _damageTexts = new();

            foreach (Define.EDamageCategory category in Enum.GetValues(typeof(Define.EDamageCategory)))
            {
                var damageText = Managers.Addressable.Load<DamageNumber>($"{category}DamageText");
                _damageTexts.Add(category, damageText);
            }
        }

        public void PopupDamageText(Vector3 position, int damage, Define.EDamageCategory damageCategory)
        {
            if (!showablePopupText) return;
            _damageTexts[damageCategory].Spawn(position, damage);
        }
    }
}
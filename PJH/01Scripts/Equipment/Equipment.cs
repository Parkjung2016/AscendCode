using System;
using PJH.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Equipment
{
    public abstract class Equipment : SerializedMonoBehaviour
    {
        protected PJH.Agent.Agent _owner;
        [EnumButtons] [BoxGroup("Info")] public Define.ESocketType socketType;
        public EquipmentSkillSystem EquipmentSkillSystemCompo { get; private set; }
        public bool isEquipped;

        public virtual void Init(PJH.Agent.Agent owner)
        {
            _owner = owner;
            EquipmentSkillSystemCompo = GetComponent<EquipmentSkillSystem>();
            EquipmentSkillSystemCompo.Init(owner as Agent.Player.Player, this);
        }

        public virtual void UnEquip(Action CallBack = null)
        {
            isEquipped = false;
            EquipmentSkillSystemCompo.UnEquip();
        }

        public virtual void Equip(Action CallBack = null)
        {
            isEquipped = true;
            EquipmentSkillSystemCompo.Equip();
        }
    }
}
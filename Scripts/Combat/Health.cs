using System;
using PJH.Agent.Player;
using PJH.Core;
using PJH.Manager;
using UnityEngine;

namespace PJH.Combat
{
    public class Health : MonoBehaviour, MInterface.IDamageable
    {
        public event Action<int> ApplyDamagedEvent;
        public event Action DeathEvent;
        public event Action<int> ChangedHealthEvent;

        [SerializeField] private int _maxHealth;
        [SerializeField] private Vector3 _popupTextOffset;
        [HideInInspector] public AilmentStat ailmentStat;
        public CombatData combatData;

        private int _currentHealth;

        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
                ChangedHealthEvent?.Invoke(_currentHealth);
                if (_currentHealth <= 0 && !IsDead)
                {
                    IsDead = true;
                    DeathEvent?.Invoke();
                }
            }
        }

        public int MaxHealth => _maxHealth;

        private MonoBehaviour _owner;
        public bool IsDead { get; set; }

        protected virtual void Awake()
        {
            ailmentStat = new AilmentStat();
            ailmentStat.DotDamageEvent += HandleDotDamageEvent;
        }

        protected virtual void Start()
        {
            CurrentHealth = _maxHealth;
        }

        public void SetMaxHealth(int maxHealth)
        {
            _maxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        private void OnDestroy()
        {
            ailmentStat.DotDamageEvent -= HandleDotDamageEvent;
        }

        private void HandleDotDamageEvent(Ailment ailmentType, int damage)
        {
            Vector3 position = transform.position + _popupTextOffset;
            if (_owner is not Player)
            {
                bool isShowPopupDamageText =
                    Convert.ToBoolean(
                        PlayerPrefs.GetInt(Baek.Define.Define.EBattleUIKey.ShowDamageTextToggle.ToString()));
                if (isShowPopupDamageText)
                    Managers.PopupText.PopupDamageText(position, damage, Define.EDamageCategory.Debuff_Red);
            }

            CurrentHealth -= damage;
        }

        public void SetAilment(Ailment ailment, float duration, int damage)
        {
            ailmentStat.ApplyAilments(ailment, duration, damage);
        }

        protected void Update()
        {
            if (IsDead) return;
            ailmentStat.UpdateAilment();
        }

        public void SetOwner(MonoBehaviour owner)
        {
            _owner = owner;
        }

        public virtual void ApplyDamage(CombatData combatData)
        {
            if (IsDead) return;

            CurrentHealth -= combatData.damage;
            this.combatData = combatData;
            ApplyDamagedEvent?.Invoke(combatData.damage);
            ShowPopupDamageText();
        }

        protected void ShowPopupDamageText()
        {
            Vector3 showPosition = combatData.hitPoint + Vector3.up * .6f;
            if (_owner is not Player)
            {
                bool isShowPopupDamageText =
                    Convert.ToBoolean(
                        PlayerPrefs.GetInt(Baek.Define.Define.EBattleUIKey.ShowDamageTextToggle.ToString()));
                if (isShowPopupDamageText)
                    Managers.PopupText.PopupDamageText(showPosition, combatData.damage,
                        combatData.damageCategory);
            }
        }
    }
}
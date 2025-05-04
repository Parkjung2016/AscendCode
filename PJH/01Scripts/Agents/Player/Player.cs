using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Events;
using PJH.Combat;
using UnityEngine;

namespace PJH.Agent.Player
{
    using Stat;
    using Input;

    public class Player : Agent
    {
        public bool IsInvincibility { get; set; }
        public bool IsHitting { get; private set; }
        public bool IsUsingSkill { get; set; }
        public string CurrentUsingSkillTypeName { get; set; }
        public event Action<int> CurrentStaminaChangedEvent;

        private int _currentStamina;

        public int CurrentStamina
        {
            get => _currentStamina;
            set
            {
                _currentStamina = Mathf.Clamp(value, 0, MaxStamina);
                CurrentStaminaChangedEvent?.Invoke(_currentStamina);
            }
        }

        [field: SerializeField] public int MaxStamina { get; private set; }
        [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }

        [SerializeField] private GameEventChannelSO _gameEventChannel;
        [SerializeField] private int _recoverHealthOnNextStage;

        private Transform _aimTarget;

        private CancellationTokenSource _knockBackSource;

        public bool CheckUsingSkill(string typeName)
        {
            if (!IsUsingSkill) return false;
            return CurrentUsingSkillTypeName != typeName;
        }

        protected override void Awake()
        {
            PlayerInput.EnablePlayerInput(true);
            base.Awake();
            _aimTarget = transform.Find("AimTarget");
        }

        protected override void AfterInit()
        {
            base.AfterInit();
            var movementCompo = GetCompo<PlayerMovement>();
            movementCompo.EvasionEvent += HandleEvasionEvent;

            HealthCompo.DeathEvent += HandleDeathEvent;
            HealthCompo.ChangedHealthEvent += HandleChangedHealthEvent;
            CurrentStamina = MaxStamina;

            var animatorCompo = GetCompo<PlayerAnimator>(true);
            animatorCompo.GetHitEvent += HandleGetHitEvent;
            animatorCompo.EndEvasionEvent += HandleEndEvasionEvent;
            animatorCompo.EndUseSkillEvent += HandleEndUseSkillEvent;
            _gameEventChannel.AddListener<ClearStage>(HandleClearStageEvent);
        }

        private void HandleClearStageEvent(ClearStage evt)
        {
            HealthCompo.CurrentHealth = HealthCompo.MaxHealth;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_knockBackSource != null)
            {
                _knockBackSource.Cancel();
                _knockBackSource.Dispose();
                _knockBackSource = null;
            }

            _knockBackSource = new CancellationTokenSource();

            if (HealthCompo.IsDead) return;
            var movementCompo = GetCompo<PlayerMovement>();
            movementCompo.EvasionEvent -= HandleEvasionEvent;

            HealthCompo.DeathEvent -= HandleDeathEvent;
            HealthCompo.ChangedHealthEvent -= HandleChangedHealthEvent;


            var animatorCompo = GetCompo<PlayerAnimator>(true);
            animatorCompo.GetHitEvent -= HandleGetHitEvent;
            animatorCompo.EndEvasionEvent -= HandleEndEvasionEvent;
            animatorCompo.EndUseSkillEvent -= HandleEndUseSkillEvent;
            _gameEventChannel.RemoveListener<ClearStage>(HandleClearStageEvent);
        }

        private void HandleEndEvasionEvent()
        {
            IsInvincibility = false;
        }

        private void HandleEvasionEvent()
        {
            IsInvincibility = true;
        }

        private void HandleEndUseSkillEvent()
        {
            IsUsingSkill = false;
        }


        private async void KnockBack(CombatData combatData)
        {
            float currentTime = 0;
            var movementCompo = GetCompo<PlayerMovement>();
            while (currentTime <= combatData.knockBackDuration)
            {
                currentTime += Time.deltaTime;
                Vector3 knockBackDir = combatData.knockBackDir;
                float knockBackPower = combatData.knockBackPower;
                movementCompo.CharacterController.Move(knockBackDir * (knockBackPower * Time.deltaTime));
            }

            try
            {
                await UniTask.Yield(cancellationToken: _knockBackSource.Token);
                movementCompo.CanMove = true;
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private void HandleGetHitEvent(bool isHitting)
        {
            IsHitting = isHitting;
            var movementCompo = GetCompo<PlayerMovement>();
            movementCompo.CanMove = false;
            if (_knockBackSource != null)
            {
                _knockBackSource.Cancel();
                _knockBackSource.Dispose();
                _knockBackSource = null;
            }

            _knockBackSource = new CancellationTokenSource();

            KnockBack(HealthCompo.combatData);
        }

        private void HandleChangedHealthEvent(int currentHealth)
        {
            if (currentHealth == 1)
            {
                var evt = GameEvents.RightBeforeThePlayerDeath;
                evt.isRightBeforeThePlayerDeath = true;
                _gameEventChannel.RaiseEvent(evt);
            }
            else
            {
                var evt = GameEvents.RightBeforeThePlayerDeath;
                if (evt.isRightBeforeThePlayerDeath)
                {
                    evt.isRightBeforeThePlayerDeath = false;
                    _gameEventChannel.RaiseEvent(evt);
                }
            }
        }

        private async void HandleDeathEvent()
        {
            PlayerInput.EnablePlayerInput(false);
            PlayerInput.EnableUIInput(false);
            var evt = GameEvents.PlayerDead;
            _gameEventChannel.RaiseEvent(evt);

            await UniTask.DelayFrame(2);
            foreach (var compo in _components.Values)
            {
                Destroy(compo as MonoBehaviour);
            }

            _components.Clear();
        }


        public void ChangeStat(PlayerStat changeStat)
        {
            var stat = Instantiate(changeStat);
            _agentStat = stat;
        }

        private void Update()
        {
            var worldMousePosition = PlayerInput.GetWorldMousePosition();
            worldMousePosition.y = _aimTarget.position.y;
            _aimTarget.position = worldMousePosition;
        }

        public new PlayerStat GetStat() => _agentStat as PlayerStat;
    }
}
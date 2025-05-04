using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using INab.Dissolve;
using Magio;
using PJH.Stat;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Equipment.Weapon
{
    using Agent.Player;

    [Serializable]
    public struct WeaponAttribute
    {
        public Ailment ailment;
        public float duration;
        public int damage;
    }

    public abstract class PlayerWeapon : Equipment
    {
        [BoxGroup("Info")] public int animationLayer;
        [BoxGroup("Info")] public int maxComboCount;
        [BoxGroup("Info")] [SerializeField] protected float _knockBackDuration;
        [BoxGroup("Info")] [SerializeField] protected float _knockBackPower;
        [BoxGroup("Info")] [SerializeField] private PlayerStat _playerStat;

        [field: BoxGroup("Sound")] [SerializeField]
        private EventReference _weaponAttackSoundEventReference;

        protected Player _player;
        public Player Player => _player;

        protected Dissolver _dissolverCompo;
        public Dissolver DissolverCompo => _dissolverCompo;

        protected List<WeaponAttribute> _attributes = new();

        private Dictionary<Ailment, MagioObjectEffect> _magioObjectEffects;

        private Collider _collider;
        private Rigidbody _rigidbody;


        private CancellationTokenSource _visibleWeaponSource;

        public override async void Init(PJH.Agent.Agent owner)
        {
            base.Init(owner);
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _rigidbody.useGravity = false;
            _dissolverCompo = GetComponent<Dissolver>();
            if (_dissolverCompo)
            {
                _dissolverCompo.materials.Clear();
                Transform model = transform.Find("Model");
                MeshRenderer meshRenderer = model.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = model.GetComponent<SkinnedMeshRenderer>();
                    _dissolverCompo.materials.AddRange(skinnedMeshRenderer.materials);
                }
                else
                    _dissolverCompo.materials.AddRange(meshRenderer.materials);
            }

            _player = owner as Player;
            await UniTask.WaitForFixedUpdate();
            MagioObjectMaster magioObjectMaster = GetComponent<MagioObjectMaster>();
            _magioObjectEffects = new();
            foreach (Ailment ailment in Enum.GetValues(typeof(Ailment)))
            {
                var effect = magioObjectMaster.magioObjects.Find(x => x.gameObject.name.Contains(ailment.ToString()));
                if (effect != null)
                    _magioObjectEffects.Add(ailment, effect);
            }
        }

        private void HandleDeathEvent()
        {
            transform.SetParent(null);
            _collider.isTrigger = false;
            _rigidbody.useGravity = true;
        }

        public override void Equip(Action CallBack = null)
        {
            if (_visibleWeaponSource != null)
            {
                _visibleWeaponSource.Cancel();
                _visibleWeaponSource.Dispose();
                _visibleWeaponSource = null;
            }

            _visibleWeaponSource = new CancellationTokenSource();
            _player.ChangeStat(_playerStat);

            ShowWeapon(CallBack);
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.WeaponAttackSoundEvent += HandleWeaponAttackSoundEvent;
            _player.HealthCompo.DeathEvent += HandleDeathEvent;

            base.Equip(CallBack);
        }

        private void HandleWeaponAttackSoundEvent()
        {
            RuntimeManager.PlayOneShot(_weaponAttackSoundEventReference, transform.position);
        }

        public override void UnEquip(Action CallBack = null)
        {
            if (_visibleWeaponSource != null)
            {
                _visibleWeaponSource.Cancel();
                _visibleWeaponSource.Dispose();
                _visibleWeaponSource = null;
            }

            _visibleWeaponSource = new CancellationTokenSource();

            HideWeapon(CallBack);
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.WeaponAttackSoundEvent -= HandleWeaponAttackSoundEvent;
            _player.HealthCompo.DeathEvent -= HandleDeathEvent;

            base.UnEquip(CallBack);
        }

        private async void ShowWeapon(Action CallBack = null)
        {
            if (!_dissolverCompo) return;
            _dissolverCompo.StopAllCoroutines();
            if (_dissolverCompo.currentState == Dissolver.DissolveState.Dissolved)
            {
                _dissolverCompo.currentState = Dissolver.DissolveState.Materialized;
            }

            _dissolverCompo.Materialize();
            try
            {
                await UniTask.WaitForSeconds(_dissolverCompo.duration + 3,
                    cancellationToken: _visibleWeaponSource.Token);
                if (!isEquipped)
                {
                    return;
                }

                CallBack?.Invoke();
            }
            catch (Exception e)
            {
            }
        }

        public virtual void AddAttribute(WeaponAttribute attribute)
        {
            if (_attributes.Contains(attribute)) return;
            _attributes.Add(attribute);
            SetWeaponEffectAura(attribute);
        }

        public virtual void RemoveAttribute(WeaponAttribute attribute)
        {
            _attributes.Remove(attribute);
        }

        private async void HideWeapon(Action CallBack = null)
        {
            if (!_dissolverCompo) return;
            _dissolverCompo.StopAllCoroutines();

            if (_dissolverCompo.currentState == Dissolver.DissolveState.Materialized)
            {
                _dissolverCompo.currentState = Dissolver.DissolveState.Dissolved;
            }

            for (int i = 0; i < _attributes.Count; i++)
            {
                RemoveAttribute(_attributes[i]);
            }

            _dissolverCompo.Dissolve();

            try
            {
                await UniTask.WaitForSeconds(_dissolverCompo.duration + 3,
                    cancellationToken: _visibleWeaponSource.Token);
                if (isEquipped)
                {
                    return;
                }

                CallBack?.Invoke();
            }
            catch (Exception e)
            {
            }
        }

        private void SetWeaponEffectAura(WeaponAttribute attribute)
        {
            var magioObject = _magioObjectEffects[attribute.ailment];
            magioObject.Setup();
            magioObject.fadeOutStart_s = attribute.duration;
            magioObject.TryToAnimateEffect(transform.position, 0);
        }

        private void OnDestroy()
        {
            if (_visibleWeaponSource != null)
            {
                _visibleWeaponSource.Cancel();
                _visibleWeaponSource.Dispose();
                _visibleWeaponSource = null;
            }
        }

        public abstract bool UsePassiveSkill();
    }
}
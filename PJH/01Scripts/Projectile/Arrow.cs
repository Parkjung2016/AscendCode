using Cysharp.Threading.Tasks;
using FMODUnity;
using INab.Dissolve;
using MoreMountains.Feedbacks;
using PJH.Combat;
using PJH.Core;
using PJH.Equipment.Weapon;
using PJH.EquipmentSkillSystem;
using UnityEngine;

namespace PJH.Projectile
{
    public class Arrow : MonoBehaviour, IPoolable
    {
        private static readonly int DissolveAmountHash = Shader.PropertyToID("_DissolveAmount");
        [SerializeField] private PoolTypeSO _poolType;
        [SerializeField] private float _dissovleTime;
        [SerializeField] private EventReference _arrowShootEventReference;
        public PoolTypeSO PoolType => _poolType;
        public GameObject GameObject => gameObject;
        private Pool _myPool;
        private Rigidbody _rigidbodyCompo;
        private bool _stuck;
        private int _damage;
        private Dissolver _dissolverCompo;
        private Transform _originParentTrm;

        public Bow owner;
        private int _currentPenetrationsCount = 0;
        public MMF_Player HitFeedbackPlayer { get; private set; }

        public void SetUpPool(Pool pool)
        {
            HitFeedbackPlayer = transform.Find("Feedbacks/HitFeedback").GetComponent<MMF_Player>();

            _rigidbodyCompo = GetComponent<Rigidbody>();
            _dissolverCompo = GetComponent<Dissolver>();
            Transform model = transform.Find("Model");
            MeshRenderer meshRenderer = model.GetComponent<MeshRenderer>();
            _dissolverCompo.materials.AddRange(meshRenderer.materials);
            _originParentTrm = transform.parent;
            _myPool = pool;
        }

        public void ResetItem()
        {
            _stuck = false;
            _currentPenetrationsCount = 0;
            _rigidbodyCompo.isKinematic = false;
            transform.SetParent(_originParentTrm);
        }


        private void FixedUpdate()
        {
            if (_myPool == null || _stuck) return;
            Quaternion look = Quaternion.LookRotation(_rigidbodyCompo.linearVelocity);
            transform.rotation = look;
        }

        public void Fire(Vector3 direction, float speed, int damage)
        {
            _rigidbodyCompo.linearVelocity = direction * speed;
            _damage = damage;
            RuntimeManager.PlayOneShot(_arrowShootEventReference, transform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out MInterface.IDamageable damageable))
            {
                if (_stuck) return;
                var damageCategory = Define.EDamageCategory.Normal;
                bool usedPassiveSkill = false;
                int damage = _damage;
                var skill = owner.EquipmentSkillSystemCompo.GetSkill<BowPassiveSkill>();
                if (skill.AttemptUseSkill())
                {
                    if (_currentPenetrationsCount >= 1)
                    {
                        damage = Mathf.RoundToInt(damage * (1 - skill.reduceDamagePercent * .01f));
                        damageCategory = Define.EDamageCategory.Debuff_Yellow;
                    }

                    if (_currentPenetrationsCount < skill.maxPenetrationsCount)
                    {
                        usedPassiveSkill = true;
                        _currentPenetrationsCount++;
                    }
                }

                HitFeedbackPlayer?.PlayFeedbacks();

                CombatData combatData = new()
                {
                    hitPoint = transform.position,
                    damage = damage,
                    damageCategory = damageCategory
                };
                damageable.ApplyDamage(combatData);

                if (usedPassiveSkill) return;
            }

            _stuck = true;
            _rigidbodyCompo.isKinematic = true;
            Dissolve();
        }

        private async void Dissolve()
        {
            await UniTask.WaitForSeconds(_dissovleTime);

            _dissolverCompo.Dissolve();
            await UniTask.WaitForSeconds(_dissolverCompo.duration + 2);
            transform.SetParent(_originParentTrm);

            foreach (var material in _dissolverCompo.materials)
            {
                material.SetFloat(DissolveAmountHash, 0);
            }

            _myPool.Push(this);
        }
    }
}
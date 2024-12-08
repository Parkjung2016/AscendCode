using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;


namespace PJH.Agent
{
    using Stat;
    using Core;

    public abstract class Agent : SerializedMonoBehaviour
    {
        public Combat.Health HealthCompo { get; private set; }
        protected Dictionary<Type, MInterface.IAgentComponent> _components;
        [SerializeField] protected AgentStat _agentStat;

        protected virtual void Awake()
        {
            HealthCompo = GetComponent<Combat.Health>();
            if (_agentStat != null)
                _agentStat = Instantiate(_agentStat);
            _components = new Dictionary<Type, MInterface.IAgentComponent>();
            AddComponentToDictionary();
            ComponentInitialize();
            AfterInit();
            HealthCompo.SetOwner(this);
        }


        private void AddComponentToDictionary()
        {
            GetComponentsInChildren<MInterface.IAgentComponent>(true)
                .ToList().ForEach(compo => _components.Add(compo.GetType(), compo));
        }

        private void ComponentInitialize()
        {
            _components.Values.ToList().ForEach(compo => compo.Initialize(this));
        }

        protected virtual void AfterInit()
        {
            _components.Values.ToList().ForEach(compo => compo.AfterInit());
        }

        public T GetCompo<T>(bool isDerived = false) where T : class
        {
            if (_components.TryGetValue(typeof(T), out MInterface.IAgentComponent compo))
            {
                return compo as T;
            }

            if (!isDerived) return default;

            Type findType = _components.Keys.FirstOrDefault(x => x.IsSubclassOf(typeof(T)));
            if (findType != null)
                return _components[findType] as T;

            return default(T);
        }

        public async UniTaskVoid StartDelayCallback(float time, Action Callback)
        {
            await UniTask.WaitForSeconds(time, cancellationToken: this.GetCancellationTokenOnDestroy());
            Callback();
        }


        protected virtual void OnDestroy()
        {
            if (_components.Count == 0) return;
            _components.Values.ToList().ForEach(compo => compo.Dispose());
        }

        public AgentStat GetStat() => _agentStat;
    }
}
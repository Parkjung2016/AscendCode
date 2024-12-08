using System;
using Magio;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace PJH.Agent
{
    using Core;

    public class AgentEffectPlayer : MonoBehaviour, MInterface.IAgentComponent
    {
        private readonly int hdrColorHash = Shader.PropertyToID("_EmissionColor");
        private Agent _agent;

        private MMF_Player _applyDamageFeedback;
        protected MagioObjectMaster _magioObjectMaster;

        public virtual void Initialize(Agent agent)
        {
            _magioObjectMaster = GetComponentInChildren<MagioObjectMaster>();
            _agent = agent;
            Transform feedbacksTrm = transform.Find("Feedbacks");
            _applyDamageFeedback = feedbacksTrm.Find("ApplyDamageFeedback")?.GetComponent<MMF_Player>();
        }

        private void Start()
        {
            _agent.HealthCompo.ailmentStat.AilmentChangeEvent += HandleAilmentChangeEvent;
        }

        private async void HandleAilmentChangeEvent(Ailment newAilment, float duration)
        {
            Ailment[] values = (Ailment[])Enum.GetValues(typeof(Ailment));
            foreach (Ailment flag in values)
            {
                if (newAilment.HasFlag(flag) && flag != Ailment.None)
                {
                    int idx = MUtil.GetEnumFlagIndex(flag) - 1;
                    var magioObject = _magioObjectMaster.magioObjects[idx];
                    magioObject.fadeOutStart_s = duration;
                    magioObject.fadeOutLength_s = 1;
                    magioObject.TryToAnimateEffect(transform.position, 0);
                }
            }
        }

        private void HandleApplyDamagedEvent(int damage)
        {
            if (_applyDamageFeedback == null) return;

            _applyDamageFeedback.PlayFeedbacks();
        }

        public virtual void AfterInit()
        {
            _agent.HealthCompo.ApplyDamagedEvent += HandleApplyDamagedEvent;
        }

        public virtual void Dispose()
        {
            _agent.HealthCompo.ApplyDamagedEvent -= HandleApplyDamagedEvent;
            _agent.HealthCompo.ailmentStat.AilmentChangeEvent -= HandleAilmentChangeEvent;
        }
    }
}
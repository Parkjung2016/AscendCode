using System;
using PJH.Agent.Animation;
using UnityEngine;

namespace PJH.Agent
{
    using Core;

    [RequireComponent(typeof(Animator))]
    public abstract class AgentAnimator : MonoBehaviour, MInterface.IAgentComponent
    {
        private static readonly int RootMotionMultiplierHash = Animator.StringToHash("RootMotionMultiplier");

        public bool IsRootMotion { get; private set; }
        public event Action<Vector3> AnimatorMoveEvent;

        public Animator AnimatorCompo { get; private set; }
        private Agent _agent;

        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
            AnimatorCompo = GetComponent<Animator>();
        }

        public virtual void AfterInit()
        {
        }

        public virtual void Dispose()
        {
        }

        public void SetParam(AnimParamSO param, bool value)
        {
            AnimatorCompo.SetBool(param.hashValue, value);
        }

        public void SetParam(AnimParamSO param, float value)
        {
            AnimatorCompo.SetFloat(param.hashValue, value);
        }

        public void SetParam(AnimParamSO param, float value, float damp, float deltaTime)
        {
            AnimatorCompo.SetFloat(param.hashValue, value, damp, deltaTime);
        }

        public void SetParam(AnimParamSO param, int value)
        {
            AnimatorCompo.SetInteger(param.hashValue, value);
        }

        public void SetParam(AnimParamSO param)
        {
            AnimatorCompo.SetTrigger(param.hashValue);
        }

        public void SetRootMotion(bool isRootMotion)
        {
            IsRootMotion = isRootMotion;
        }

        private void OnAnimatorMove()
        {
            if (!IsRootMotion) return;
            var multiplier = AnimatorCompo.GetFloat(RootMotionMultiplierHash);
            Vector3 deltaPosition = AnimatorCompo.deltaPosition * multiplier;
            AnimatorMoveEvent?.Invoke(deltaPosition);
        }
    }
}
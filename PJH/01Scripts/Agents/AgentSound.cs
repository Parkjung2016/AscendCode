using FMOD.Studio;
using FMODUnity;
using PJH.Core;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace PJH.Agent
{
    public class AgentSound : MonoBehaviour, MInterface.IAgentComponent
    {
        private Agent _agent;

        [SerializeField] private EventReference _ignitedEventReference;
        [SerializeField] private EventReference _getHitEventReference;
        private EventInstance _ignitedEventInstance;

        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
        }

        public virtual void AfterInit()
        {
            _agent.HealthCompo.ailmentStat.AilmentChangeEvent += HandleAilmentChangeEvent;
            _agent.HealthCompo.ApplyDamagedEvent += HandleApplyDamagedEvent;
            _agent.HealthCompo.ailmentStat.DotDamageEvent += HandleDotDamageEvent;
        }

        private void HandleDotDamageEvent(Ailment ailmenttype, int damage)
        {
            RuntimeManager.PlayOneShot(_getHitEventReference, transform.position);
        }

        private void HandleApplyDamagedEvent(int damage)
        {
            RuntimeManager.PlayOneShot(_getHitEventReference, transform.position);
        }

        private void HandleAilmentChangeEvent(Ailment newailment, float duration)
        {
            if (newailment == Ailment.None)
            {
                _ignitedEventInstance.stop(STOP_MODE.IMMEDIATE);
                _ignitedEventInstance.release();
            }
            else
            {
                _ignitedEventInstance = RuntimeManager.CreateInstance(_ignitedEventReference);
                _ignitedEventInstance.set3DAttributes(transform.position.To3DAttributes());
                RuntimeManager.AttachInstanceToGameObject(_ignitedEventInstance, gameObject,
                    gameObject.GetComponent<Rigidbody>());
                _ignitedEventInstance.start();
            }
        }

        public virtual void Dispose()
        {
            _ignitedEventInstance.release();
        }
    }
}
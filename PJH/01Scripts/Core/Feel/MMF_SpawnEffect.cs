using Game.Events;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback shows effect.")]
    [FeedbackPath("Effect/SpawnEffect")]
    [MovedFrom(false, null, "MoreMountains.Feedbacks.URP")]
    public class MMF_SpawnEffect : MMF_Feedback
    {
        [MMFInspectorGroup("Info", true, 41)] public PoolTypeSO effectPoolType;
        public Transform spawnPosition;
        public Vector3 additionalSpawnPosition;
        public Quaternion spawnRotation;
        public GameEventChannelSO spawnEventChannel;

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            var evt = SpawnEvents.EffectSpawn;
            evt.effectType = effectPoolType;
            evt.position = spawnPosition.position + additionalSpawnPosition;
            evt.rotation = spawnRotation;
            spawnEventChannel.RaiseEvent(evt);
        }
    }
}
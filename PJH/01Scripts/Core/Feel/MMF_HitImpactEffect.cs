using Game.Events;
using MoreMountains.Feedbacks;
using PJH.Combat;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[AddComponentMenu("")]
[FeedbackHelp(
    "This feedback shows the hit impact effect.")]
[FeedbackPath("Effect/Hit Impact Effect")]
[MovedFrom(false, null, "MoreMountains.Feedbacks.URP")]
public class MMF_HitImpactEffect : MMF_Feedback
{
    [MMFInspectorGroup("Info", true, 41)] public Health healthCompo;

    public GameEventChannelSO spawnEventChannel;
    public PoolTypeSO hitImpactEffectPoolType;

    protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
    {
        CombatData combatData = healthCompo.combatData;
        Vector3 spawnPosition = combatData.hitPoint + Vector3.up * .6f;

        var evt = SpawnEvents.EffectSpawn;
        evt.effectType = hitImpactEffectPoolType;
        evt.position = spawnPosition;
        evt.rotation = Quaternion.Euler(-90, 0, 0);
        spawnEventChannel.RaiseEvent(evt);
    }
}
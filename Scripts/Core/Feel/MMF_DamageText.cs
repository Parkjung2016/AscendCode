using MoreMountains.Feedbacks;
using PJH.Combat;
using PJH.Manager;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback displays the damage text")]
    [FeedbackPath("TextMesh Pro/TMP Damage Text")]
    [MovedFrom(false, null, "MoreMountains.Feedbacks.URP")]
    public class MMF_DamageText : MMF_Feedback
    {
        [MMFInspectorGroup("Info", true, 41)] public PJH.Combat.Health healthCompo;


        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            CombatData combatData = healthCompo.combatData;
            Vector3 showPosition = combatData.hitPoint + Vector3.up * .6f;
            Managers.PopupText.PopupDamageText(showPosition, combatData.damage,
                combatData.damageCategory);
        }
    }
}
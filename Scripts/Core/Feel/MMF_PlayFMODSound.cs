using FMODUnity;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[AddComponentMenu("")]
[FeedbackHelp(
    "This feedback play fmod sound")]
[FeedbackPath("Sound/FMOD")]
[MovedFrom(false, null, "MoreMountains.Feedbacks.URP")]
public class MMF_PlayFMODSound : MMF_Feedback
{
    [MMFInspectorGroup("Info", true, 41)] public EventReference eventReference;
    [MMFInspectorGroup("Info", true, 41)] public bool is3dSound = true;


    protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
    {
        RuntimeManager.PlayOneShot(eventReference, !is3dSound ? default : Owner.transform.position);
    }
}
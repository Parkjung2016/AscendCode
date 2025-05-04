using UnityEngine;

namespace PJH.Agent.Animation
{
    public enum ParamType
    {
        Boolean,
        Float,
        Trigger,
        Integer
    }

    [CreateAssetMenu(menuName = "SO/Animation/Param")]
    public class AnimParamSO : ScriptableObject
    {
        public ParamType paramType;
        public string paramName;
        public int hashValue;

        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(paramName))
            {
                hashValue = Animator.StringToHash(paramName);
            }
        }
    }
}
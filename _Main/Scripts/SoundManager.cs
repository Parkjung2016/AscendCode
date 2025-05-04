using FMODUnity;
using UnityEngine;

namespace Game.Core
{
    public class SoundManager : LJS.MonoSingleton<SoundManager>
    {
        [SerializeField] private EventReference _groundImpactEventReference;

        public void PlayGroundImpactSound(Vector3 position)
        {
            RuntimeManager.PlayOneShot(_groundImpactEventReference, position);
        }
    }
}
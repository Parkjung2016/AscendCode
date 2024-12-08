using DG.Tweening;
using FMODUnity;
using Game.Events;
using UnityEngine;

namespace PJH.UI
{
    public class DeathUI : MonoBehaviour
    {
        [SerializeField] private EventReference _deathEventReference;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            GameEvents.PlayerDead.FadeEndEvent = HandleFadeEndEvent;
        }


        private void HandleFadeEndEvent()
        {
            RuntimeManager.PlayOneShot(_deathEventReference);
            DOVirtual.DelayedCall(.5f, () => _canvasGroup.DOFade(1, 2));
        }
    }
}
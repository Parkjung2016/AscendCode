using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace PJH.UI
{
    public class FadeUI : LJS.MonoSingleton<FadeUI>
    {
        private CanvasGroup _canvasGroup;

        private TextMeshProUGUI _thankYouTMP;

        private void Awake()
        {
            _thankYouTMP = transform.Find("Thank you").GetComponent<TextMeshProUGUI>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            _thankYouTMP.DOFade(0, 0);
            Instance.gameObject.SetActive(false);
        }

        public static void FadeIn(float duration = .5f, Action CallBack = null)
        {
            Instance.gameObject.SetActive(true);

            Instance._canvasGroup.DOFade(1, duration).OnComplete(() => CallBack?.Invoke());
        }

        public static void FadeOut(float duration = .5f, Action CallBack = null)
        {
            Instance._canvasGroup.DOFade(0, duration).OnComplete(() =>
            {
                Instance.gameObject.SetActive(false);
                CallBack?.Invoke();
            });
        }

        public void ShowThankYou()
        {
            _thankYouTMP.DOFade(1, 1);
        }

        public void HideThankYou()
        {
            _thankYouTMP.DOFade(0, 1);
        }
    }
}
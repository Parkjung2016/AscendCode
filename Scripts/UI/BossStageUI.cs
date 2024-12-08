using DamageNumbersPro;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace PJH.UI
{
    public class BossStageUI : MonoBehaviour
    {
        private Image _bossImage;
        [SerializeField] private DamageNumber _damageNumber;
        [SerializeField] private EventReference _bossStageEventReference;

        private RectTransform _bossMaskRectTransform;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _bossMaskRectTransform = transform.Find("Mask").GetComponent<RectTransform>();
            _bossImage = _bossMaskRectTransform.Find("Boss Image").GetComponent<Image>();
            float x = _bossMaskRectTransform.sizeDelta.x;

            _bossMaskRectTransform.sizeDelta = new Vector2(x, 0);
        }

        public void ShowStageUI(Sprite bossImage, string bossName)
        {
            RuntimeManager.PlayOneShot(_bossStageEventReference);

            _bossImage.sprite = bossImage;
            _damageNumber.leftText = bossName;
            _damageNumber.SpawnGUI(_rectTransform, new Vector2(355, -140), bossName);
            float x = _bossMaskRectTransform.sizeDelta.x;

            _bossMaskRectTransform.sizeDelta = new Vector2(x, 0);
            Sequence seq = DOTween.Sequence();
            seq.Append(_bossMaskRectTransform.DOSizeDelta(new Vector2(x, 700), .3f));
            seq.AppendInterval(2f);
            seq.Append(_bossMaskRectTransform.DOSizeDelta(new Vector2(x, 0), .3f));
        }
    }
}
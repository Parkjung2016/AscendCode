using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PJH.Core
{
    public class DecalProjectorPool : MonoBehaviour
    {
        private DecalProjector _decalProjector;

        private void Awake()
        {
            _decalProjector = GetComponent<DecalProjector>();
        }

        public void ShowDecal()
        {
            _decalProjector.fadeFactor = 1;
        }

        public void HideDecal()
        {
            DOVirtual.Float(1, 0, .5f, x => _decalProjector.fadeFactor = x).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}
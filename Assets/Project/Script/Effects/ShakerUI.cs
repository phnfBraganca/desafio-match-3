using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gazeus.DesafioMatch3
{
    public class ShakerUI : MonoBehaviour
    {

        [SerializeField]
        private ComboEffect _comboEffect;
        [SerializeField]
        private RectTransform _rectTransform;

        private Tween _uiTween;

        public float ShakeDuration = 1;
        public float ShakeStrength = 15f;

        private void Start()
        {
            _uiTween = _rectTransform.DOShakePosition(ShakeDuration, ShakeStrength).SetLoops(-1).SetEase(Ease.InOutSine).Pause().SetAutoKill(false);
        }

        private void Update()
        {
            DoShakeUI();
        }

        private void DoShakeUI()
        {
            if(_comboEffect.GetComboCount() > 1)
            {
                _uiTween.Play();
            }
            else
            {
                _uiTween.Rewind();
            }
        }
    }
}

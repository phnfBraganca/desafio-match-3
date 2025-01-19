using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3
{
    public class TintImageUI : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Material _material;

        private Material _materialInstance;

        private Tween _tweenMaterial;

        public float DissolveSpeed = 1f;
        public float DissolveAmount = -2f;
        public string DissolveStringProperty;

        // Start is called before the first frame update
        void Start()
        {
            _materialInstance = new Material(_material);
            _image.material = _materialInstance;

            _materialInstance.color = _image.color;

            _tweenMaterial = _materialInstance.DOFloat(DissolveAmount, DissolveStringProperty, DissolveSpeed).Pause();
        }

        private void Update()
        {
            if (_materialInstance.color == _image.color)
            {
                return;
            }
            _materialInstance.color = _image.color;
        }

        public void DissolveImage()
        {
            _tweenMaterial.Play().OnComplete(DestruirObjecto);
        }

        private void DestruirObjecto()
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gazeus.DesafioMatch3
{
    public class ComboEffect : MonoBehaviour
    {
        [SerializeField]
        private float _comboDuration = 2f;
        private float _timer;
        private bool _comboActive = false;
        private int _comboCount = 0;

        // Update is called once per frame
        void Update()
        {
            CheckCombo();
        }

        private void CheckCombo()
        {
            if (_comboActive)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0f)
                {
                    EndCombo();
                }
            }
        }

        public void AddCombo()
        {
            if (!_comboActive)
            {
                StartCombo();
            }
            else
            {
                _timer = _comboDuration;
                _comboCount++;
                //Debug.Log($" Combo x{_comboCount}!");
            }
        }

        private void StartCombo()
        {
            _comboActive = true;
            _comboCount = 1;
            _timer = _comboDuration;
            //Debug.Log($" Combo começou!");
        }

        private void EndCombo()
        {
            _comboActive = false;
            _comboCount = 0;
            //Debug.Log($" Combo terminou!");
        }

        public int GetComboCount()
        {
            return _comboCount;
        }
    }
}

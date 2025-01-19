using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gazeus.DesafioMatch3
{
    public class PointsMatch : MonoBehaviour
    {
        private int _points;

        public TMP_Text PointText;
        public TMP_Text PointTextBG;

        private void Start()
        {
            _points = 0;
        }

        private void Update()
        {
            SetScore();
        }

        private void SetScore()
        {
            PointText.text = _points.ToString();
            PointTextBG.text = _points.ToString();
        }

        public void AddPoints(int points)
        {
            _points += points;
        }
    }
}

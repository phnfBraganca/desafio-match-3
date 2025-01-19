using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField]
        private string SceneName;
        [SerializeField]
        private Image FadeImage;
        [SerializeField]
        private float FadeDuration;

        public void LoadGameScene()
        {
            StartCoroutine(FadeAndLoad());
        }

        private IEnumerator FadeAndLoad()
        {
            for (float t = 0; t < FadeDuration; t += Time.deltaTime)
            {
                float alpha = t / FadeDuration;
                FadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            SceneManager.LoadScene(SceneName);

            for (float t = 0; t < FadeDuration; t += Time.deltaTime)
            {
                float alpha = 1 - (t / FadeDuration);
                FadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
        }
        
    }
}

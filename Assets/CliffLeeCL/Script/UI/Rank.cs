using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CliffLeeCL {
    public class Rank : MonoBehaviour {
        public Text rankText;
        public int maxScore = 1000000;

        // Use this for initialization
        void Start() {
            EventManager.Instance.onGameOver += OnGameOver;
        }

        void OnDisable()
        {
            EventManager.Instance.onGameOver -= OnGameOver;
        }

        void OnGameOver()
        {
            int currentScore = ScoreManager.Instance.CurrentScore;

            if(currentScore > maxScore * 0.8f)
            {
                rankText.text = "S";
            }
            else if (currentScore > maxScore * 0.5f)
            {
                rankText.text = "A";
            }
            else if (currentScore > maxScore * 0.2f)
            {
                rankText.text = "B";
            }
            else
            {
                rankText.text = "C";
            }
        }
    }
}

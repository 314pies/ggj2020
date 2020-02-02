using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace CliffLeeCL
{
    /// <summary>
    /// The class controls how game over menu works.
    /// </summary>
    public class GameOverMenu : MonoBehaviour
    {
        /// <summary>
        /// Objects that should be set active when UI is opened.
        /// </summary>
        public GameObject[] objectsToOpen;
        /// <summary>
        /// Objects that should be set inactive when UI is opened.
        /// </summary>
        public GameObject[] objectsToClose;

        public TextMeshProUGUI gameOverTitle;
        public string leftWinText = "Left Wins!";
        public string rightWinText = "Right Wins!";

        /// <summary>
        /// Start is called once on the frame when a script is enabled.
        /// </summary>
        void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            Time.timeScale = 1.0f;
            //EventManager.Instance.onGameOver += OnGameOver;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        void OnDisable()
        {
            //EventManager.Instance.onGameOver -= OnGameOver;
        }

        /// <summary>
        /// Event listener that listen to EventManager's onGameOver event.
        /// </summary>
        /// <seealso cref="EventManager.onGameOver"/>
        public void OnGameOver(bool isLeftWin)
        {
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OpenGameOverUI();
            SetGameOverTitle(isLeftWin);
        }

        /// <summary>
        /// The function will set up and open game over UI.
        /// </summary>
        void OpenGameOverUI()
        {
            foreach (GameObject obj in objectsToOpen)
                obj.SetActive(true);
            foreach (GameObject obj in objectsToClose)
                obj.SetActive(false);
        }

        void SetGameOverTitle(bool isLeftWin)
        {
            if (isLeftWin)
            {
                gameOverTitle.text = leftWinText;
            }
            else
            {
                gameOverTitle.text = rightWinText;
            }
        }
    }
}

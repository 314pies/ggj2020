using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

        /// <summary>
        /// Start is called once on the frame when a script is enabled.
        /// </summary>
        void Start()
        {
            EventManager.Instance.onGameOver += OnGameOver;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        void OnDisable()
        {
            EventManager.Instance.onGameOver -= OnGameOver;
        }

        /// <summary>
        /// Event listener that listen to EventManager's onGameOver event.
        /// </summary>
        /// <seealso cref="EventManager.onGameOver"/>
        void OnGameOver()
        {
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OpenGameOverUI();
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
    }
}

using UnityEngine;
using System.Collections;

namespace CliffLeeCL
{
    /// <summary>
    /// The class controls how in-game menu works.
    /// </summary>
    public class InGameMenu : MonoBehaviour
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
        /// Is true when the game is over.
        /// </summary>
        bool isGameOver = false;

        /// <summary>
        /// Start is called once on the frame when a script is enabled.
        /// </summary>
        void Start()
        {
            EventManager.Instance.onGameOver += OnGameOver;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (!isGameOver && Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
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
            isGameOver = true;
        }

        /// <summary>
        /// Used to toggle pause.
        /// </summary>
        void TogglePause()
        {
            if (Time.timeScale > 0.0f)
            {
                Time.timeScale = 0.0f;
                ToggleUI(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1.0f;
                ToggleUI(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        /// <summary>
        /// Used to toggle UI.
        /// </summary>
        /// <param name="isActive"></param>
        void ToggleUI(bool isActive)
        {
            foreach (GameObject obj in objectsToOpen)
                obj.SetActive(isActive);
            foreach (GameObject obj in objectsToClose)
                obj.SetActive(!isActive);
        }

        /// <summary>
        /// Is called when resume button is called. Resume the game.
        /// </summary>
        public void ResumeButtonCallback()
        {
            TogglePause();
        }
    }
}

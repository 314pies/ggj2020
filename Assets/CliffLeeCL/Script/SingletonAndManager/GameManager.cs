using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace CliffLeeCL
{
    /// <summary>
    /// This singleton and facade class manage main processes in the game.
    /// </summary>
    public class GameManager : SingletonMono<GameManager>
    {


        /// <summary>
        /// Define how long a sigle round is.
        /// </summary>
        public float roundTime = 60.0f;

        [HideInInspector]
        /// <summary>
        /// Is true when the player is in game.
        /// </summary>
        public bool isInGame = false;

        /// <summary>
        /// Is true when the game is over.
        /// </summary>
        bool isGameOver= false;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
            isInGame = false;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// This function is called after a new level was loaded.
        /// </summary>
        /// <param name="level">The index of the level that was loaded.</param>
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Time.timeScale = 1.0f;

            if (scene.buildIndex > 0)
            {
                isInGame = true;
                isGameOver = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                isInGame = false;
                isGameOver = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        void OnRoundTimeIsUp()
        {
            GameOver();
        }

        void GameOver()
        {
            isGameOver = true;
            EventManager.Instance.OnGameOver();
            Time.timeScale = 0.0f;
        }
    }
}

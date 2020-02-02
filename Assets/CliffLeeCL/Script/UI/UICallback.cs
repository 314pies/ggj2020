using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CliffLeeCL
{
    /// <summary>
    /// This class collect all UI's callback function.
    /// </summary>
    public class UICallback : MonoBehaviour
    {
        /// <summary>
        /// Is called when menu button is called. Return to the menu.
        /// </summary>
        public void StartButtonCallback(int levelIndex)
        {
            SceneManager.LoadScene(levelIndex);
        }

        /// <summary>
        /// Is called when restart button is called. Restrat the game.
        /// </summary>
        public void RestartButtonCallback()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Is called when menu button is called. Return to the menu.
        /// </summary>
        public void MenuButtonCallback()
        {
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// Is called when exit button is called. Exit the game.
        /// </summary>
        public void ExitButtonCallback()
        {
            Application.Quit();
            Debug.Log("Exit game!");
        }
    }
}


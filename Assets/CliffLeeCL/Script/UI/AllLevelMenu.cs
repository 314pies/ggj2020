using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace CliffLeeCL
{
    /// <summary>
    /// Is used to generate buttons to access all levels in build settings.
    /// </summary>
    public class AllLevelMenu : MonoBehaviour
    {
        /// <summary>
        /// Button prefab to load each level.
        /// </summary>
        public GameObject levelButton;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            for(int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                GameObject obj = Instantiate(levelButton, transform);
                string sceneName = GetSceneNameByScenePath(SceneUtility.GetScenePathByBuildIndex(i));
                Button button;
                Text buttonText;
                int index = i;

                obj.name = sceneName + obj.name;
                button = obj.GetComponent<Button>();
                button.onClick.AddListener(delegate{
                    LoadLevel(index);
                    });
                buttonText = obj.GetComponentInChildren<Text>();
                buttonText.text = sceneName;
            }
        }

        /// <summary>
        /// Load the specific level.
        /// </summary>
        /// <param name="index">build index of the scene.</param>
        void LoadLevel(int index)
        {
            SceneManager.LoadScene(index);
        }

        /// <summary>
        /// Parse path of the scene to get scene's name.
        /// </summary>
        /// <param name="scenePath">Path of the scene.</param>
        /// <returns>Name of the scene</returns>
        public static string GetSceneNameByScenePath(string scenePath)
        {
            int nameStratIndex = scenePath.LastIndexOf("/") + 1;
            int nameEndIndex = scenePath.LastIndexOf(".");
            int nameLength = nameEndIndex - nameStratIndex;

            return scenePath.Substring(nameStratIndex, nameLength);
        }
    }
}

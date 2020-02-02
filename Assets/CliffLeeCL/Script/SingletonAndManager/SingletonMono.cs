using UnityEngine;

namespace CliffLeeCL
{
    /// <summary>
    /// The base class for singleton with MonoBehaviour.
    /// </summary>
    /// <typeparam name="T">The class to use the singleton.</typeparam>
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        private static object lockObj = new object();

        /// <summary>
        /// Is used to prevent the sigleton creation after it is destroyed.
        /// </summary>
        private static bool isApplicationQuit = false;

        public static T Instance
        {
            get
            {
                if (isApplicationQuit)
                {
                    Debug.LogWarning("You are trying to access destroyed the singleton(" + typeof(T) + ")");
                    return null;
                }

                // Can only accessed one at a time in the lock block.
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<T>();

                        if (FindObjectsOfType<T>().Length > 1)
                        {
                            Debug.LogError("There is more than one instance in the singleton(" + typeof(T) + ")!");
                            return instance;
                        }

                        // If there is no any cretead singleton.
                        if (instance == null)
                        {
                            GameObject singleton = new GameObject(typeof(T).ToString());

                            instance = singleton.AddComponent<T>();
                            DontDestroyOnLoad(singleton);
                            Debug.Log("Create a new singleton(" + typeof(T) + ")!");
                        }
                    }

                    return instance;
                }
            }
        }

        // If there is singleton in the editor, this Awake will make it work.
        public virtual void Awake()
        {
            T Tcomponent = GetComponent<T>();

            if (instance == null)
                instance = Tcomponent;
            else if (instance != Tcomponent)
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        public void OnApplicationQuit()
        {
            isApplicationQuit = true;
        }
    }
}
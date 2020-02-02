namespace CliffLeeCL
{
    /// <summary>
    /// The base class for singleton with no MonoBehaviour.
    /// </summary>
    /// <typeparam name="T">The class to use the singleton.</typeparam>
    public class Singleton<T> where T : class, new()
    {
        private static T instance;

        private static object lockObj = new object();

        public static T Instance
        {
            get
            {
                // Can only accessed one at a time in the lock block.
                lock (lockObj)
                {
                    if (instance == null)
                        instance = new T();
                    return instance;
                }
            }
        }
    }
}

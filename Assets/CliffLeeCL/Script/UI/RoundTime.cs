using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CliffLeeCL
{
    /// <summary>
    /// The class is used to show round time in text format.
    /// </summary>
    public class RoundTime : MonoBehaviour
    {
        /// <summary>
        /// The text to show round time.
        /// </summary>
        public Text roundTimeText;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
        
        }

        /// <summary>
        /// Turn time to string in (minutes : seconds) format.
        /// </summary>
        /// <param name="time">Time to be parsed.</param>
        /// <returns>A string of parsed time in (minutes : seconds) format.</returns>
        string TimeToString(float time)
        {
            string timeString = string.Format("{0:0} : {1:00}", (int)time / 60, (int)time % 60); ;

            return timeString;
        }
    }
}

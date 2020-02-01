using UnityEngine;

namespace CliffLeeCL
{
    /// <summary>
    /// Define each player's basic status.
    /// </summary>
    public class PlayerStatus : MonoBehaviour
    {
        [Header("Movement related")]
        /// <summary>
        /// Define how fast the player moves.
        /// </summary>
        public float movingSpeed = 300.0f;
        /// <summary>
        /// Define how fast the player sprints.
        /// </summary>
        public float sprintSpeed = 500.0f;
        /// <summary>
        /// Define how fast the player climbs.
        /// </summary>
        public float climbingSpeed = 200.0f;
        /// <summary>
        /// Define how tall the player jumps.
        /// </summary>
        public float jumpForce = 30.0f;
        /// <summary>
        /// Define how fast the player rotates.
        /// </summary>
        [Tooltip("(Degree/Second)")]
        public float angularSpeed = 80.0f;
        [Space]
        /// <summary>
        /// The variable stored current stamina.
        /// </summary>
        public float currentStamina = 100.0f;
        /// <summary>
        /// Define how much stamina the player can have.
        /// </summary>
        public float maxStamina = 100.0f;
        /// <summary>
        /// Define the amount that the player's stamina will lose.
        /// </summary>
        public float staminaLossPerSecond = 40.0f;
        /// <summary>
        /// Define the amount that the player's stamina will recover.
        /// </summary>
        public float staminaRecoveryPerSecond = 25.0f;
        /// <summary>
        /// Define the time that the player have to wait for recovering stamina.
        /// </summary>
        public float restTimeWhenDrained = 2.0f;
    }
}


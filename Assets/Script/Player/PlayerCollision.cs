using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CliffLeeCL
{
    /// <summary>
    /// Handle player-related collision.
    /// </summary>
    [System.Obsolete]
    public class PlayerCollision : MonoBehaviour
    {
        /// <summary>
        /// Is true when the player is collided with enemy.
        /// </summary>
        [HideInInspector]
        public bool isCollidedWithEnemy = false;
        /// <summary>
        /// Is true when the player is in ore's range.
        /// </summary>
        [HideInInspector]
        public bool isInOreRange = false;
        /// <summary>
        /// Is true when the player is in anvil's range.
        /// </summary>
        [HideInInspector]
        public bool isInAnvilRange = false;
        /// <summary>
        /// Is true when the player is in item's range.
        /// </summary>
        [HideInInspector]
        public bool isInItemRange = false;

        /// <summary>
        /// Storing the recent collision data.
        /// </summary>
        [HideInInspector]
        public Collision2D collisionData = null;
        /// <summary>
        /// Storing the recent collider data.
        /// </summary>
        [HideInInspector]
        public Collider2D colliderData = null;
        /// <summary>
        /// Storing the recent item's collider data.
        /// </summary>
        [HideInInspector]
        public Collider2D itemColliderData = null;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (collisionData == null)
                isCollidedWithEnemy = false;

            if (colliderData == null)
            {
                isInOreRange = false;
                isInAnvilRange = false;
            }

            if (itemColliderData == null)
                isInItemRange = false;
        }

        /// <summary>
        /// Sent each frame where a collider on another object is touching this object's collider (2D physics only).
        /// </summary>
        /// <param name="col">The Collision data associated with this collision.</param>
        void OnCollisionStay2D(Collision2D col)
        {
            if (col.gameObject.tag == "Enemy")
            {
                isCollidedWithEnemy = true;
                collisionData = col;
            }
        }

        /// <summary>
        /// ent when a collider on another object stops touching this object's collider (2D physics only).
        /// </summary>
        /// <param name="col">The Collision data associated with this collision.</param>
        void OnCollisionExit2D(Collision2D col)
        {
            if (col.gameObject.tag == "Enemy")
            {
                isCollidedWithEnemy = false;
                collisionData = null;
            }
        }

        /// <summary>
        /// Sent each frame where another object is within a trigger collider attached to this object (2D physics only).
        /// </summary>
        /// <param name="col">The Collision data associated with this collision.</param>
        void OnTriggerStay2D(Collider2D col)
        {

            if (col.tag == "Ore")
            {
                isInOreRange = true;
                colliderData = col;
            }
            else if (col.tag == "Anvil")
            {
                isInAnvilRange = true;
                colliderData = col;
            }
            else if (col.tag == "Item")
            {
                isInItemRange = true;
                itemColliderData = col;
            }
        }

        /// <summary>
        /// Sent when another object leaves a trigger collider attached to this object (2D physics only).
        /// </summary>
        /// <param name="col">The Collision data associated with this collision.</param>
        void OnTriggerExit2D(Collider2D col)
        {

            if (col.tag == "Ore")
            {
                isInOreRange = false;
                colliderData = null;
            }
            else if (col.tag == "Anvil")
            {
                isInAnvilRange = false;
                colliderData = null;
            }
            else if (col.tag == "Item")
            {
                isInItemRange = false;
                itemColliderData = null;
            }
        }
    }
}


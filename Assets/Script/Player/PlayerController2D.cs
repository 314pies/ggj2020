using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CliffLeeCL
{
    /// <summary>
    /// Control player's 2D movement and rotation by user's input. 
    /// The movement feeling will be effected by input setteings' gravity field.
    /// </summary>
    public class PlayerController2D : MonoBehaviour
    {

        /// <summary>
        /// Disable/enable player left-right movement control
        /// </summary>
        /// <param name="state"></param>
        public void DisableLeftRightMovement(bool state)
        {
            this.enabled = state;
        }

        /// <summary>
        /// Define there are how many movement types are supported.
        /// </summary>
        public enum MovementType
        {
            Platformer,
            TopDown
        };

        /// <summary>
        /// Decide how the player moves.
        /// </summary>
        public MovementType movementType = MovementType.Platformer;

        /// <summary>
        /// Is used to emit sprint effect particle.
        /// </summary>
        public ParticleSystem sprintEffect;
        /// <summary>
        /// Decide whether the player can sprint or not.
        /// </summary>
        public bool canSprint = true;
        /// <summary>
        /// Define the key that is used for sprint;
        /// </summary>
        public KeyCode sprintKey = KeyCode.LeftShift;
        /// <summary>
        /// Decide whether the player can jump or not.
        /// </summary>
        public bool canJump = true;

        /// <summary>
        /// Define where the checker should be relatively to player.
        /// </summary>
        [Header("Ground check")]
        public Vector3 checkerPositionOffset = Vector3.zero;
        /// <summary>
        /// Define how large the checker is.
        /// </summary>
        public float checkerRadius = 1.0f;
        /// <summary>
        /// Define how many layers the checker will check.
        /// </summary>
        public LayerMask checkLayer;

        Rigidbody2D rigid;
        /// <summary>
        /// Is used to get movement related data.
        /// </summary>
        PlayerStatus status;
        /// <summary>
        /// Is used to store the local scale on x coordinate when game starts.
        /// </summary>
        float startLocalScaleX;
        /// <summary>
        /// Is true when player is on the ground.
        /// </summary>
        bool isGrounded = true;
        /// <summary>
        /// Is true when player is sprinting.
        /// </summary>
        bool isSprinting = false;
        /// <summary>
        /// Is true when player drain his stamina.
        /// </summary>
        bool isDrained = false;

        /// <summary>
        /// Start is called once on the frame when a script is enabled.
        /// </summary>
        void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            Assert.IsTrue(rigid, "Need \"Rigidbody\" component on this gameObject");
            status = GetComponent<PlayerStatus>();
            Assert.IsTrue(status, "Need \"PlayerStatus\" component on this gameObject");

            status.currentStamina = status.maxStamina;
            startLocalScaleX = transform.localScale.x;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            UpdateSprintEffect();
            UpdateStamina();
        }

        /// <summary>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
        /// </summary>
        void FixedUpdate()
        {
            if (movementType == MovementType.Platformer)
            {
                UpdateMovementPlatformer();
                UpdateRotationPlatformer();
                UpdateIsGrounded();
                UpdateJump();
            }
            else if(movementType == MovementType.TopDown)
            {
                UpdateMovementTopDown();
                UpdateRotationTopDown();
            }
        }

        /// <summary>
        /// The function is used to emit sprint effect paticle when the player is sprinting.
        /// </summary>
        void UpdateSprintEffect()
        {
            if (isSprinting)
            {
                if (sprintEffect)
                    sprintEffect.Play();
            }
            else
            {
                if (sprintEffect)
                    sprintEffect.Stop();
            }
        }

        /// <summary>
        /// Recover or decrease player's stamina in conditions.
        /// </summary>
        void UpdateStamina()
        {
            if (isSprinting)
            {
                if (status.currentStamina > 0.0f)
                    status.currentStamina -= status.staminaLossPerSecond * Time.deltaTime;
                else
                {
                    status.currentStamina = 0.0f;
                    isDrained = true;
                    StartCoroutine("Rest", status.restTimeWhenDrained);
                }
            }
            else if (!isDrained) // Have to rest a while if the player is drained.
            {
                if (status.currentStamina < status.maxStamina)
                    status.currentStamina += status.staminaRecoveryPerSecond * Time.deltaTime;
                else
                    status.currentStamina = status.maxStamina;
            }
        }

        /// <summary>
        /// Coroutine that will makes the player to wait for restTime to recover its stamina.
        /// </summary>
        /// <param name="restTime">The time the player needs to rest.</param>
        /// <returns>Interface that all coroutines use.</returns>
        IEnumerator Rest(float restTime)
        {
            yield return new WaitForSeconds(restTime);
            isDrained = false;
        }

        /// <summary>
        /// Update player's movement via assigning new velocity for platformer.
        /// </summary>
        private void UpdateMovementPlatformer()
        {
            float inputHorizontal = Input.GetAxis("Horizontal");
            Vector2 direction = new Vector2(inputHorizontal, 0.0f).normalized;
            Vector2 moveVelocity = (direction * status.movingSpeed * Time.fixedDeltaTime);
            Vector2 sprintVelocity = (direction * status.sprintSpeed * Time.fixedDeltaTime);
            bool isSprintKeyPressed = Input.GetKey(sprintKey);

            if (canSprint && isSprintKeyPressed && (status.currentStamina > 0))
            {
                rigid.velocity = new Vector2(sprintVelocity.x, rigid.velocity.y);
                if (sprintVelocity.sqrMagnitude > Mathf.Epsilon)// Is really moving.
                    isSprinting = true;
                else
                    isSprinting = false;
            }
            else
            {
                rigid.velocity = new Vector2(moveVelocity.x, rigid.velocity.y);
                isSprinting = false;
            }
        }

        /// <summary>
        /// Update player's rotation for platformer.
        /// </summary>
        private void UpdateRotationPlatformer()
        {
            float inputHorizontal = Input.GetAxis("Horizontal");

            if (inputHorizontal != 0.0f)
                transform.localScale = new Vector3(Mathf.Sign(inputHorizontal) * startLocalScaleX, transform.localScale.y, transform.localScale.z);
        }

        /// <summary>
        /// Update player's movement via assigning new velocity for top down.
        /// </summary>
        private void UpdateMovementTopDown()
        {
            float inputHorizontal = Input.GetAxis("Horizontal");
            float inputVertical = Input.GetAxis("Vertical");
            Vector2 direction = new Vector2(inputHorizontal, inputVertical).normalized;
            Vector2 moveVelocity = (direction * status.movingSpeed * Time.fixedDeltaTime);
            Vector2 sprintVelocity = (direction * status.sprintSpeed * Time.fixedDeltaTime);
            bool isSprintKeyPressed = Input.GetKey(sprintKey);

            if (canSprint && isSprintKeyPressed && (status.currentStamina > 0))
            {
                rigid.velocity = new Vector2(sprintVelocity.x, sprintVelocity.y);
                if (sprintVelocity.sqrMagnitude > Mathf.Epsilon)// Is really moving.
                    isSprinting = true;
                else
                    isSprinting = false;
            }
            else
            {
                rigid.velocity = new Vector2(moveVelocity.x, moveVelocity.y);
                isSprinting = false;
            }
        }

        /// <summary>
        /// Update player's rotation for top down.
        /// </summary>
        private void UpdateRotationTopDown()
        {
            float inputHorizontal = Input.GetAxis("Horizontal");
            float inputVertical = Input.GetAxis("Vertical");
            Vector2 direction = new Vector2(inputHorizontal, inputVertical).normalized;

            if (direction != Vector2.zero)
            {
                float lookRot;

                lookRot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rigid.MoveRotation(lookRot - 90);   //use y axis as foward.
            }
        }

        /// <summary>
        /// Check whether the player is grounded.
        /// </summary>
        private void UpdateIsGrounded()
        {
            if (Physics2D.OverlapCircle(transform.position + checkerPositionOffset, checkerRadius, checkLayer))
                isGrounded = true;
            else
                isGrounded = false;
        }

        /// <summary>
        /// Update player's jump via adding impulse force.
        /// </summary>
        private void UpdateJump()
        {
            bool isJumpping = Input.GetButtonDown("Jump");

            if (canJump && isGrounded && isJumpping)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0f);
                rigid.AddForce(new Vector2(0f, status.jumpForce), ForceMode2D.Impulse);
                isGrounded = false;
            }
        }


        /// <summary>
        /// Implement this OnDrawGizmosSelected if you want to draw gizmos only if the object is selected.
        /// </summary>
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + checkerPositionOffset, checkerRadius);
        }
    }
}

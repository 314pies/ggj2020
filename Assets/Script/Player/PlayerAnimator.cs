using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CliffLeeCL
{
    /// <summary>
    /// This class is used to contorl player's animator.
    /// </summary>
    public class PlayerAnimator : MonoBehaviour
    {
        Animator animator;
        Rigidbody2D rigid;

        /// <summary>
        /// Start is called once on the frame when a script is enabled.
        /// </summary>
        void Start()
        {
            animator = GetComponent<Animator>();
            Assert.IsTrue(animator, "Need \"Animator\" component on this gameObject");
            rigid = GetComponent<Rigidbody2D>();
            Assert.IsTrue(rigid, "Need \"Rigidbody\" component on this gameObject");
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            UpdateAnimator();
        }

        /// <summary>
        /// Update player's animator with some parameters.
        /// </summary>
        private void UpdateAnimator()
        {
            animator.SetFloat("Velocity", Mathf.Abs(rigid.velocity.x));
        }
    }
}


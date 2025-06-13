using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : KinematicObject
{
    /// <summary>
    /// Max horizontal speed.
    /// </summary>
    public float maxSpeed = 7;
    /// <summary>
    /// Max jump velocity
    /// </summary>
    public float jumpTakeOffSpeed = 7;

    /// <summary>
    /// Used to indicated desired direction of travel.
    /// </summary>
    public Vector2 move;

    /// <summary>
    /// Set to true to initiate a jump.
    /// </summary>
    public bool jump;

    /// <summary>
    /// Set to true to set the current jump velocity to zero.
    /// </summary>
    public bool stopJump;

    SpriteRenderer spriteRenderer;
    Animator animator;
    // [SerializeField] private PlatformerModel model;
    public float jumpModifier = 1.5f;
    public float jumpDeceleration = 0.5f;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        if (jump && IsGrounded)
        {
            velocity.y = jumpTakeOffSpeed * jumpModifier;
            jump = false;
        }
        else if (stopJump)
        {
            stopJump = false;
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * jumpDeceleration;
            }
        }

        if (move.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (move.x < -0.01f)
            spriteRenderer.flipX = true;

        // animator.SetBool("grounded", IsGrounded);
        // animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }
}

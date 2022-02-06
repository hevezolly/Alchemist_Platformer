using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new JumpOption", menuName = "game/move options/jump option")]
public class JumpOption : MoveOption
{
    [Header("JumpOption")]
    public float jumpTime;
    public float jumpGravity;
    public float horizontalVelocityCoefficient = 5f / 7f;
    public float ButtonDelay;
    public float JumpDelay;
    public float maxAnimatorFallSpeed;

    private bool needToResetSide;
    private float resetSideMoment;
    private float cantJumpMoment;
    private bool needToRemoveJump;
    private float buttonPressMoment;
    private bool isJumpButtonPressed;
    private float jumpMoment;

    private int remainingJumps;
    public bool canJump => remainingJumps > 0;
    private float standartGravity;
    public bool inJump { get; private set; }
    private int jumpSide = 0;

    public void SetUp(GameObject target)
    {
        base.SetUp(target, this);
        standartGravity = rb.gravityScale;
    }
    public override void FixedUpdate()
    {
        TryJump();
    }

    private void UpdateAnimations()
    {
        animator.SetBool("fall", !inJump && rb.velocity.y < -0.1f);
        animator.SetFloat("fallSpeed", Mathf.InverseLerp(0, -maxAnimatorFallSpeed, Mathf.Clamp(rb.velocity.y, -maxAnimatorFallSpeed, 0)));
    }

    public override bool IsConditionsReached()
    {
        return true;
    }

    private void TryJump()
    {
        if (canJump && isJumpButtonPressed)
        {
            animator.SetTrigger("Jump");
            inJump = true;
            remainingJumps--;
            jumpMoment = Time.time;
            if (rb.velocity.y < creatureStatus.jumpVelocity)
                rb.velocity = new Vector2(rb.velocity.x + jumpSide * creatureStatus.jumpVelocity * horizontalVelocityCoefficient, 
                    creatureStatus.jumpVelocity);
            isJumpButtonPressed = false;
            rb.gravityScale = jumpGravity;
        }
    }

    private void InJump()
    {
        if (inJump && jumpMoment + jumpTime >= Time.time && Input.GetKey(KeyCode.Space))
        {
            rb.gravityScale = Mathf.Lerp(standartGravity, jumpGravity, (jumpMoment + jumpTime - Time.time) / jumpTime);
        }
        else if (inJump)
        {
            inJump = false;
            rb.gravityScale = standartGravity;
        }
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            buttonPressMoment = Time.time;
            isJumpButtonPressed = true;
        }
        if (isJumpButtonPressed && ((buttonPressMoment + ButtonDelay < Time.time) || Input.GetKeyUp(KeyCode.Space)))
            isJumpButtonPressed = false;
        if (canJump && needToRemoveJump && cantJumpMoment + JumpDelay < Time.time)
        {
            if (remainingJumps == creatureStatus.numOfJumps)
                remainingJumps--;
            needToRemoveJump = false;
        }
        if (jumpSide != 0 && needToResetSide && resetSideMoment + JumpDelay < Time.time)
        {
            jumpSide = 0;
            needToResetSide = false;
        }
        InJump();
        UpdateAnimations();
    }

    public void SetJumpSide(int side)
    {
        jumpSide = side;
        needToResetSide = false;
    }

    public void ResetJumpSide()
    {
        if (!needToResetSide)
        {
            resetSideMoment = Time.time;
            needToResetSide = true;
        }

    }

    public void RecoverJump()
    {
        needToRemoveJump = false;
        remainingJumps = creatureStatus.numOfJumps;
    }

    public void RemoveJump()
    {
        if (!needToRemoveJump)
        {
            cantJumpMoment = Time.time;
            needToRemoveJump = true;
        }
    }
}

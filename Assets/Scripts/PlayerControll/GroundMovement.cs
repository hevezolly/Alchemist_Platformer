using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new GroundMovement", menuName = "game/move options/Ground Movement")]
public class GroundMovement : MoveOption
{
    [Header("GroundMovement")]
    public float acceleration;
    public float decceleration;
    private float standartScale;
    public float animationMoveSpeed;
    private Transform groundContainer;
    public float checkGroundDistance;
    public LayerMask ground;

    public override void SetUp(GameObject target, JumpOption jump)
    {
        base.SetUp(target, jump);
        jump.SetUp(target, jump);
        standartScale = transform.localScale.x;
        groundContainer = transform.Find("GroundPoints");
    }

    public override void FixedUpdate()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var currentVel = rb.velocity;
        if ((Mathf.Abs(horizontal) == 0 && Mathf.Abs(currentVel.x) > 0.1f)
            || (Mathf.Abs(horizontal) > 0 && Mathf.Sign(currentVel.x) != horizontal))
        {
            currentVel -= Vector2.right * Mathf.Sign(currentVel.x) * decceleration * Time.deltaTime;
        }
        if (Mathf.Abs(horizontal) > 0)
        {
            transform.localScale = new Vector3(standartScale * horizontal, transform.localScale.y, transform.localScale.z);
            currentVel += Vector2.right * horizontal * acceleration * Time.deltaTime;
            currentVel.x = Mathf.Clamp(currentVel.x, -creatureStatus.speed, creatureStatus.speed);
        }

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(currentVel.x) < 0.1f)
            currentVel.x = 0;
        rb.velocity = currentVel;

        jump.FixedUpdate();
    }

    public override void TurnOff()
    {
        
    }

    public override bool IsConditionsReached()
    {
        for (var i = 0; i < groundContainer.childCount; i++)
        {
            var origin = groundContainer.GetChild(i);
            var hit = Physics2D.Raycast(origin.position, Vector2.down, checkGroundDistance, ground);
            if (hit)
            {
                animator.SetBool("fall", false);
                animator.SetBool("OnGround", true);
                jump.RecoverJump();
                animator.SetFloat("moveSpeed", Mathf.LerpUnclamped(0, animationMoveSpeed, Mathf.Abs(rb.velocity.x) / creatureStatus.speed));
                return true;
            }
        }
        animator.SetBool("OnGround", false);
        jump.RemoveJump();
        return false;
    }

    public override void OnDrawGizoms()
    {
        if (groundContainer == null)
            return;
        Gizmos.color = Color.white;
        for (var i = 0; i < groundContainer.childCount; i++)
        {
            Gizmos.DrawLine(groundContainer.GetChild(i).position,
                groundContainer.GetChild(i).position + Vector3.down * checkGroundDistance);
        }
    }


}

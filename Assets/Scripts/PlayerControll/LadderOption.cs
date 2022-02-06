using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Ladder", menuName = "game/move options/ladder")]
public class LadderOption : MoveOption
{
    [Header("Ladder option")]
    public LayerMask laddersLevel;
    public LayerMask ground;
    public float climbAnimationSpeed;
    

    private bool isAlreadyOnLadder;
    private Transform toursoLevel;
    public float sideDistance;
    private Transform headLevel;
    public float horizontalSpeed;
    public float verticalSpeed;

    public override void SetUp(GameObject target, JumpOption jump)
    {
        base.SetUp(target, jump);
        toursoLevel = transform.Find("SidePoints").GetChild(1);
        headLevel = transform.Find("headLevel");
    }


    public override void FixedUpdate()
    {
        var hor = Input.GetAxisRaw("Horizontal");
        var vert = Input.GetAxisRaw("Vertical");
        if (!HeadOnLadder())
            vert = Mathf.Min(0, vert);
        if (CheckSide(hor))
            hor = 0;
        rb.velocity = Vector2.up * verticalSpeed * vert +
            Vector2.right * horizontalSpeed * hor;
        jump.SetJumpSide(Mathf.RoundToInt(hor));
    }

    public override void TurnOn()
    {
        jump.RecoverJump();
        animator.SetTrigger("OnLadder");
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        isAlreadyOnLadder = true;
        
    }

    public override void TurnOff()
    {
        rb.isKinematic = false;
        isAlreadyOnLadder = false;
        animator.SetTrigger("OnLadderFinish");
        jump.ResetJumpSide();
        jump.RemoveJump();
    }

    private bool CheckSide(float dir)
    {
        var origin = toursoLevel.position;
        var hit = Physics2D.Raycast(origin, Vector2.right * dir, sideDistance, ground);
        return hit;
    }

    private bool HeadOnLadder()
    {
        var origin = headLevel.position;
        var hit = Physics2D.Raycast(origin, Vector2.zero, 1, laddersLevel);
        return hit;
    }

    public override void Update()
    {
        var t = Mathf.Abs(rb.velocity.y) / verticalSpeed;
        animator.SetFloat("ladderSpeed", Mathf.Lerp(0, climbAnimationSpeed, t));
        jump.RecoverJump();
        if (jump.inJump)
            isAlreadyOnLadder = false;
    }


    private bool IsOnLadder()
    {
        var origin = toursoLevel.position;
        var hit = Physics2D.Raycast(origin, Vector2.zero, 1, laddersLevel);
        return hit;
    }

    public override bool IsConditionsReached()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        var onLadder = IsOnLadder();
        var smallVelocity = rb.velocity.y <= verticalSpeed + 0.1f;
        if (onLadder && !jump.inJump && (isAlreadyOnLadder || vertical != 0))
        {
            return true;
        }
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Wall Option", menuName = "game/move options/wall option")]
public class WallOption : MoveOption
{
    [Header("Wall Option")]
    public float wallVelocityUp;
    public float wallVelocityDown;
    public float checkSideDistance;
    public float maxClimbSpeed;
    public LayerMask ground;

    public float bumpVelocity;

    private int wallSide = 0;
    private bool isOnWall;
    private Transform toursoLevel;
    private Transform headLevel;

    private Transform sideContainer;


    public override void SetUp(GameObject target, JumpOption jump)
    {
        base.SetUp(target, jump);
        sideContainer = transform.Find("SidePoints");
        headLevel = transform.Find("headLevel");
        toursoLevel = sideContainer.GetChild(1);
    }

    public override void Update()
    {
        jump.RecoverJump();
    }

    private void ChangeWallVelosity()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        if (!jump.inJump)
        {
            var t = 0f;
            if (vertical == 0)
            {
                animator.SetBool("OnClimb", false);
                rb.velocity = new Vector2(rb.velocity.x, 0);
                t = 0f;
            }
            else if (vertical == 1)
            {
                animator.SetBool("OnClimb", true);
                rb.velocity = new Vector2(rb.velocity.x, wallVelocityUp);
                t = rb.velocity.y / wallVelocityUp;
            }
            else
            {
                animator.SetBool("OnClimb", true);
                rb.velocity = new Vector2(rb.velocity.x, -wallVelocityDown);
                t = -rb.velocity.y / wallVelocityDown;
            }
            animator.SetFloat("verticalSpeed", Mathf.Lerp(0, maxClimbSpeed, t));
        }
        if (isWallBellowTourso() && jump.canJump && !jump.inJump && vertical > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + bumpVelocity);
        }
    }

    private bool isWallBellowTourso()
    {
        var origin = toursoLevel.position;
        var hit = Physics2D.Raycast(origin, Vector2.right * wallSide, checkSideDistance, ground);
        var hitHead = Physics2D.Raycast(headLevel.position, Vector2.right * wallSide, checkSideDistance, ground);
        return !hit && !hitHead;
    }

    public override void TurnOn()
    {
        animator.SetBool("OnWall", true);
        isOnWall = true;
        jump.RecoverJump();
        CheckWallCollision(out var side);
        wallSide = side;
        jump.SetJumpSide(-side);
    }

    public override void TurnOff()
    {
        animator.SetBool("OnWall", false);
        isOnWall = false;
        jump.RemoveJump();
        jump.ResetJumpSide();
    }

    private bool CheckWallCollision(out int side)
    {
        side = 0;
        var horizontal = Input.GetAxisRaw("Horizontal");
        for (var i = 0; i < sideContainer.childCount; i++)
        {
            var origin = sideContainer.GetChild(i);
            side = 1;
            var hit = Physics2D.Raycast(origin.position, Vector2.right, checkSideDistance, ground);
            if (hit && horizontal == side)
            {
                return true;
            }
            side = -1;
            hit = Physics2D.Raycast(origin.position, Vector2.left, checkSideDistance, ground);
            if (hit && horizontal == side)
            {
                return true;
            }
        }
        return false;
    }
    public override void FixedUpdate()
    {
        ChangeWallVelosity();
    }

    public override bool IsConditionsReached()
    {
        var lowVelocity = rb.velocity.y <= 0;
        return CheckWallCollision(out var _) && (isOnWall || lowVelocity);
    }

    public override void OnDrawGizoms()
    {
        if (sideContainer == null)
            return;
        Gizmos.color = Color.white;
        for (var i = 0; i < sideContainer.childCount; i++)
        {
            Gizmos.DrawLine(sideContainer.GetChild(i).position - Vector3.right * checkSideDistance,
                sideContainer.GetChild(i).position + Vector3.right * checkSideDistance);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawLine(toursoLevel.position - Vector3.right * checkSideDistance,
                toursoLevel.position + Vector3.right * checkSideDistance);
        Gizmos.DrawLine(headLevel.position - Vector3.right * checkSideDistance,
                headLevel.position + Vector3.right * checkSideDistance);
    }
}

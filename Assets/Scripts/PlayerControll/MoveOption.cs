using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveOption : ScriptableObject
{
    protected GameObject gameObject;
    protected Transform transform;
    protected Rigidbody2D rb;
    protected JumpOption jump;
    protected Animator animator;
    protected CreatureStatus creatureStatus;

    public virtual void SetUp(GameObject target, JumpOption jump)
    {
        gameObject = target;
        transform = target.transform;
        creatureStatus = target.GetComponent<CreatureStatus>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        this.jump = jump;
        animator = gameObject.GetComponent<Animator>();
    }
  
    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {

    }

    public virtual void NeccessaryUpdate()
    {

    }

    public virtual void TurnOn()
    {

    }

    public virtual void TurnOff()
    {

    }

    public virtual void NeccessaryFixedUpdate()
    {

    }

    public abstract bool IsConditionsReached();

    public virtual void OnDrawGizoms()
    {
        
    }
}

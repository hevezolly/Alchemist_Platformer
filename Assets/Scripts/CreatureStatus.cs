using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureStatus : MonoBehaviour
{
    public float speed;
    public int numOfJumps;
    public int MaxHealth;
    public float jumpVelocity;
    public int health { get; private set; }

    public bool canFly;
}

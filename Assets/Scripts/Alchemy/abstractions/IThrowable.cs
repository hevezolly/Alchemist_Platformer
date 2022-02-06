using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThrowable
{
    void Throw(ThrowInformation info);

    bool CanBeThrown { get; }
}

public class ThrowInformation
{
    public Vector2 origin;
    public Vector2 direction;
    public GameObject thrower;
}

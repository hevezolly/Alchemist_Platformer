using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Immortal : MonoBehaviour
{
    private static bool created = false;

    private void Awake()
    {
        if (created)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            created = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public Transform target;
    [Range(0, 1)]
    public float moveAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null)
            return;
        var end = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, end, moveAmount);
    }

    public void AssignPlayer(Transform player)
    {
        target = player;
    }

}

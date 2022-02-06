using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxEffect : MonoBehaviour
{
    [Range(0, 1)]
    public float Strength;
    public Transform camera;
    public TerrainGenerator generator;

    private Vector3 startPos;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        startPos = (Vector2)generator.WidthHeight / 2f;
        startPos = new Vector3(startPos.x, startPos.y, transform.position.z);
        offset = transform.position - startPos;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var camPos = new Vector3(camera.position.x, camera.position.y, startPos.z);
        transform.position = Vector3.Lerp(startPos, camPos, Strength) + offset;
    }
}

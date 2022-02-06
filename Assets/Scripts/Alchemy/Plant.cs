using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new plant", menuName = "game/plant")]
public class Plant : ScriptableObject
{
    [SerializeField]
    private GameObject plantObject;
    [SerializeField]
    private PlantItem item;
    [Space]
    [SerializeField]
    private float emissionStrength;
    [SerializeField]
    private float moveAmount;
    [SerializeField]
    private bool Up;
    [SerializeField]
    private bool Down;
    [SerializeField]
    private bool Left;
    [SerializeField]
    private bool Right;

    public HashSet<Vector2Int> GetSides()
    {
        var s = new HashSet<Vector2Int>();
        if (Up)
            s.Add(Vector2Int.up);
        if (Down)
            s.Add(Vector2Int.down);
        if (Left)
            s.Add(Vector2Int.left);
        if (Right)
            s.Add(Vector2Int.right);
        return s;
    }

    public void PlacePlant(Vector2Int side, Vector2 cordinate)
    {
        var position = (Vector3)cordinate;
        var rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)side);
        var obj = Instantiate(plantObject, position, rotation);
        obj.GetComponent<PIckupable>().SetUp(item.Copy());
        var renderer = obj.GetComponent<SpriteRenderer>();
        renderer.flipX = Random.value <= 0.5f;
        renderer.material.SetVector("_Down", (Vector2)(-side));
        renderer.material.SetFloat("_Move", moveAmount);
        renderer.material.SetFloat("_EmissionStrength", emissionStrength);
        renderer.material.SetFloat("_Rotation", obj.transform.rotation.eulerAngles.z);
        GameObject.FindGameObjectWithTag("GridController").GetComponent<GridControlller>().AddPlant(renderer);
    }

}

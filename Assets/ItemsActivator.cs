using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsActivator : MonoBehaviour
{
    public List<PlantItem> plants;

    public List<Bottle> bottles;

    private void Awake()
    {
        var id = 0;
        foreach (var p in plants)
        {
            p.ID = id;
            p.chemicalProperty.SetUp(p.Color);
            id++;
        }
        foreach (var b in bottles)
        {
            b.ID = id;
            id++;
        }
    }
}

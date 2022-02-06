using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "new Recepy", menuName = "alchemy/recepy")]
public class Recepy : ScriptableObject
{
    public List<PlantItem> components;
    public PotionEffect result;

    public bool ComponentsFitForRecepy(List<IChemicalComponent> components)
    {
        if (components.Count != this.components.Count)
            return false;
        var success = true;
        foreach (var got in components)
        {
            var found = false;
            foreach (var requare in this.components)
            {
                if (requare.ID == got.ID) {
                    found = true;
                    break;
                }
            }
            if (!found)
                return false;
        }
        return true;
    }
}

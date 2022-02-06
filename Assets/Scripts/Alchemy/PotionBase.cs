using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "new potion base", menuName = "alchemy/potion base")]
public class PotionBase : ScriptableObject
{
    [HideInInspector]
    public Element element;

    public List<AspectModifyer> modificators;

    public void FillDescription(IDescription description)
    {
        if (modificators.Count == 0)
            description.AddBase();
        else
        {
            foreach (var mod in modificators)
            {
                mod.SetDescription(description);
            }
        }
    }

    public void Modify(PotionEffect effect)
    {
        foreach (var modifyer in modificators)
        {
            modifyer.Modify(effect, element);
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class PotionEffect : ScriptableObject
{
    public PotionEffectPropery BaseProperty;

    [HideInInspector]
    public ColorController colors;

    [TextArea()]
    public string Description;
    
    [HideInInspector]
    public Element element;

    public abstract void ApplyToCharacter(GameObject obj);

    public abstract void ApplayOnArea(AreaApplication area);

    public virtual void FillDescription(IDescription description, bool useEffect = true)
    {
        if (useEffect)
            description.SetEffect(Description);
        else
            description.SetDescription(Description);
        foreach (var aspect in BaseProperty.GetAspects())
        {
            aspect.FillDescription(description);
        }
    }

    public void SetUp()
    {
        BaseProperty.InitiateAspects();
    }

    public abstract PotionEffect Copy();
}


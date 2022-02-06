using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new test effect", menuName = "alchemy/effects/test")]
public class TestEffect : PotionEffect
{
    public override void ApplayOnArea(AreaApplication area)
    {
         Debug.Log("Activate On Area");
    }

    public override void ApplyToCharacter(GameObject obj)
    {
         Debug.Log("apply on character");
    }

    public override PotionEffect Copy()
    {
        var copy = CreateInstance<TestEffect>();
        copy.Description = Description;
        copy.colors = colors;
        copy.BaseProperty = BaseProperty.Copy();
        copy.element = element;
        return copy;
    }
}

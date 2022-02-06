using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AspectModifyer : ScriptableObject
{
    public abstract void Modify(PotionEffect effect, Element baseElement);

    public abstract void SetDescription(IDescription description);
}

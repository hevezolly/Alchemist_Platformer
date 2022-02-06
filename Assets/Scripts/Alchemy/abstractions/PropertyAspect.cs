using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PropertyAspect : ScriptableObject
{
    public string parameterName;
    public Sprite icon;
    public abstract PropertyAspect Modify(ModificationType type);

    public abstract void FillDescription(IDescription description);
}

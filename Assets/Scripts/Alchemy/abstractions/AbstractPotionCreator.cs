using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPotionCreator : ScriptableObject, IPotionCreator
{
    public abstract bool CanCook { get; }

    public abstract void AddBaseComponent(IChemicalComponent component);
    public abstract void AddEffectComponent(IChemicalComponent component);
    public abstract void Clear();

    public abstract void SetUp();
    public abstract void Cook();
    public abstract void RemoveBaseComponent(IChemicalComponent component);
    public abstract void RemoveEffectComponent(IChemicalComponent component);
    public abstract void SetBottle(IBottle bottle);
}

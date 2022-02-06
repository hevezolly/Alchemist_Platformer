using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPotionCreator
{
    void AddEffectComponent(IChemicalComponent component);
    void AddBaseComponent(IChemicalComponent component);
    void RemoveBaseComponent(IChemicalComponent component);
    void RemoveEffectComponent(IChemicalComponent component);
    void SetBottle(IBottle bottle);
    void Cook();
    void Clear();

    bool CanCook { get; }

}

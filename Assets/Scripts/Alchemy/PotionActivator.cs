using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PotionActivator
{
    private List<PotionEffect> effects;

    public List<ColorController> baseColors { get; private set; }

    public Element element { get; private set; }
    public PotionActivator(List<PotionEffect> effects, List<ColorController> bases)
    {
        baseColors = bases;
        this.effects = effects;
        element = EnumController.GetStrongestElement(effects.Select(e => e.element));
    }

    public void ApplyOnCharacter(GameObject obj)
    {
        foreach (var effect in effects)
        {
            effect.ApplyToCharacter(obj);
        }
    }

    public void ApplyOnArea(AreaApplication aplication)
    {
        foreach (var effect in effects)
        {
            effect.ApplayOnArea(aplication);
        }
    }

    public IEnumerable<PotionEffect> GetEffects()
    {
        foreach (var e in effects)
            yield return e;
    }

    public int EffectsCount => effects.Count;
}

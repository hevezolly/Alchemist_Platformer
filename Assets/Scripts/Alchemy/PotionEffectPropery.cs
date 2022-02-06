using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "potion effect property", menuName = "alchemy/effect property")]
public class PotionEffectPropery : ScriptableObject
{
    [SerializeField]
    private List<PropertyAspect> Aspects;

    private Dictionary<Type, PropertyAspect> sortedAspects;
    
    public T GetPropertyAspect<T>() where T: PropertyAspect
    {
        if (sortedAspects.ContainsKey(typeof(T)))
            return null;
        return (T)(sortedAspects[typeof(T)]);
    }

    public void ModifyProperty<T>(ModificationType type) where T: PropertyAspect
    {
        var property = GetPropertyAspect<T>();
        if (property == null)
            return;
        sortedAspects[typeof(T)] = property.Modify(type);
    }

    public void ModifyProperty(PropertyAspect aspect, ModificationType type)
    {
        if (!sortedAspects.ContainsKey(aspect.GetType()))
            return;
        sortedAspects[aspect.GetType()] = sortedAspects[aspect.GetType()].Modify(type);
    }

    public IEnumerable<PropertyAspect> GetAspects()
    {
        if (Aspects == null)
            yield break;
        foreach (var a in Aspects)
            yield return a;
    }

    public void InitiateAspects()
    {
        sortedAspects = new Dictionary<Type, PropertyAspect>();
        foreach (var aspect in Aspects)
        {
            sortedAspects[aspect.GetType()] = aspect;
        }
    }
    
    public PotionEffectPropery Copy()
    {
        var copy = CreateInstance<PotionEffectPropery>();
        copy.Aspects = new List<PropertyAspect>(Aspects);
        copy.InitiateAspects();
        return copy;
    }
}

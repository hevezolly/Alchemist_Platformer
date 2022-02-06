using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "new basic potion creator", menuName = "alchemy/potion creators/basic")]
public class BasePotionCreator: AbstractPotionCreator
{
    public int NumOfEffects;
    public int NumOfBases;
    private List<IChemicalComponent> effects;
    private List<IChemicalComponent> bases;
    public List<Recepy> recepies;
    private IBottle bottle;

    public override void SetUp()
    {
        Clear();
    }

    public override void AddEffectComponent(IChemicalComponent component)
    {
        if (CanAddEffect)
        {
            effects.Add(component);
        }
    }

    public override void AddBaseComponent(IChemicalComponent component)
    {
        if (CanAddBase)
        {
            bases.Add(component);
        }
    }

    public override void RemoveBaseComponent(IChemicalComponent component)
    {
        bases.Remove(component);
    }

    public override void RemoveEffectComponent(IChemicalComponent component)
    {
        effects.Remove(component);
    }

    public override void SetBottle(IBottle bottle)
    {
        this.bottle = bottle;
    }

    public override void Cook()
    {
        var resultEffectComponents = ClearByElements(effects);
        var resultBaseComponents = ClearByElements(bases);
        var realEffects = GetEffects(resultEffectComponents);
        var baseColors = resultBaseComponents.Select(c => c.Color).ToList();
        var realBases = resultBaseComponents.Select(c => c.chemicalProperty.PotionBase);
        foreach (var recepy in recepies)
        {
            if (recepy.ComponentsFitForRecepy(effects))
            {
                realEffects = new List<PotionEffect>();
                realEffects.Add(recepy.result);
                break;
            }
        }
        foreach (var b in realBases)
        {
            foreach (var effect in realEffects)
            {
                b.Modify(effect);
            }
        }
        bottle.Fill(new PotionActivator(new List<PotionEffect>(realEffects), baseColors));
        Clear();
    }

    private List<PotionEffect> GetEffects(List<IChemicalComponent> components)
    {
        var effects = new List<PotionEffect>();
        var correspondingIDs = new List<int>();
        var modifications = new List<ModificationType>();
        foreach (var c in components)
        {
            if (!correspondingIDs.Contains(c.ID))
            {
                effects.Add(c.chemicalProperty.MainEffect);
                correspondingIDs.Add(c.ID);
                modifications.Add(ModificationType.Nutral);
                continue;
            }
            var index = correspondingIDs.FindIndex(i => i == c.ID);
            modifications[index] = EnumController.UpgradeModification(modifications[index]);
        }
        for (var index = 0; index < effects.Count; index++)
        {
            foreach (var aspect in effects[index].BaseProperty.GetAspects())
            {
                effects[index].BaseProperty.ModifyProperty(aspect, modifications[index]);
            }
        }
        return effects;
    }

    private List<IChemicalComponent> ClearByElements(List<IChemicalComponent> c)
    {
        var candidatesToRemove = new List<IChemicalComponent>();
        var components = new List<IChemicalComponent>(c);
        for (var i = 0; i < components.Count; i++)
        {
            var one = components[i];
            for (var j = i + 1; j < components.Count; j++)
            {
                var other = components[j];
                if (other.chemicalProperty.element == 
                    EnumController.GetAntipode(one.chemicalProperty.element))
                {
                    candidatesToRemove.Add(one);
                    candidatesToRemove.Add(other);
                }
            }
        }
        foreach (var candidate in candidatesToRemove)
            components.Remove(candidate);
        return components;
    }

    public override void Clear()
    {
        effects = new List<IChemicalComponent>();
        bases = new List<IChemicalComponent>();
        bottle = null;
    }

    public bool CanAddEffect => effects.Count < NumOfEffects;
    public bool CanAddBase => bases.Count < NumOfBases;

    public override bool CanCook => effects.Count > 0 && bases.Count > 0 && bottle != null;
}

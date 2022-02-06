using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class EnumController
{
    public static ModificationType UpgradeModification(ModificationType type)
    {
        switch (type)
        {
            case ModificationType.BigNegative:
                return ModificationType.StandartNegative;
            case ModificationType.BigPositive:
                return ModificationType.BigPositive;
            case ModificationType.LesserNegative:
                return ModificationType.Nutral;
            case ModificationType.LesserPositive:
                return ModificationType.StandartPositive;
            case ModificationType.Nutral:
                return ModificationType.LesserPositive;
            case ModificationType.StandartNegative:
                return ModificationType.LesserNegative;
            case ModificationType.StandartPositive:
                return ModificationType.BigPositive;
        }
        throw new System.ArgumentException("incorrect type");
    }

    public static ModificationType RegradeModification(ModificationType type)
    {
        switch (type)
        {
            case ModificationType.BigNegative:
                return ModificationType.BigNegative;
            case ModificationType.BigPositive:
                return ModificationType.StandartPositive;
            case ModificationType.LesserNegative:
                return ModificationType.StandartNegative;
            case ModificationType.LesserPositive:
                return ModificationType.Nutral;
            case ModificationType.Nutral:
                return ModificationType.LesserNegative;
            case ModificationType.StandartNegative:
                return ModificationType.BigNegative;
            case ModificationType.StandartPositive:
                return ModificationType.LesserPositive;
        }
        throw new System.ArgumentException("incorrect type");
    }

    public static Element GetAntipode(Element element)
    {
        switch (element)
        {
            case Element.Air:
                return Element.Earth;
            case Element.Death:
                return Element.Life;
            case Element.Earth:
                return Element.Air;
            case Element.Fire:
                return Element.Water;
            case Element.Life:
                return Element.Death;
            case Element.Water:
                return Element.Fire;
        }
        throw new System.ArgumentException("incorrect type");
    }

    public static Color GetElementColor(Element element)
    {
        switch (element)
        {
            case Element.Air:
                return new Color(160/255f, 255 / 255f, 249 / 255f, 0.7f);
            case Element.Death:
                return new Color(94 / 255f, 24 / 255f, 77 / 255f);
            case Element.Earth:
                return new Color(118 / 255f, 81 / 255f, 24 / 255f);
            case Element.Fire:
                return new Color(255 / 255f, 66 / 255f, 0);
            case Element.Life:
                return new Color(86 / 255f, 255 / 255f, 0);
            case Element.Water:
                return new Color(0, 74 / 255f, 237 / 255f, 0.9f);
        }
        throw new System.ArgumentException("incorrect type");
    }

    public static string GetModificationDescription(ModificationType type)
    {
        switch (type)
        {
            case ModificationType.BigNegative:
                return "greatly reduce";
            case ModificationType.BigPositive:
                return "greatly incrase";
            case ModificationType.LesserNegative:
                return "slightly reduce";
            case ModificationType.LesserPositive:
                return "slightly increase";
            case ModificationType.Nutral:
                return "does not change";
            case ModificationType.StandartNegative:
                return "reduce";
            case ModificationType.StandartPositive:
                return "increase";
        }
        throw new System.ArgumentException("incorrect type");
    }

    public static Sprite GetElementImage(Element element, bool colorfool)
    {
        var storage = GameObject.FindGameObjectWithTag("DataContainer").GetComponent<DataStorage>();
        if (colorfool)
            return storage.ColorfulElementsSprites[element];
        return storage.BlankElementsSprites[element];
    }

    public static string GetElementName(Element element)
    {
        switch (element)
        {
            case Element.Air:
                return "air";
            case Element.Death:
                return "death";
            case Element.Earth:
                return "earth";
            case Element.Fire:
                return "fire";
            case Element.Life:
                return "life";
            case Element.Water:
                return "water";
        }
        throw new System.ArgumentException("incorrect type");
    }

    public static int CompareElements(Element one, Element other)
    {
        return ((int)one).CompareTo((int)other);
    }

    public static Element GetStrongestElement(IEnumerable<Element> elements)
    {
        var strongest = elements.First();
        foreach (var element in elements)
        {
            if (CompareElements(strongest, element) < 0)
                strongest = element;
        }
        return strongest;
    }
}

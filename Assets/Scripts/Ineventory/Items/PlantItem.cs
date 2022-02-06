using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Plant Item", menuName = "items/plant")]
public class PlantItem : ScriptableObject, IChemicalComponent
{
    [SerializeField]
    private string Name;

    [SerializeField]
    private ColorController color;

    public ColorController Color => color;

    [SerializeField]
    private Sprite Icon;

    [TextArea]
    [SerializeField]
    private string Description;

    [SerializeField]
    private ChemicalProperty property;

    private Action<IItem> onReplace;

    public ChemicalProperty chemicalProperty => property;

    [SerializeField]
    private int TempID;
    public int ID { get { return TempID; } set { TempID = value; } }

    public string ItemName => Name;

    public IItem Copy()
    {
        var copy = CreateInstance<PlantItem>();
        copy.color = color;
        copy.property = property;
        copy.ID = ID;
        copy.Name = ItemName;
        copy.Description = Description;
        copy.Icon = Icon;
        return copy;
    }

    public void FillDescription(IDescription description)
    {
        description.SetName(Name);
        description.SetMainImage(Icon);
        description.SetDescription(Description);
        chemicalProperty.FillDescription(description);
    }

    public Sprite GetIcon()
    {
        return Icon;
    }

    public Material GetMaterial()
    {
        return null;
    }

    public void SetReplaceFunction(Action<IItem> onReplace)
    {
        this.onReplace = onReplace;
    }
}

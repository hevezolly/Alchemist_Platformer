using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChemicalComponent: IItem
{
    ChemicalProperty chemicalProperty { get; }
    ColorController Color { get; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new chemical property", menuName = "alchemy/chemical property")]
public class ChemicalProperty : ScriptableObject
{
    [SerializeField]
    private PotionEffect mainEffect; 
    [SerializeField]
    private PotionBase potionBase;

    private PotionEffect copiedEffect;
    public Element element;
    public PotionEffect MainEffect => copiedEffect.Copy();
    public PotionBase PotionBase => potionBase;

    public void FillDescription(IDescription description)
    {
        description.SetElement(element);
        mainEffect.FillDescription(description);
        potionBase.FillDescription(description);
    }

    public void SetUp(ColorController color)
    {
        copiedEffect = mainEffect.Copy();
        copiedEffect.colors = color;
        copiedEffect.element = element;
        potionBase.element = element;
    }
}

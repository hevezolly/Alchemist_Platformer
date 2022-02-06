using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new standart modifier", menuName = "alchemy/aspects modifyers/standart")]
public class StandartModyfier : AspectModifyer
{
    public PropertyAspect aspectType;
    public ModificationType strength;

    public override void Modify(PotionEffect effect, Element baseElement)
    {
        var type = strength;
        if (EnumController.GetAntipode(effect.element) == baseElement)
            type = EnumController.RegradeModification(type);
        else if (effect.element == baseElement)
            type = EnumController.UpgradeModification(type);
        effect.BaseProperty.ModifyProperty(aspectType, type);
    }

    public override void SetDescription(IDescription description)
    {
        description.AddBase(EnumController.GetModificationDescription(strength),
            aspectType.parameterName,
            aspectType.icon);
    }
}

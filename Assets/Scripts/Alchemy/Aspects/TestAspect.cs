using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new test aspect", menuName = "alchemy/aspects/test")]
public class TestAspect : PropertyAspect
{
    public override PropertyAspect Modify(ModificationType type)
    {
        Debug.Log(Name(type) + " modification was applied");
        return CreateInstance<TestAspect>();
    }

    public override void FillDescription(IDescription description)
    {
        description.AddState(icon, parameterName);
    }

    private string Name(ModificationType type)
    {
        switch (type)
        {
            case ModificationType.BigNegative:
                return "Big Negative";
            case ModificationType.LesserNegative:
                return "Lesser Negative";
            case ModificationType.StandartNegative:
                return "Standart Negative";
            case ModificationType.Nutral:
                return "Nutral";
            case ModificationType.BigPositive:
                return "Big Positive";
            case ModificationType.StandartPositive:
                return "Standart Positive";
            case ModificationType.LesserPositive:
                return "Lesser Positive";
        }
        throw new System.ArgumentException();
    }
}

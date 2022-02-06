using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    IItem Copy();

    int ID { get; set; }

    void FillDescription(IDescription description);

    Sprite GetIcon();

    Material GetMaterial();

    void SetReplaceFunction(System.Action<IItem> onReplace);
}

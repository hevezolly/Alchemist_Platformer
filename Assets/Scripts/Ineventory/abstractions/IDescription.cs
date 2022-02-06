using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDescription
{
    void SetEffect(string effect);
    void SetName(string name);
    void SetMainImage(Sprite sprite, Material mat = null);
    void SetElement(Element element);
    void SetDescription(string description);
    void AddBase(string descr, string iconName = "", Sprite icon = null);
    void AddBase();
    void AddState(Sprite icon, string iconName, string value = null);
    IDescription AddDescription();
}

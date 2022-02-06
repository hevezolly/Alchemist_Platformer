using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PotionEffectDescription : MonoBehaviour, IDescription
{
    public TextMeshProUGUI Description;
    public Transform StateHolder;
    public GameObject singleState;
    public void AddBase(string descr, string iconName = "", Sprite icon = null)
    {
        return;
    }

    public void AddBase()
    {
        return;
    }

    public IDescription AddDescription()
    {
        return null;
    }

    public void AddState(Sprite icon, string iconName, string value = null)
    {
        StateHolder.gameObject.SetActive(true);
        var stateObj = Instantiate(singleState, StateHolder);
        var image = stateObj.transform.Find("Icon").GetComponent<Image>();
        var icontext = image.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        icontext.text = iconName;
        image.sprite = icon;
        var desc = stateObj.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        if (value != null)
        {
            desc.text = " - " + value;
        }
        else
            desc.text = "";
    }

    public void SetDescription(string description)
    {
        Description.text = description;
    }

    public void SetEffect(string effect)
    {
        Description.text = effect;
    }

    public void SetElement(Element element)
    {
        return;
    }

    public void SetMainImage(Sprite sprite, Material mat = null)
    {
        return;
    }

    public void SetName(string name)
    {
        return;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DescriptionController : MonoBehaviour, IPointerExitHandler, IPointerDownHandler, IDescription
{
    public TextMeshProUGUI Name;
    public Image mainImage;
    public Image ElementBlank;
    public Image ElementColorfool;
    public TextMeshProUGUI ElementName;
    public TextMeshProUGUI Description;
    public Transform States;
    public GameObject singleState;
    public TextMeshProUGUI Effect;
    public Transform BaseModifiers;
    public GameObject singleModifyer;
    public GameObject NoProperties;
    public GameObject AdditionalDescription;
    public Transform AdditionalDescriptionHolder;
    public moreDescriptionButton button;
    public GameObject MoreDescriptionObject;
    // Start is called before the first frame update

    public void Clear()
    {
        Name.text = "без названия";
        mainImage.sprite = null;
        ElementBlank.sprite = null;
        ElementBlank.gameObject.SetActive(false);
        Description.text = "";
        Description.gameObject.SetActive(false);
        Effect.transform.parent.gameObject.SetActive(false);
        while (States.childCount > 0)
        {
            var child = States.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        States.gameObject.SetActive(false);
        while (BaseModifiers.childCount > 0)
        {
            var child = BaseModifiers.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        BaseModifiers.parent.gameObject.SetActive(false);
        AdditionalDescription.SetActive(false);
        button.ResetClick();
        while (AdditionalDescriptionHolder.childCount > 0)
        {
            var child = AdditionalDescriptionHolder.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }

    void Start()
    {
        Clear();
        gameObject.SetActive(false);
    }

    public void Place(Transform item)
    {
        var t = (RectTransform)transform;
        transform.position = item.position;
    }

    private void Update()
    {
        var t = (RectTransform)transform;
        var pos = t.position;
        pos.x = Mathf.Max(t.sizeDelta.x * t.pivot.x * t.parent.localScale.x, pos.x);
        pos.y = Mathf.Max(t.sizeDelta.y * t.pivot.y * t.parent.localScale.y, pos.y);
        t.position = pos;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Clear();
        gameObject.SetActive(false);
    }

    public void SetEffect(string effect)
    {
        Effect.transform.parent.gameObject.SetActive(true);
        Effect.text = effect;
        Canvas.ForceUpdateCanvases();
    }

    public void SetName(string name)
    {
        Name.text = name;
        Canvas.ForceUpdateCanvases();
    }

    public void SetMainImage(Sprite sprite, Material mat = null)
    {
        mainImage.sprite = sprite;
        if (mat != null)
            mainImage.material = mat;
        Canvas.ForceUpdateCanvases();
    }

    public void SetElement(Element element)
    {
        ElementBlank.gameObject.SetActive(true);
        ElementBlank.sprite = EnumController.GetElementImage(element, false);
        ElementColorfool.sprite = EnumController.GetElementImage(element, true);
        ElementName.text = EnumController.GetElementName(element);
        Canvas.ForceUpdateCanvases();
    }

    public void SetDescription(string description)
    {
        Description.gameObject.SetActive(true);
        Description.text = description;
        Canvas.ForceUpdateCanvases();
    }

    public IDescription AddDescription()
    {
        AdditionalDescription.SetActive(true);
        var obj = Instantiate(MoreDescriptionObject, AdditionalDescriptionHolder); 
        return obj.GetComponent<IDescription>();
        Canvas.ForceUpdateCanvases();
    }

    public void AddBase(string descr, string iconName = "", Sprite icon = null)
    {
        NoProperties.SetActive(false);
        BaseModifiers.gameObject.SetActive(true);
        BaseModifiers.parent.gameObject.SetActive(true);
        var modObj = Instantiate(singleModifyer, BaseModifiers);
        var image = modObj.transform.Find("Icon").GetComponent<Image>();
        var iconText = image.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        var text = modObj.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        if (icon != null)
        {
            image.sprite = icon;
            iconText.text = iconName;
        }
        else
            image.gameObject.SetActive(false);

        text.text = descr;
        Canvas.ForceUpdateCanvases();
    }

    public void AddBase()
    {
        BaseModifiers.gameObject.SetActive(false);
        NoProperties.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    public void AddState(Sprite icon, string iconName, string value = null)
    {
        States.gameObject.SetActive(true);
        var stateObj = Instantiate(singleState, States);
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
        Canvas.ForceUpdateCanvases();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Clear();
        gameObject.SetActive(false);
    }
}

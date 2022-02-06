using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DescrShower : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject Data;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Data.SetActive(true);
        Canvas.ForceUpdateCanvases();
        Data.SetActive(false);
        Data.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Data.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Data.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItem : MonoBehaviour, 
    IBeginDragHandler, 
    IEndDragHandler, 
    IDragHandler,
    IPointerClickHandler
{
    private PlayerInventory inventory;
    private Vector2 offset;
    public IItem item { get; private set; }
    private DescriptionController description;
    public float stillTime;
    private bool isInDrag;
    private bool mouseEntered;
    private float lastMoveMoment;
    private Image image;
    public void SetUp(PlayerInventory inventory, DescriptionController desc, IItem item)
    {
        image = GetComponent<Image>();
        var mat = item.GetMaterial();
        if (mat != null)
            image.material = mat;
        image.sprite = item.GetIcon();
        this.inventory = inventory;
        description = desc;
        this.item = item;
    }

    public void Update()
    {
        if (image.sprite != item.GetIcon())
            image.sprite = item.GetIcon();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isInDrag)
        {
            isInDrag = false;
            inventory.OnDragExit(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!inventory.IsOpened)
            return;
        isInDrag = true;
        offset = transform.position - Input.mousePosition;
        transform.SetSiblingIndex(transform.parent.childCount - 1);
        
    }

    private void ShowDescription()
    {
        description.gameObject.SetActive(true);
        item.FillDescription(description);
        description.Place(transform);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isInDrag)
            transform.position = (Vector2)Input.mousePosition + offset;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEntered = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInDrag && inventory.IsOpened)
            ShowDescription();
    }
}

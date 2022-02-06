using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class HotBarController : MonoBehaviour
{
    public float desapireTime;
    public float waitTime;

    [Space]
    public float StartAlpha;
    [Space]
    public Transform Slots;
    public Transform Items;
    [Space]
    public GameObject SelectionIcon;

    private Sequence desapire;
    private bool[] occupiedCords;
    private int selected = -1;

    private InventoryCellsController cells;
    private IInventory inventory;
    // Start is called before the first frame update

    // Update is called once per frame

    private void Start()
    {
        cells = GetComponent<InventoryCellsController>();
        inventory = GetComponent<PlayerInventory>().inventory;
        HideHotBar();
        occupiedCords = new bool[Slots.childCount];
        SelectionIcon.SetActive(false);
    }

    public void AddCord(int index)
    {
        occupiedCords[index] = true;
        if (selected == -1)
            SetSelected(index);
    }

    public void TrySetSelected(int index)
    {
        RestartSequance();
        if (!occupiedCords[index])
            return;
        SetSelected(index);
    }

    private void SetSelected(int index)
    {
        SelectionIcon.SetActive(true);
        SelectionIcon.transform.position =
            cells.GetCell(new InventoryCellCord(index, CellType.HotBar)).position;
        selected = index;
    }

    public void RemoveCord(int index)
    {
        occupiedCords[index] = false;
        if (occupiedCords.All(c => !c))
        {
            selected = -1;
            SelectionIcon.SetActive(false);
        }
        else if (selected == index)
        {
            for (var i = 0; i < occupiedCords.Length; i++)
            {
                if (occupiedCords[i])
                {
                    SetSelected(i);
                    return;
                }
            }
        }
    }

    public IItem GetItem()
    {
        if (selected == -1)
            return null;
        return inventory.GetItem(new InventoryCellCord(selected, CellType.HotBar));
    }

    public void RestartSequance()
    {
        ShowHotBar();
        desapire = DOTween.Sequence();
        desapire.AppendInterval(waitTime);
        desapire.AppendInterval(0);
        for (var i = 0; i < Slots.childCount; i++)
        {
            var image = Slots.GetChild(i).GetComponent<Image>();
            desapire.Join(DOTween.To(() => image.color, (c) => image.color = c,
                new Color(image.color.r, image.color.g, image.color.b, 0), desapireTime));
        }
        for (var i = 0; i < Items.childCount; i++)
        {
            var image = Items.GetChild(i).GetComponent<Image>();
            desapire.Join(DOTween.To(() => image.color, (c) => image.color = c,
                new Color(image.color.r, image.color.g, image.color.b, 0), desapireTime));
        }
        var select = SelectionIcon.GetComponent<Image>();
        desapire.Join(DOTween.To(() => select.color, (c) => select.color = c,
            new Color(select.color.r, select.color.g, select.color.b, 0), desapireTime));
        desapire.Play();
    }

    public void ShowHotBar()
    {
        if (desapire != null)
            desapire.Kill();
        for (var i = 0; i < Slots.childCount; i++)
        {
            var image = Slots.GetChild(i).GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b,
                StartAlpha);
        }
        for (var i = 0; i < Items.childCount; i++)
        {
            var image = Items.GetChild(i).GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b,
                1);
        }
        var select = SelectionIcon.GetComponent<Image>();
        select.color = new Color(select.color.r, select.color.g, select.color.b, 1);
    }

    public void HideHotBar()
    {
        if (desapire != null)
            desapire.Kill();
        for (var i = 0; i < Slots.childCount; i++)
        {
            var image = Slots.GetChild(i).GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b,
                0);
        }
        for (var i = 0; i < Items.childCount; i++)
        {
            var image = Items.GetChild(i).GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b,
                0);
        }
        var select = SelectionIcon.GetComponent<Image>();
        select.color = new Color(select.color.r, select.color.g, select.color.b, 0);
    }
}

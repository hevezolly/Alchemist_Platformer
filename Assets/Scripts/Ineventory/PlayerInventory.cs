using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private AbstractPotionCreator potionCreator;

    
    public AbstractInventory inventory;

    public float CookTime;

    public GameObject UiObject;

    public Canvas HidingInventory;

    public Transform HidingItemsHolder;
    public Transform ShowingItemsHolder;

    public float maxDistanceToPlaceInCell;

    private ModulatedPlayerMovement movement;

    private Dictionary<IItem, UIItem> ToUIItem;

    public CookingAnimationActivator activator;

    public DescriptionController description;

    public Bottle bottleToAdd;

    private InventoryCellsController cells;
    private HotBarController hotBar;

    public bool IsOpened => GetComponent<Canvas>().enabled;

    private void Start()
    {
        cells = GetComponent<InventoryCellsController>();
        hotBar = GetComponent<HotBarController>();
        inventory.Clear();
        potionCreator.SetUp();
        cells.SetUp(inventory.Dimentions(CellType.Inventory), 
            -inventory.Dimentions(CellType.Potions).x, 
            inventory.Dimentions(CellType.Potions).y,
            inventory.Dimentions(CellType.HotBar).y + 1);
        ToUIItem = new Dictionary<IItem, UIItem>();
        StartCoroutine(PlaceItems());
        HidingInventory.enabled = false;
    }

    IEnumerator PlaceItems()
    {
        yield return new WaitForSeconds(1);
        for (var i = 0; i < 5; i++)
        {
            TryAddItem(bottleToAdd.Copy());
        }
    }

    public void Turn(bool on)
    {
        HidingInventory.enabled = on;
        if (on)
            hotBar.ShowHotBar();
        else
            hotBar.RestartSequance();
    }

    public bool TryAddItem(IItem item)
    {
        var cell = inventory.GetEmptyCell();
        if (cell == null)
            return false;
        var uiObj = Instantiate(UiObject, HidingItemsHolder);
        uiObj.transform.position = cells.GetCell(cell).position;
        uiObj.transform.localScale = Vector3.one;
        var uitem = uiObj.GetComponent<UIItem>();
        uitem.SetUp(this, description, item);
        inventory.PlaceItem(cell, item);
        ToUIItem[item] = uitem;
        item.SetReplaceFunction(o => ReplaceItem(item, o));
        return true;
    }

    public void OnDragExit(UIItem target)
    {
        var position = target.transform.position;
        var resultCell = cells.GetCord(position, maxDistanceToPlaceInCell);
        var startCell = inventory.GetCell(target.item);
        if (resultCell == null || 
            (!IsOpened && resultCell.CellType != CellType.HotBar))
        {
            ResetPosition(target);
            return;
        }
        var result = inventory.MoveItem(startCell, resultCell);
        if (result == MoveResult.None)
            ResetPosition(target);
        if (result == MoveResult.Stack)
        {
            if (startCell.CellType == CellType.HotBar)
                hotBar.RemoveCord(startCell.NotStandartCord);
            ToUIItem.Remove(target.item);
            Destroy(target);
            return;
        }
        else if (result == MoveResult.Swap)
        {
            var otherItem = ToUIItem[inventory.GetItem(startCell)];
            ResetPosition(otherItem);
            ResetPosition(target);
        }
        else if (result == MoveResult.Move)
        {
            if (startCell.CellType == CellType.HotBar)
                hotBar.RemoveCord(startCell.NotStandartCord);
            if (resultCell.CellType == CellType.HotBar)
                hotBar.AddCord(resultCell.NotStandartCord);
            ResetPosition(target);
        }
    }

    public void ReplaceItem(IItem item, IItem other)
    {
        RemoveItem(item);
        if (other != null)
            TryAddItem(other);
    }

    public void RemoveItem(IItem item)
    {
        var uiitem = ToUIItem[item];
        var cell = inventory.GetCell(item);
        if (cell.CellType == CellType.HotBar)
            hotBar.RemoveCord(cell.NotStandartCord);
        inventory.RemoveItem(cell);
        ToUIItem.Remove(item);
        Destroy(uiitem.gameObject);
    }


    public void Cook()
    {
        var itemsToClear = new List<IItem>();
        for (var i = inventory.Dimentions(CellType.Potions).x; i <= inventory.Dimentions(CellType.Potions).y; i++)
        {
            var cord = new InventoryCellCord(i, CellType.Potions);
            var item = inventory.GetItem(cord);
            if (item == null)
                continue;
            if (i == 0)
                potionCreator.SetBottle(item as IBottle);
            else if (i > 0)
            {
                potionCreator.AddEffectComponent(item as IChemicalComponent);
                itemsToClear.Add(item);
            }
            else
            {
                potionCreator.AddBaseComponent(item as IChemicalComponent);
                itemsToClear.Add(item);
            }
        }
        if (!potionCreator.CanCook)
        {
            potionCreator.Clear();
            activator.CookFail();
        }
        else
        {
            
            potionCreator.Cook();
            var colors = new Dictionary<InventoryCellCord, Texture2D>();
            foreach (var item in itemsToClear)
            {
                var cord = inventory.GetCell(item);
                var componentColors = ((IChemicalComponent)inventory.GetItem(cord)).Color.colors;

                colors[cord] = ColorController.CombineColors(componentColors);
                inventory.RemoveItem(cord);
                Destroy(ToUIItem[item].gameObject);
            }
            activator.CookSuccessfull(colors, CookTime);
        }
    }

    private void ResetPosition(UIItem target)
    {
        var destination = inventory.GetCell(target.item);
        if (destination.CellType == CellType.HotBar)
            target.transform.parent = ShowingItemsHolder;
        else
            target.transform.parent = HidingItemsHolder;
        target.transform.position = cells.GetCell(destination).position;
    }
}

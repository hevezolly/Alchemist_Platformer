using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new basic inventory", menuName = "game/baseinventory")]
public class BaseInventory : AbstractInventory
{
    [SerializeField]
    private Vector2Int cellsWidthHeight;
    [SerializeField]
    private Vector2Int minMaxPotions;
    [SerializeField]
    private int HotBarLength;



    private Dictionary<InventoryCellCord, IItem> items;
    private Dictionary<IItem, InventoryCellCord> itemsToCords;

    public override void Clear()
    {
        items = new Dictionary<InventoryCellCord, IItem>();
        itemsToCords = new Dictionary<IItem, InventoryCellCord>();
    }

    public override InventoryCellCord GetCell(IItem item)
    {
        foreach (var cord in items.Keys)
        {
            if (!items.ContainsKey(cord))
                continue;
            if (items[cord] == item)
                return cord;
        }
        return null;
    }

    public override Vector2Int Dimentions(CellType type)
    {
        switch (type)
        {
            case CellType.Inventory:
                return cellsWidthHeight;
            case CellType.HotBar:
                return new Vector2Int(0, HotBarLength - 1);
            case CellType.Potions:
                return minMaxPotions;
            default:
                throw new System.ArgumentException("incorrect type");
        }
    }

    public override InventoryCellCord GetEmptyCell(IItem item = null)
    {
        for (var y = 0; y < Dimentions(CellType.Inventory).x; y++)
        {
            for (var x = 0; x < Dimentions(CellType.Inventory).y; x++)
            {
                var cord = new InventoryCellCord(x, y);
                if (!ContainsItemAt(cord))
                    return cord;
            }
        }
        return null;
    }

    public override IItem GetItem(InventoryCellCord index)
    {
        if (!ContainsItemAt(index))
            return null;
        return items[index];
    }

    public override bool IsItemFitForCell(InventoryCellCord cord, IItem item)
    {
        ContainsItemAt(cord);
        if (cord.CellType == CellType.Inventory)
            return true;
        if (cord.CellType == CellType.Potions)
        {
            if (cord.NotStandartCord == 0)
            {
                var bottle = item as IBottle;
                return bottle != null && !bottle.Filled;
            }
            return (item as IChemicalComponent) != null;
        }
        var throwable = item as IThrowable;
        var drinkable = item as IDrinkable;
        return (throwable != null && throwable.CanBeThrown) ||
            (drinkable != null && drinkable.CanBeDrinked);
    }

    public override MoveResult MoveItem(InventoryCellCord from, InventoryCellCord to)
    {
        if (!ContainsItemAt(from))
            return MoveResult.None;
        var fromItem = items[from];
        if (!IsItemFitForCell(to, fromItem))
            return MoveResult.None;
        if (ContainsItemAt(to))
        {
            var toItem = items[to];
            if (!IsItemFitForCell(from, toItem))
                return MoveResult.None;
            items[from] = toItem;
            items[to] = fromItem;
            itemsToCords[fromItem] = to;
            itemsToCords[toItem] = from;
            return MoveResult.Swap;
        }
        items[to] = fromItem;
        itemsToCords[fromItem] = to;
        items.Remove(from);
        return MoveResult.Move;
    }

    public override void PlaceItem(InventoryCellCord index, IItem item)
    {
        if (ContainsItemAt(index))
            throw new System.ArgumentException("already contains item at thouse cords");
        items[index] = item;
        itemsToCords[item] = index;
    }

    public override IItem RemoveItem(InventoryCellCord index)
    {
        if (!ContainsItemAt(index))
            return null;
        var item = items[index];
        items.Remove(index);
        itemsToCords.Remove(item);
        return item;
    }

    private bool ContainsItemAt(InventoryCellCord cord)
    {
        if ((cord.CellType == CellType.Inventory && (
            cord.StandartCord.x < 0 ||
            cord.StandartCord.x >= Dimentions(CellType.Inventory).x ||
            cord.StandartCord.y < 0 ||
            cord.StandartCord.y >= Dimentions(CellType.Inventory).y)) ||
            (cord.CellType == CellType.Potions && (
            cord.NotStandartCord < Dimentions(CellType.Potions).x ||
            cord.NotStandartCord > Dimentions(CellType.Potions).y)) ||
            (cord.CellType == CellType.HotBar && (
            cord.NotStandartCord < Dimentions(CellType.HotBar).x ||
            cord.NotStandartCord > Dimentions(CellType.HotBar).y)))
            throw new System.ArgumentException("incorrect Cell");
        return items.ContainsKey(cord) && items[cord] != null;
    }
}

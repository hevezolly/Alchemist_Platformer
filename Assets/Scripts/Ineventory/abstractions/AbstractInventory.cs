using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractInventory : ScriptableObject, IInventory
{

    public abstract void Clear();
    public abstract InventoryCellCord GetCell(IItem item);
    public abstract Vector2Int Dimentions(CellType type);
    public abstract InventoryCellCord GetEmptyCell(IItem item = null);
    public abstract IItem GetItem(InventoryCellCord index);
    public abstract bool IsItemFitForCell(InventoryCellCord cord, IItem item);
    public abstract MoveResult MoveItem(InventoryCellCord from, InventoryCellCord to);
    public abstract void PlaceItem(InventoryCellCord index, IItem item);
    public abstract IItem RemoveItem(InventoryCellCord index);
}

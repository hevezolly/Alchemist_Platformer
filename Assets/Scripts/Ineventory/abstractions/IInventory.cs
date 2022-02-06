using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventory
{
    Vector2Int Dimentions(CellType type);

    IItem GetItem(InventoryCellCord index);

    IItem RemoveItem(InventoryCellCord index);

    void PlaceItem(InventoryCellCord index, IItem item);

    MoveResult MoveItem(InventoryCellCord from, InventoryCellCord to);

    InventoryCellCord GetCell(IItem item);

    InventoryCellCord GetEmptyCell(IItem item = null);

    bool IsItemFitForCell(InventoryCellCord cord, IItem item);
}

public enum MoveResult
{
    None,
    Move,
    Swap,
    Stack
}

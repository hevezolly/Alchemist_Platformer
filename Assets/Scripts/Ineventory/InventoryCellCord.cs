using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCellCord
{
    public Vector2Int StandartCord { get; private set; }
    public CellType CellType { get; private set; }
    public int NotStandartCord { get; private set; }

    public InventoryCellCord(int cord, CellType type)
    {
        if (type == CellType.Inventory)
            throw new System.ArgumentException("incorrect type");
        CellType = type;
        NotStandartCord = cord;
    }

    public InventoryCellCord(Vector2Int cord)
    {
        CellType = CellType.Inventory;
        StandartCord = cord;
    }

    public InventoryCellCord(int x, int y)
    {
        CellType = CellType.Inventory;
        StandartCord = new Vector2Int(x, y);
    }

    public override int GetHashCode()
    {
        if (CellType == CellType.Inventory)
            return StandartCord.GetHashCode();
        return (997 * 997 * (int)CellType + 997 * NotStandartCord).GetHashCode();
    }

    public override bool Equals(object obj)
    {
        var other = obj as InventoryCellCord;
        if (other == null)
            return false;
        if (other.CellType != CellType)
            return false;
        return (CellType == CellType.Inventory && (other.StandartCord.Equals(StandartCord))) ||
            (CellType != CellType.Inventory && (other.NotStandartCord.Equals(NotStandartCord)));
    }

    public static implicit operator InventoryCellCord(Vector2Int cord)
    {
        return new InventoryCellCord(cord);
    }
}

public enum CellType
{
    Inventory,
    Potions,
    HotBar
}

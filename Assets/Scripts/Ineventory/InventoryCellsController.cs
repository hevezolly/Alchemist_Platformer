using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCellsController : MonoBehaviour
{
    private Vector2Int widthHeight;
    private int maxBases;
    private int maxEffects;
    private int maxHotBar;

    public Transform inventory;

    public Transform effects;
    public Transform bottle;
    public Transform bases;

    public Transform hotBar;

    public void SetUp(Vector2Int widthHeight, int maxBases, int maxEffects, int maxHotBar)
    {
        this.widthHeight = widthHeight;
        this.maxBases = maxBases;
        this.maxEffects = maxEffects;
        this.maxHotBar = maxHotBar;
        if (maxBases != bases.childCount || 
            inventory.childCount != widthHeight.x * widthHeight.y ||
            maxEffects != effects.childCount ||
            maxHotBar != hotBar.childCount)
            throw new System.ArgumentException("something wrong with inventory cells");
    }

    public Transform GetCell(InventoryCellCord cord)
    {
        if (!InRange(cord))
            return null;
        if (cord.CellType == CellType.Inventory)
        {
            return inventory.GetChild(cord.StandartCord.x +
                cord.StandartCord.y * widthHeight.x);
        }
        if (cord.CellType == CellType.Potions)
        {
            if (cord.NotStandartCord == 0)
                return bottle;
            if (cord.NotStandartCord < 0)
                return bases.GetChild(-cord.NotStandartCord - 1);
            return effects.GetChild(cord.NotStandartCord - 1);
        }
        if (cord.CellType == CellType.HotBar)
        {
            return hotBar.GetChild(cord.NotStandartCord);
        }
        return null;

    }

    public InventoryCellCord GetCord(Vector2 position, float maxDistance)
    {
        Vector2Int? cell = null;
        var distance = maxDistance;
        for (var x = 0; x < widthHeight.x; x++)
        {
            for (var y = 0; y < widthHeight.y; y++)
            {
                var current = new Vector2Int(x, y);
                var currentDist = Vector2.Distance(GetCell(current).position, position);
                if (currentDist < distance)
                {
                    distance = currentDist;
                    cell = current;
                }
            }
        }
        if (cell != null)
            return new InventoryCellCord(cell.Value);
        if (Vector2.Distance(bottle.position, position) < maxDistance)
            return new InventoryCellCord(0, CellType.Potions);
        for (var i = 0; i < bases.childCount; i++)
        {
            if (Vector2.Distance(bases.GetChild(i).position, position) < maxDistance)
                return new InventoryCellCord(-i -1, CellType.Potions);
        }
        for (var i = 0; i < effects.childCount; i++)
        {
            if (Vector2.Distance(effects.GetChild(i).position, position) < maxDistance)
                return new InventoryCellCord(i + 1, CellType.Potions);
        }
        for (var i = 0; i < hotBar.childCount; i++)
        {
            if (Vector2.Distance(hotBar.GetChild(i).position, position) < maxDistance)
                    return new InventoryCellCord(i, CellType.HotBar);
        }
        return null;
    }

    private bool InRange(InventoryCellCord cord)
    {
        if (cord.CellType == CellType.Inventory)
        {
            return (cord.StandartCord.x >= 0 && cord.StandartCord.x < widthHeight.x &&
                cord.StandartCord.y >= 0 && cord.StandartCord.y < widthHeight.y);
        }
        if (cord.CellType == CellType.Potions)
        {
            return -bases.childCount <= cord.NotStandartCord &&
                cord.NotStandartCord <= effects.childCount;
        }
        if (cord.CellType == CellType.HotBar)
        {
            return 0 <= cord.NotStandartCord &&
                cord.NotStandartCord < hotBar.childCount;
        }
        return false;
    }
}

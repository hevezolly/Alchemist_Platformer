using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new vector field", menuName = "game/move options/vector field")]
public class VectorFieldOption : MoveOption
{
    [Header("Vector field")]
    private int cellResolution;
    public Vector2 playerSize;
    private Vector2Int cellCount;
    private VectorField field;

    private VectorCell[,] cells;
    private Dictionary<Vector2Int, VectorCell> activeCells;

    private Vector2Int previusCord;
    public override void SetUp(GameObject target, JumpOption jump)
    {
        base.SetUp(target, jump);
        var map = GameObject.FindGameObjectWithTag("GameController");
        if (map == null)
            return;
        field = map.GetComponent<VectorField>();
        cellResolution = field.CellResolution;
    }

    public override void NeccessaryUpdate()
    {
        if (field == null)
            return;
        foreach (var cord in GetPlayerArea())
        {
            field.ModifyCell(cord, rb.velocity);
        }
    }

    private IEnumerable<Vector2Int> GetPlayerArea()
    {
        for (var x = -0.5f; x <= 1f; x+= 1f / cellResolution)
        {
            for (var y = -0.5f; y <= 1f; y += 1f / cellResolution)
            {
                var pos = rb.position + new Vector2(x * playerSize.x, y * playerSize.y);
                yield return positionToCord(pos);
            }
        }
    }

    private Vector2Int positionToCord(Vector2 position)
    {
        return Vector2Int.FloorToInt(position * cellResolution);
    }

    public override bool IsConditionsReached()
    {
        return false;
    }
}

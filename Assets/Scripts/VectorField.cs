using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainGenerator))]
public class VectorField : MonoBehaviour
{
    public float moveAmount;
    public int CellResolution;
    public float maxVelocity;
    public GridControlller grid;
    public float mass;
    public float k;
    public float friction;
    private Texture2D vectorTexture;

    private VectorCell[,] cells;
    private Dictionary<Vector2Int, VectorCell> activeCells;

    private Vector2Int previusCord;
    // Start is called before the first frame update

    public void SetTextures()
    {
        var map = GetComponent<TerrainGenerator>();
        vectorTexture = new Texture2D(map.WidthHeight.x * CellResolution, map.WidthHeight.y * CellResolution);
        grid.SetGrasFloat("_perUnit", CellResolution);
        grid.SetGrassVector("_DisplacementResolution", new Vector4(vectorTexture.width, vectorTexture.height));
        grid.SetGrassVector("_Resolution", new Vector4(map.WidthHeight.x, map.WidthHeight.y));
        grid.SetGrasFloat("_MoveAmount", moveAmount);
        cells = new VectorCell[vectorTexture.width, vectorTexture.height];
        activeCells = new Dictionary<Vector2Int, VectorCell>();
        for (var x = 0; x < vectorTexture.width; x++)
        {
            for (var y = 0; y < vectorTexture.height; y++)
            {
                vectorTexture.SetPixel(x, y, DirectionToColor(Vector2.zero));
                cells[x, y] = new VectorCell();
            }
        }
        vectorTexture.Apply();
    }

    public void ModifyCell(Vector2Int cord, Vector2 velocity)
    {
        if (!activeCells.ContainsKey(cord))
        {
            var currentCell = cells[cord.x, cord.y];
            currentCell.Move(velocity);
            activeCells[cord] = currentCell;
        }
    }

    private Color DirectionToColor(Vector2 dir)
    {
        if (dir.magnitude > maxVelocity)
            dir = dir.normalized;
        else
            dir = dir / maxVelocity;
        dir += Vector2.one;
        dir /= 2;
        return new Color(dir.x, dir.y, 0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var cellsToRemove = new List<Vector2Int>();
        foreach (var activeCord in activeCells.Keys)
        {
            if (!activeCells[activeCord].Update(k, mass, friction))
            {
                cellsToRemove.Add(activeCord);
            }
            vectorTexture.SetPixel(activeCord.x, activeCord.y,
                DirectionToColor(activeCells[activeCord].state));
        }
        foreach (var cordToDelete in cellsToRemove)
        {
            activeCells.Remove(cordToDelete);
        }
        vectorTexture.Apply();
        grid.SetGrassTexture("_Displacement", vectorTexture);
    }

    private void OnDrawGizmos()
    {
        if (cells == null)
            return;
        Gizmos.color = Color.green;
        for (var x = 0; x < vectorTexture.width; x++)
        {
            for (var y = 0; y < vectorTexture.height; y++)
            {
                var origin = new Vector2(x, y) / CellResolution;
                Gizmos.DrawLine(origin, origin + cells[x, y].state / maxVelocity);
            }
        }
    }
}


public class VectorCell
{
    public bool isInMotion { get; private set; }
    public Vector2 state { get; private set; }
    public Vector2 velocity { get; private set; }
    private float k;
    private float m;

    public VectorCell()
    {
        isInMotion = false;
    }

    public bool Update(float k, float m, float friction)
    {
        state += velocity;
        velocity -= state * k / m;
        velocity -= velocity * friction;
        if (velocity.magnitude < 0.1f && state.magnitude < 0.1f)
        {
            velocity = Vector2.zero;
            state = Vector2.zero;
            isInMotion = false;
        }
        return isInMotion;
    }

    public void Move(Vector2 velocity)
    {
        this.velocity += velocity;
        isInMotion = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundColorSetter : MonoBehaviour
{
    public Tilemap tilemap;
    private Transform player;
    private BackgroundColor color;
    public void Initiate(BackgroundColor color, Transform player)
    {
        this.player = player;
        this.color = color;
    }

    private void LateUpdate()
    {
        tilemap.color = color.GetBackgrpundColor(player.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class PIckupable : MonoBehaviour
{
    private IItem item;
    public void SetUp(IItem item)
    {
        this.item = item;
    }

    public void PickUp(PlayerInventory inventory)
    {
        var succes = inventory.TryAddItem(item);
        if (succes)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var c = transform.GetChild(i);
                
                var light = c.GetComponent<Light2D>();
                if (light == null)
                    continue;
                c.parent = null;
                var s = DOTween.Sequence();
                s.Append(DOTween.To(() => light.intensity, f => light.intensity = f, 0, 1));
                s.AppendCallback(() => Destroy(c.gameObject));
            }
            Destroy(gameObject);
        }
    }
}

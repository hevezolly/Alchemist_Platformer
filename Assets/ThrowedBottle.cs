using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class ThrowedBottle : MonoBehaviour
{
    public float radius;
    private PotionActivator activator;

    public void SetUp(PotionActivator activator)
    {
        this.activator = activator;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var aplication = new AreaApplication();
        aplication.position = transform.position;
        aplication.radius = radius;
        activator.ApplyOnArea(aplication);
        for (var i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            c.parent = null;
            var light = c.GetComponent<Light2D>();
            var s = DOTween.Sequence();
            s.Append(DOTween.To(() => light.intensity, f => light.intensity = f, 0, 1));
            s.AppendCallback(() => Destroy(c.gameObject));
        }
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using System.Linq;

[CreateAssetMenu(fileName = "new standart bottle", menuName = "items/bottles/standart")]
public class StandartBottle : Bottle, IDrinkable, IThrowable
{
    public string ItemName;
    [TextArea]
    public string Description;
    public Sprite sprite;
    public Sprite throwedSprite;
    public Material mat;
    public float throwForce;
    public GameObject Throwable;
    private StandartBottle initialObject;
    private System.Random random;

    public bool CanBeDrinked => Filled;

    public bool CanBeThrown => Filled;

    public override IItem Copy()
    {
        var copy = CreateInstance<StandartBottle>();
        copy.ItemName = ItemName;
        copy.Description = Description;
        copy.sprite = sprite;
        copy.throwedSprite = throwedSprite;
        copy.Throwable = Throwable;
        copy.throwForce = throwForce;
        if (mat != null)
            copy.mat = new Material(mat);
        else
            copy.mat = null;
        copy.random = new System.Random(Random.Range(0, 9999999));
        if (initialObject == null)
            copy.initialObject = this;
        else
            copy.initialObject = initialObject;
        return copy;
    }

    public override void FillDescription(IDescription description)
    {
        description.SetName(ItemName);
        description.SetDescription(Description);
        description.SetMainImage(sprite, mat);
        if (!Filled)
            return;
        description.SetElement(activator.element);
        foreach (var effect in activator.GetEffects())
        {
            var d = description.AddDescription();
            effect.FillDescription(d);
        }
    }

    public override void Fill(PotionActivator activator)
    {
        base.Fill(activator);
        var colors = activator.GetEffects().SelectMany(e => e.colors.colors).ToList();
        colors.AddRange(activator.baseColors.SelectMany(e => e.colors));
        var tex = ColorController.CombineColors(colors);
        mat.SetTexture("_ColorTexture", tex);
        var s = DOTween.Sequence();
        var getter = new DOGetter<float>( () => mat.GetFloat("_FillPercent"));
        var setter = new DOSetter<float>(f => mat.SetFloat("_FillPercent", f));
        s.Append(DOTween.To(getter, setter, 1, 2));
        s.Play();
    }

    public override Sprite GetIcon()
    {
        return sprite;
    }

    public override Material GetMaterial()
    {
        return mat;
    }

    public void Drink(GameObject target)
    {
        activator.ApplyOnCharacter(target);
        Replace(initialObject.Copy());
    }

    public void Throw(ThrowInformation throwInfo)
    {
        var obj = Instantiate(Throwable, throwInfo.origin, Quaternion.identity);
        var thr = obj.GetComponent<ThrowedBottle>();
        thr.SetUp(activator);
        if (mat != null)
        {
            obj.GetComponent<SpriteRenderer>().material = mat;
        }
        var rb = obj.GetComponent<Rigidbody2D>();
        var colliders = throwInfo.thrower.GetComponents<Collider2D>();
        var bottleColliders = obj.GetComponents<Collider2D>();
        foreach (var c1 in colliders)
        {
            foreach (var c2 in bottleColliders)
            {
                Physics2D.IgnoreCollision(c1, c2);
            }
        }
        rb.AddForce(throwForce * throwInfo.direction.normalized, ForceMode2D.Impulse);
        rb.AddTorque(((float)random.NextDouble() - 0.5f) * 2 * 30);
        Replace();
    }
}

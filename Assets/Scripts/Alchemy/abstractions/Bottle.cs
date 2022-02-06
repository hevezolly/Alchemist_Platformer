using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bottle : ScriptableObject, IBottle
{
    protected PotionActivator activator = null;

    private Action<IItem> onReplace;

    public int ID { get; set; }

    public bool Filled => activator != null;

    public virtual void Fill(PotionActivator activator)
    {
        this.activator = activator;
    }

    public abstract IItem Copy();
    public abstract void FillDescription(IDescription description);
    public abstract Sprite GetIcon();

    public virtual Material GetMaterial()
    {
        return null;
    }

    public void SetReplaceFunction(Action<IItem> onReplace)
    {
        this.onReplace = onReplace;
    }

    protected virtual void Replace(IItem other = null)
    {
        onReplace(other);
    }
}

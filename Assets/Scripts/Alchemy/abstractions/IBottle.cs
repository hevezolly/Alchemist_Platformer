using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBottle: IItem
{
    void Fill(PotionActivator activator);

    bool Filled { get; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDrinkable
{
    void Drink(GameObject target);

    bool CanBeDrinked { get; }
}

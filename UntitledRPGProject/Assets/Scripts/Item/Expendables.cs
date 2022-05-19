using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expendables : Item
{
    public int Amount = 1;

    public override void Initialize()
    {
        base.Initialize();
        Amount = 1;
    }

    public override void Apply()
    {
        Amount++;
    }

    public override void End()
    {
        // Use
        if(Amount > 0)
            Amount--;
    }
}

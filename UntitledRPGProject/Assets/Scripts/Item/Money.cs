using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Money")]
public class Money : Item
{
    public Money(string name, int val,int amount) : base(name, val, amount)
    {
    }

    public override void Apply()
    {
        PlayerController.Instance.mGold += mValue;
    }

    public override void End()
    {
    }
}

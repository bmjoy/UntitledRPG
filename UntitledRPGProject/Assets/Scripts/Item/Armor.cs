using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : EquipmentItem
{
    [HideInInspector]
    public ArmorType armorType;

    public override void Initialize(int id)
    {
        base.Initialize(id);
        var info = (ArmorInfo)Info;
        armorType = info.mArmorType;
    }
}

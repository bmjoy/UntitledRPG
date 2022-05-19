using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : EquipmentItem
{
    [HideInInspector]
    public ArmorType armorType;
    public override void Initialize()
    {
        base.Initialize();
        var info = (ArmorInfo)Info;
        armorType = info.mArmorType;
    }
}

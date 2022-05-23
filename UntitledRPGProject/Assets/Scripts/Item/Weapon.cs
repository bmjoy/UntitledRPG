using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : EquipmentItem
{
    [HideInInspector]
    public WeaponType weaponType;

    public override void Initialize(int id)
    {
        base.Initialize(id);
        var info = (WeaponInfo)Info;
        weaponType = info.mWeaponType;
    }
}

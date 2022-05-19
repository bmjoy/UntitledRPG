using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponInfo : EquipmentInfo
{
    public WeaponType mWeaponType;
    public WeaponInfo() : base()
    {
    }
}

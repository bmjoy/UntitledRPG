using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInfo : ItemInfo
{
    [Header("Bonus Ability")]
    [SerializeField]
    public List<BonusAbility> mBonusAbilities;
}

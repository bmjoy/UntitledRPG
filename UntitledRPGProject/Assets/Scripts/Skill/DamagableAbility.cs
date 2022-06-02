using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableAbility : Skill_Setting
{
    [Range(0.0f, 1000.0f)]
    public float mValue;
    public bool isCritical = false;
    public DamageType mDamageType = DamageType.Magical;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Physical = 0,
    Magical
}

public enum AttackType
{
    Melee = 0,
    Range,
    Instant
}

public enum BuffType
{
    Melee,
    Magic
}

public enum NPCType
{
    None,
    Villager,
    Market,
    Hero,
    Trap
}

public enum WeaponType
{
    None,
    Soul,
    Bow,
    Orb,
    Spear
}
public enum ArmorType
{
    Helmet,
    BodyArmor,
    LegArmor,
    Bracer
}
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
    Instant,
    None
}

public enum BuffType
{
    Melee,
    Magic
}

public enum WeaponType
{
    None,
    Soul,
    Bow,
    Orb,
    Spear,
    Double_Swords
}
public enum ArmorType
{
    Helmet,
    BodyArmor,
    LegArmor,
    Bracer
}

public enum EnemyUnit
{
    None,
    Ghoul,
    Spitter,
    Summoner,
    Temple_Guardian,
    The_Bloody_King
}
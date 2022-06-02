using System;
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
    Spirit_Boxer,
    Droid_Assassin,
    Temple_Guardian,
    The_Bloody_King
}

[Serializable]
public class SoundClip
{
    public enum SoundType
    {
        Attack0,
        Attack1,
        Attack2,
        Death,
        Run0,
        Run1,
        Run2,
        SkillChanneling,
        SkillShoot0,
        SkillShoot1,
        SkillShoot2,
    }

    public SoundType Type;
    public AudioClip Clip;
}
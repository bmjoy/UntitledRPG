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

public enum NPCUnit
{
    None,
    Roger,
    Victor,
    Vin,
    Eleven,
    ArmorMerchant,
    WeaponMerchant
}

public enum EnvironmentObject
{
    None,
    Well
}

[Serializable]
public class SoundClip
{
    public enum SoundType
    {
        Attack,
        Death,
        Run,
        Skill
    }

    public SoundType Type;
    public AudioClip Clip;
}
[Serializable]
public class ItemDrop
{
    public float mRate = 0.0f;
    public GameObject mItem = null;
}
[Serializable]
public class CinematicEventMethod
{
    public enum CinematicEventType
    {
        None,
        Animation,
        FadeIn,
        FadeOut,
        Move,
        MoveAndDialogue,
        Dialogue,
        Teleport
    }

    public CinematicEventType Type;
    public string Dialogue = string.Empty;
    public string AnimationName = string.Empty;
    public float Time = 0.0f;
    public Transform Position = null;
    public GameObject Target = null;
}
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
    Dagger_Mush,
    Spirit_Boxer,
    Droid_Assassin,
    Cyber_Shielder,
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
    WeaponMerchant,
    Monk,
    Citizen,
    Jimmy
}

public enum EnvironmentObject
{
    None,
    Well,
    Rock,
    Chest,
    Door,
    GroundFireTrap,
    WallFireTrap,
    ProjectileTrap,
    SwitchFireTrap,
    FireOrb
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
        Teleport,
        PlayCameraShake,
        StopCameraShake,
        CameraZoom,
        PlayMusic,
        TargetEffect,
        PositionEffect
    }

    public CinematicEventType Type;
    public string Dialogue = string.Empty;
    public string AnimationName = string.Empty;
    public float Time = 0.0f;
    public float Speed = 0.0f;
    public float MaxZoom = 0.0f;
    public Transform Position = null;
    public GameObject Target = null;
    public AudioClip Clip = null;
    public GameObject Effect = null;
}

[Serializable]
public class Dialogue
{
    public Dialogue(string text, TriggerType trigger)
    {
        Text = text;
        Trigger = trigger;
    }
    public enum TriggerType
    {
        None,
        Trade,
        Event,
        Fail,
        Success
    }
    [TextArea]
    public string Text = string.Empty;
    public TriggerType Trigger = TriggerType.None;
}

[Serializable]
public class NeedsInfo
{
    public string Name = string.Empty;
    public int Value = 0;
    public int Amount = 0;
    [HideInInspector]
    public bool onComplete = false;
    public NeedsInfo(string n, int v, int a, bool complete = false)
    {
        Name = n;
        Value = v;
        Amount = a;
    }
}
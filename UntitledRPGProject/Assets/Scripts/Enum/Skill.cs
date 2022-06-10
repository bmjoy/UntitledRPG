using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Attack,
    Heal,
    Buff,
    BuffNerf,
    Nerf,
    Summon
}

public enum SkillProperty
{
    Friendly,
    Hostile
}

public enum SkillElement
{
    Fire,
    Water,
    Shadow,
    Holy,
    Undead,
    Normal
}

public enum SKillShootType
{
    Melee,
    Range,
    Instant
}

public enum SkillTarget
{
    None,
    Self,
    All,
    Random
}

[Serializable]
public class BuffList
{
    public Buff mBuff;
}
[Serializable]
public class NerfList
{
    public Nerf mNerf;
}
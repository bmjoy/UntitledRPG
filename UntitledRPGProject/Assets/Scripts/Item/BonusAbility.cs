using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class BonusAbility
{
    public enum AbilityType
    {
        Health,
        Damage,
        MagicPower,
        MagicResistance,
        Agility,
        Armor,
        Magic,
        Mana
    }

    public AbilityType Type;
    public float Value;
    public Skill_Setting Skill;
}
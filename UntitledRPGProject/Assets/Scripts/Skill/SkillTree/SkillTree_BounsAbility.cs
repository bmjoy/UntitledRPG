using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SkillTree_BounsAbility
{
    public enum SkillTreeAbilityType
    {
        Health,
        Mana,
        Damage,
        Armor,
        Movement,
        DoubleAttack,
        Shield,
        HPRegeneration,
        MPRegeneration
    }

    public SkillTreeAbilityType Type;
    public float Value;
}

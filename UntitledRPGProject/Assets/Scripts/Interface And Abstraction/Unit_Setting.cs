using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[CreateAssetMenu(menuName = "Unit Status")]
public class Unit_Setting : ScriptableObject
{
    public string Name;
    public float MaxHealth;
    public float MaxMana;
    public float Magic_Resistance;
    public float Attack;
    public float Defend;
    public float Armor;
    public float Agility;
    public Unit mTarget;

    public List<AudioClip> SFXClips;
}

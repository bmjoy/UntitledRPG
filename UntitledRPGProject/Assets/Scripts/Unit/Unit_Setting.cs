using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[CreateAssetMenu(menuName = "Unit Status")]
public class Unit_Setting : ScriptableObject
{
    public float Health;
    public float MaxHealth;
    public float Mana;
    public float MaxMana;
    public float Magic_Resistance;
    public float Attack;
    public float Defend;
    public float Armor;
    public float Agility;
    public bool Death = false;
    public GameObject mTarget;

    public List<AudioClip> SFXClips;
}

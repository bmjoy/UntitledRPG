using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[CreateAssetMenu(menuName = "Unit Status")]
public class Unit_Setting : ScriptableObject
{
    public string Name;
    public int Level;
    public int EXP;
    public float MaxHealth;
    public float MaxMana;
    public float Magic_Resistance;
    public float Attack;
    public float Defend;
    public float MagicPower;
    public float Armor;
    public float Agility;
    public int Gold;

    public WeaponType WeaponType;

    public Sprite BasicSprite;
    public List<SoundClip> Clips;
    public List<ItemDrop> Item;
}

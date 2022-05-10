using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDataStorage
{
    public int Level;
    public int EXP;
    public float CurrentHealth;
    public float MaxHealth;
    public float CurrentMana;
    public float MaxMana;
    public float Magic_Resistance;
    public float Attack;
    public float Defend;
    public float MagicPower;
    public float Armor;
    public float Agility;    
    
    public const string Level_KEY = "Level";
    public const string EXP_KEY = "Exp";
    public const string CurrentHealth_KEY = "CurrentHealth";
    public const string MaxHealth_KEY = "MaxHealth";
    public const string CurrentMana_KEY ="CurrentMana";
    public const string MaxMana_KEY = "MaxMana";
    public const string Magic_Resistance_KEY = "MagicResistance";
    public const string Attack_KEY = "Attack";
    public const string Defend_KEY = "Defend";
    public const string MagicPower_KEY = "MagicPower";
    public const string Armor_KEY = "Armor";
    public const string Agility_KEY = "Agility";


    public void Initialize()
    {
        Level = PlayerPrefs.GetInt(Level_KEY, 0);
        EXP = PlayerPrefs.GetInt(EXP_KEY, 0);
        CurrentHealth = PlayerPrefs.GetFloat(CurrentHealth_KEY, 0.0f);
        MaxHealth = PlayerPrefs.GetFloat(MaxHealth_KEY, 0.0f);
        CurrentMana = PlayerPrefs.GetFloat(CurrentMana_KEY, 0.0f);
        MaxMana = PlayerPrefs.GetFloat(MaxMana_KEY, 0.0f);
        Magic_Resistance = PlayerPrefs.GetFloat(Magic_Resistance_KEY, 0.0f);
        Attack = PlayerPrefs.GetFloat(Attack_KEY, 0.0f);
        Defend = PlayerPrefs.GetFloat(Defend_KEY, 0.0f);
        MagicPower = PlayerPrefs.GetFloat(MagicPower_KEY, 0.0f);
        Armor = PlayerPrefs.GetFloat(Armor_KEY, 0.0f);
        Agility = PlayerPrefs.GetFloat(Agility_KEY, 0.0f);
    }
    public void LoadData(ref Status status)
    {
        Level = PlayerPrefs.GetInt(Level_KEY);
        EXP = PlayerPrefs.GetInt(EXP_KEY);
        CurrentHealth = PlayerPrefs.GetFloat(CurrentHealth_KEY);
        MaxHealth = PlayerPrefs.GetFloat(MaxHealth_KEY);
        CurrentMana = PlayerPrefs.GetFloat(CurrentMana_KEY);
        MaxMana = PlayerPrefs.GetFloat(MaxMana_KEY);
        Magic_Resistance = PlayerPrefs.GetFloat(Magic_Resistance_KEY);
        Attack = PlayerPrefs.GetFloat(Attack_KEY);
        Defend = PlayerPrefs.GetFloat(Defend_KEY);
        MagicPower = PlayerPrefs.GetFloat(MagicPower_KEY);
        Armor = PlayerPrefs.GetFloat(Armor_KEY);
        Agility = PlayerPrefs.GetFloat(Agility_KEY);

        status.mLevel = Level;
        status.mEXP = EXP;
        status.mHealth = CurrentHealth;
        status.mMana = CurrentMana;
        status.mMaxMana = MaxMana;
        status.mMagic_Resistance = Magic_Resistance;
        status.mDamage = Attack;
        status.mDefend = Defend;
        status.mMagicPower = MagicPower;
        status.mArmor = Armor;
        status.mAgility = Agility;
    }

    public void SaveData(ref Status status)
    {
        PlayerPrefs.SetInt(Level_KEY, status.mLevel);
        PlayerPrefs.SetInt(EXP_KEY, status.mEXP);
        PlayerPrefs.SetFloat(CurrentHealth_KEY, status.mHealth);
        PlayerPrefs.SetFloat(MaxHealth_KEY, status.mMaxHealth);
        PlayerPrefs.SetFloat(MagicPower_KEY, status.mMagicPower);
        PlayerPrefs.SetFloat(CurrentMana_KEY, status.mMana);
        PlayerPrefs.SetFloat(MaxMana_KEY, status.mMaxMana);
        PlayerPrefs.SetFloat(Attack_KEY, status.mDamage);
        PlayerPrefs.SetFloat(Defend_KEY, status.mDefend);
        PlayerPrefs.SetFloat(Agility_KEY, status.mAgility);
        PlayerPrefs.SetFloat(Armor_KEY, status.mArmor);
        PlayerPrefs.SetFloat(Magic_Resistance_KEY, status.mMagic_Resistance);
    }


}

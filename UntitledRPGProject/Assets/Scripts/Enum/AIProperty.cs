using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIProperty
{
    Offensive,
    Defensive
}

public enum AIType
{
    None,
    Manual,
    Auto
}

public enum ActionEvent
{
    None,
    IntroWalk,
    AttackWalk,
    MagicWalk,
    Busy,
    BackWalk
}

public struct AIBuild
{
    public StateMachine stateMachine;
    public AIProperty property;
    public AIType type;
    public ActionEvent actionEvent;
}

public struct Status
{
    public int mLevel;
    public int mEXP;
    public int mGold;
    public float mMaxHealth;
    public float mHealth;
    public float mMana;
    public float mMaxMana;
    public float mDamage;
    public float mArmor;
    public float mMagic_Resistance;
    public float mDefend;
    public float mAgility;
    public float mMagicPower;

    public Status(int level, int exp, int gold, float maxHp, float hp, float mana, float maxMp, float dmg, float am, float mr, float de, float ag, float mp)
    {
        mLevel = level;
        mEXP = exp;
        mGold = gold;
        mMaxHealth = maxHp;
        mHealth = hp;
        mMana = mana;
        mMaxMana = maxMp;
        mDamage = dmg;
        mArmor = am;
        mMagic_Resistance = mr;
        mDefend = de;
        mAgility = ag;
        mMagicPower = mp;
    }
}

public struct Conditions
{
    public bool isPicked;
    public bool isDied;
    public bool isDefend;
    public bool isCancel;

    public Conditions(bool pick, bool die, bool de, bool can)
    {
        isPicked = pick;
        isDied = die;
        isDefend = de;
        isCancel = can;
    }
}
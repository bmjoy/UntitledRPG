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
    IntroRotate,
    AttackWalk,
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
    public float mMaxHealth;
    public float mHealth;
    public float mMana;
    public float mDamage;
    public float mArmor;
    public float mMagic_Resistance;
    public float mDefend;
    public float mAgility;
    public float mMagicPower;

    public Status(float maxHp, float hp, float mana, float dmg, float am, float mr, float de, float ag, float mp)
    {
        mMaxHealth = maxHp;
        mHealth = hp;
        mMana = mana;
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
    public bool isMove;
    public bool isCancel;

    public Conditions(bool pick, bool die, bool de, bool mov, bool can)
    {
        isPicked = pick;
        isDied = die;
        isDefend = de;
        isMove = mov;
        isCancel = can;
    }
}
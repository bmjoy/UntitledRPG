using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Skill_Setting : ScriptableObject
{
    public Sprite mSprite;
    [TextArea]
    public string mName;
    [TextArea]
    public string mDescription;
    [Range(0.0f, 1000.0f)]
    public float mManaCost;
    [Range(0.0f, 1000.0f)]
    public float mValue;
    public Unit mOwner;
    public bool isActive = false;
    public bool isComplete = false;
    public SkillType mSkillType;
    public SkillProperty mProperty;

    public List<GameObject> mBuffs;
    public List<GameObject> mNerfs;
    public abstract void Initialize(Unit owner);
    public abstract void Activate(MonoBehaviour parent);
    public abstract IEnumerator WaitforDecision();
}

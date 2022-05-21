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
    public SkillElement mElement = SkillElement.Normal;
    public float mEffectTime = 0.0f;

    [Serializable]
    public class BuffList
    {
        public Buff mBuff;
    }

    public List<Buff> mBuffList = new List<Buff>();
    [Serializable]
    public class NerfList
    {
        public Nerf mNerf;
    }
    
    public List<Nerf> mNerfList = new List<Nerf>();

    public UnityAction mAction;
    
    public abstract void Initialize(Unit owner);
    public abstract void Activate(MonoBehaviour parent);
    public abstract IEnumerator Effect();
    public abstract IEnumerator WaitforDecision();
}

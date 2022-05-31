using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Skill_Setting : ScriptableObject
{
    public Sprite mSprite;
    [TextArea]
    public string mName;
    [TextArea]
    public string mDescription;
    [Range(0.0f, 1000.0f)]
    public float mManaCost;
    [HideInInspector]
    public Unit mOwner;
    [HideInInspector]
    public bool isActive = false;
    [HideInInspector]
    public bool isComplete = false;
    public SkillType mSkillType;
    public SkillProperty mProperty;
    public SkillElement mElement = SkillElement.Normal;
    public float mEffectTime = 0.0f;

    public Action mActionTrigger = null;
    public string mAnimationName = string.Empty;

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
    public virtual void Initialize(Unit owner) { }
    public virtual void Activate(MonoBehaviour parent) { }
    public virtual IEnumerator Effect() { yield return null; }
    public virtual IEnumerator WaitforDecision() { yield return null; }
}

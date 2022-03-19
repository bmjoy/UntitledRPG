using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Skill_Setting
{
    public string mName;
    public string mButtonName;
    public int mLevel;
    public float mCost;
    public float mDamage;
    public SkillProperty mSkillProperty = SkillProperty.Self;
    public SkillType mSkillType = SkillType.Attack;
    public Sprite mSprite;
    public GameObject mOwner;
    public GameObject mTarget;
    public bool isActive = false;

    public void Update(MonoBehaviour parent)
    {
        if (isActive == false && IsActivate())
            parent.StartCoroutine(Activate());
    }

    public bool IsActivate()
    {
        return Input.GetButtonDown(mButtonName);
    }

    public IEnumerator Activate()
    {
        isActive = true;
        yield return null;
        isActive = false;
    }
}

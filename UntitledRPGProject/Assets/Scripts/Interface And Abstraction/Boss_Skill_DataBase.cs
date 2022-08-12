using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Skill_DataBase : Skill_DataBase
{
    public Skill_Setting[] mSkillDatas = new Skill_Setting[3];
    public int mCurrentIndex = 0;
    public int mUltimateSkillIndex = 2;
    public override void Initialize()
    {
        if (mSkillDatas.Length == 0)
        {
            mSkill = mSkillData;
            mSkill.mOwner = transform.GetComponent<Unit>();
            mSkill.Initialize(GetComponent<Unit>());
        }
        else
        {
            mSkill = mSkillDatas[0];
            mSkill.mOwner = transform.GetComponent<Unit>();
            mSkill.Initialize(GetComponent<Unit>());
        }

    }

    public Skill_Setting ChangeSkill(int index)
    {
        mSkill.mOwner = null;
        mSkill.isComplete = false;

        mSkill = mSkillDatas[index];
        mSkill.Initialize(GetComponent<Unit>());
        mCurrentIndex = index;
        return mSkill;
    }

    public override void Use()
    {
        base.Use();
    }
}

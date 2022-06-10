using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DataBase : MonoBehaviour
{
    //public List<Skill_Setting> mSkills = new List<Skill_Setting>();
    [SerializeField]
    protected Skill_Setting mSkillData;
    [HideInInspector]
    public Skill_Setting mSkill;

    public virtual Skill_Setting Skill { set { Skill = value; } get { return mSkill; } }
    public virtual string Name { get { return mSkill.mName; } }
    public virtual float Mana { get { return mSkill.mManaCost; } }
    public virtual string Description { get { return mSkill.mDescription; } }
    public virtual SkillType Type { get { return mSkill.mSkillType; } }

    protected virtual void Start()
    {
        if (mSkillData == null)
            return;
        mSkill = mSkillData;
        mSkill.mOwner = transform.GetComponent<Unit>();
        mSkill.Initialize(GetComponent<Unit>());
    }

    public virtual void Use()
    {
        if (mSkill == null)
            return;
        mSkill.isComplete = false;
        mSkill.Activate(this);
    }
}

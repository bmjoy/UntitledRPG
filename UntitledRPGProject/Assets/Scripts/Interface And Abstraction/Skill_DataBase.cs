using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DataBase : MonoBehaviour
{
    //public List<Skill_Setting> mSkills = new List<Skill_Setting>();
    public Skill_Setting mSkill;
    private void Start()
    {
        mSkill.Initialize(GetComponent<Unit>());
    }

    public void Use()
    {
        mSkill.isComplete = false;
        mSkill.Activate(this);
    }
}

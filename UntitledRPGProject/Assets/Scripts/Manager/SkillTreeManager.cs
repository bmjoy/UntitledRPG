using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    private static SkillTreeManager mInstance;
    public static SkillTreeManager _Instance { get { return mInstance; } }

    public List<Skill_Node> skill_Nodes;
    public List<SkillTree_BounsAbility> mTotalBounsAbilities;
    public event Action<SkillTree_BounsAbility> OnGainAbility;

    private void Awake()
    {
        if(mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
        skill_Nodes = new List<Skill_Node>();
        mTotalBounsAbilities.Clear();
        OnGainAbility = null;
    }

    public void UnlockSkill(Skill_Node skill_Node)
    {
        if(!skill_Node.IsUnlocked() && skill_Node.IsChildrenUnlocked())
        {
            skill_Node.Unlock();
            if(skill_Node.IsUnlocked())
            {
                mTotalBounsAbilities.Add(skill_Node._BonusAbility);
                OnGainAbility?.Invoke(skill_Node._BonusAbility);
            }
        }
    }

    public void ResetSkills()
    {
        foreach(var skill in skill_Nodes)
        {
            skill.ResetNode();
        }
        mTotalBounsAbilities.Clear();
        PlayerController.Instance.ResetAbility();
    }
}

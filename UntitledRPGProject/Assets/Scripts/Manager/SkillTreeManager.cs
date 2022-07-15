using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    private static SkillTreeManager mInstance;
    public static SkillTreeManager _Instance { get { return mInstance; } }

    public List<SkillTree_BounsAbility> mTotalBounsAbilities;
    public event Action<SkillTree_BounsAbility> OnGainAbility;

    private void Awake()
    {
        if(mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
        mTotalBounsAbilities.Clear();
        OnGainAbility = null;
    }

    public void UnlockSkill(Skill_Node skill_Node)
    {
        if(!skill_Node.IsUnlocked() && skill_Node.IsChildrenUnlocked())
        {
            skill_Node.Unlock();
            mTotalBounsAbilities.Add(skill_Node._BonusAbility);
            OnGainAbility?.Invoke(skill_Node._BonusAbility);
        }
    }
}

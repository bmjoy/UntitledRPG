using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    private static SkillTreeManager mInstance;
    public static SkillTreeManager Instance { get { return mInstance; } }


    public Skill_Node[] All_Skill_Nodes;

    public List<Skill_Node> skill_Nodes; // Achieved Skills
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
                OnGainAbility?.Invoke(skill_Node._BonusAbility);
        }
    }

    public void UnlockSkill(string name)
    {
        Skill_Node node = skill_Nodes.Find(n => n._Name == name);
        if (node == null)
            return;
        if (!node.IsUnlocked())
        {
            node.Unlock_Free();
            if (node.IsUnlocked())
                OnGainAbility?.Invoke(node._BonusAbility);
        }
    }

    public void ResetSkills()
    {
        for (int i = 0; i < skill_Nodes.Count; ++i)
        {
            var skill = skill_Nodes[i];
            skill.ResetNode();
        }
        mTotalBounsAbilities.Clear();
        PlayerController.Instance.ResetAbility();
    }
}

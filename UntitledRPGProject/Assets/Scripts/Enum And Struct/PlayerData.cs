using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    [Serializable]
    public struct InventoryInfoString
    {
        public string Head;
        public string Arm;
        public string Body;
        public string Leg;
        public string Weapon;
    }

    public int mMoney;
    public int mSoul;
    public Dictionary<string, Status> mPlayerUnitStatus = new Dictionary<string, Status>();
    public Dictionary<string, InventoryInfoString> mPlayerUnitInventory = new Dictionary<string, InventoryInfoString>();
    public string[] mPossessedItems;
    public string[] mUnlockedSkill_Nodes;
    public PlayerData()
    {
        mMoney = PlayerController.Instance.mGold;
        mSoul = PlayerController.Instance.mSoul;

        // Item and Status
        mPossessedItems = new string[PlayerController.Instance.mBag.transform.childCount];
        for (int i = 0; i < PlayerController.Instance.mBag.transform.childCount; i++)
        {
            mPossessedItems[i] = PlayerController.Instance.mBag.transform.GetChild(i).GetComponent<Item>().Name;
        }
        for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
        {
            var unit = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
            mPlayerUnitStatus.Add(unit.mSetting.Name, unit.mStatus);
            InventoryInfoString info = new InventoryInfoString();
            
            info.Arm = (unit.mInventroySystem.mInventoryInfo.Arm) ? unit.mInventroySystem.mInventoryInfo.Arm.Name : "Null";
            info.Weapon = (unit.mInventroySystem.mInventoryInfo.Weapon) ? unit.mInventroySystem.mInventoryInfo.Weapon.Name : "Null";
            info.Head = (unit.mInventroySystem.mInventoryInfo.Head) ? unit.mInventroySystem.mInventoryInfo.Head.Name : "Null";
            info.Body = (unit.mInventroySystem.mInventoryInfo.Body) ? unit.mInventroySystem.mInventoryInfo.Body.Name : "Null";
            info.Leg = (unit.mInventroySystem.mInventoryInfo.Leg) ? unit.mInventroySystem.mInventoryInfo.Leg.Name : "Null";
            mPlayerUnitInventory.Add(unit.mSetting.Name, info);
        }
        
        // Achieved Skill Tree
        for (int i = 0; i < SkillTreeManager._Instance.skill_Nodes.Count; ++i)
        {
            var skill = SkillTreeManager._Instance.skill_Nodes[i];
            if(skill.IsUnlocked())
            {
                mUnlockedSkill_Nodes[i] = skill._Name;
            }
        }
    }
}

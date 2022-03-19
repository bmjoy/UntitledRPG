using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Unit Skills")]
public class Skill_DataBase : ScriptableObject
{
    public List<Skill_Setting> mSkills = new List<Skill_Setting>();
    public void UpdateSkills(MonoBehaviour parent)
    {
        mSkills.ForEach(skill => skill.Update(parent));
    }
}

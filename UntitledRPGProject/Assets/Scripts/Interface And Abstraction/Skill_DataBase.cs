using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DataBase : MonoBehaviour
{
    //public List<Skill_Setting> mSkills = new List<Skill_Setting>();
    public Skill_Setting mSkill;
    public bool isDisplay = false;
    public bool isComplete = false;
    private void Start()
    {
        mSkill.Initialize(GetComponent<Unit>());
    }

    public void Use()
    {
        mSkill.Activate();
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        isComplete = true;
        yield return new WaitForSeconds(0.2f);
        isComplete = false;
    }

    public void Display()
    {
        isDisplay = !isDisplay;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkillTreeScreen : MonoBehaviour
{
    private Transform mBorader;
    public Sprite mBasicImage;
    [SerializeField]
    private GameObject mSelect;

    private bool isInitialize = false;

    private int mSelectIndex = 0;
    public void Initialize()
    {
        mBorader = transform.Find("Panel");
        mSelectIndex = 0;
        isInitialize = true;
        SkillTreeManager.Instance.All_Skill_Nodes = FindObjectsOfType<Skill_Node>();
        Array.Reverse(SkillTreeManager.Instance.All_Skill_Nodes);
        for (int i = 0; i < SkillTreeManager.Instance.All_Skill_Nodes.Length; ++i)
            SkillTreeManager.Instance.All_Skill_Nodes[i].Initialize();
    }

    void Update()
    {
        if(PlayerController.Instance)
            transform.Find("MyPanel").Find("Value").GetComponent<TextMeshProUGUI>().text =
            PlayerController.Instance.mSoul.ToString();
        if (!gameObject.activeSelf)
            return;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (mSelectIndex > 0)
                mSelectIndex--;
            Display(SkillTreeManager.Instance.skill_Nodes[mSelectIndex]);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (mSelectIndex < SkillTreeManager.Instance.skill_Nodes.Count - 1)
                mSelectIndex++;
            Display(SkillTreeManager.Instance.skill_Nodes[mSelectIndex]);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (mSelectIndex > 0)
            {
                mSelectIndex -= 3;
                if (mSelectIndex < 0)
                    mSelectIndex = 0;
            }
            Display(SkillTreeManager.Instance.skill_Nodes[mSelectIndex]);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (mSelectIndex < SkillTreeManager.Instance.skill_Nodes.Count - 1)
            {
                mSelectIndex += 3;
                if (mSelectIndex > SkillTreeManager.Instance.skill_Nodes.Count - 1)
                    mSelectIndex = SkillTreeManager.Instance.skill_Nodes.Count - 1;
            }
            Display(SkillTreeManager.Instance.skill_Nodes[mSelectIndex]);
        }
        if (Input.GetKeyDown(KeyCode.E))
            SkillTreeManager.Instance.UnlockSkill(SkillTreeManager.Instance.skill_Nodes[mSelectIndex]);
        mSelect.transform.SetParent(SkillTreeManager.Instance.skill_Nodes[mSelectIndex].transform);
        mSelect.transform.position = SkillTreeManager.Instance.skill_Nodes[mSelectIndex].transform.position;
    }

    public void Active(bool active)
    {
        transform.gameObject.SetActive(active);
    }

    public void Display(Skill_Node skill_Node)
    {
        AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mSkillTreeButtonSFX);

        mBorader.Find("Name").GetComponent<TextMeshProUGUI>().text = skill_Node._Name;
        mBorader.Find("Description").GetComponent<TextMeshProUGUI>().text = skill_Node._Description;
        mBorader.Find("Requirements").GetComponent<TextMeshProUGUI>().text =
            "Requirements: ";
        if(skill_Node._Parents.Count > 0)
            for (int i = 0; i < skill_Node._Parents.Count; ++i)
                mBorader.Find("Requirements").GetComponent<TextMeshProUGUI>().text += $"{skill_Node._Parents[i]._Name} ";
        else
            mBorader.Find("Requirements").GetComponent<TextMeshProUGUI>().text += "None";

        mBorader.Find("Cost").GetComponent<TextMeshProUGUI>().text = skill_Node._Cost.ToString();
        mBorader.Find("Icon").GetComponent<Image>().sprite = skill_Node._Sprite.sprite;
        mBorader.Find("Icon").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
    }

    private void OnEnable()
    {
        if (!isInitialize)
            return;
        mBorader.Find("Name").GetComponent<TextMeshProUGUI>().text = "Name";
        mBorader.Find("Description").GetComponent<TextMeshProUGUI>().text = "Choose a upgrade";
        mBorader.Find("Requirements").GetComponent<TextMeshProUGUI>().text =
            "Requirements: ";
        mBorader.Find("Cost").GetComponent<TextMeshProUGUI>().text = "0";
        mBorader.Find("Icon").GetComponent<Image>().sprite = mBasicImage;
        mBorader.Find("Icon").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
        mSelectIndex = 0;
    }
}

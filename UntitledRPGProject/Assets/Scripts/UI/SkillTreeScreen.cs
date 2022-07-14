using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeScreen : MonoBehaviour
{
    private Transform mBorader;
    private Button mPurchase;
    public Sprite mBasicImage;

    private bool isInitialize = false;

    public void Initialize()
    {
        mBorader = transform.Find("Panel");
        mPurchase = mBorader.Find("Buy").GetComponent<Button>();
        isInitialize = true;
    }

    void Update()
    {
        if(PlayerController.Instance)
            transform.Find("MyPanel").Find("Value").GetComponent<TextMeshProUGUI>().text =
            PlayerController.Instance.mSoul.ToString();
    }

    public void Active(bool active)
    {
        mPurchase.onClick.RemoveAllListeners();
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
        mPurchase.onClick.RemoveAllListeners();
        if(!skill_Node.IsUnlocked())
            mPurchase.onClick.AddListener(() => SkillTreeManager._Instance.UnlockSkill(skill_Node));
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
        mPurchase.onClick.RemoveAllListeners();
    }
}

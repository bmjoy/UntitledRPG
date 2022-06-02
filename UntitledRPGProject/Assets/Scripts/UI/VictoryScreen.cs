using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    private Transform[] Boraders;
    private Transform mItemList;
    private Sprite _basicSprite;
    public void Initialize()
    {
        Boraders = Array.FindAll(transform.Find("Panel").Find("Characters").GetComponentsInChildren<Transform>(), x => x.name == "Borader");
        mItemList = transform.Find("Panel").Find("ItemList");
        _basicSprite = Boraders[0].Find("Sprite").GetComponent<Image>().sprite;
        foreach (Transform t in Boraders)
            t.gameObject.SetActive(false);
        Active(false);
    }

    void Update()
    {
        for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
        {
            var fill = Boraders[i].Find("EXP").Find("Fill").GetComponent<Image>().fillAmount;
            var hero = PlayerController.Instance.mHeroes[i].GetComponent<Unit>().mStatus;
            Boraders[i].Find("EXP").Find("Fill").GetComponent<Image>().fillAmount = Mathf.Lerp(fill, (float)(hero.mEXP) / (float)(500 * hero.mLevel), Time.deltaTime * 1.5f);
        }
    }

    public void Active(bool active)
    {
        if (active)
            UpdateCharacterList();
        else
        {
            foreach (Transform t in mItemList.Find("ItemScroll").Find("Items"))
                Destroy(t.gameObject);
        }
        if (PlayerController.Instance != null)
        {
            for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
            {
                Boraders[i].gameObject.SetActive(active);
            }
        }
        gameObject.SetActive(active);
    }

    private void UpdateCharacterList()
    {
        for(int i=0; i < PlayerController.Instance.mHeroes.Count; ++i)
        {
            var hero = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
            var levelUp = hero.LevelUP();
            var borader = Boraders[i].transform;
            borader.Find("Status").gameObject.SetActive((levelUp.Key == true) ? true : false);
            if(levelUp.Key)
            {
                borader.Find("Status").Find("Health").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + levelUp.Value.mHealth.ToString();
                borader.Find("Status").Find("Mana").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + levelUp.Value.mMana.ToString();
                borader.Find("Status").Find("Sub_First").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + levelUp.Value.mDamage.ToString();
                borader.Find("Status").Find("Sub_Second").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + levelUp.Value.mMagicPower.ToString();
                borader.Find("Status").Find("Sub_Third").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + levelUp.Value.mArmor.ToString();
                borader.Find("Status").Find("Sub_Forth").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + levelUp.Value.mMagic_Resistance.ToString();
            }
            borader.Find("Name").GetComponent<TextMeshProUGUI>().text = hero.mSetting.Name;
            borader.Find("Level").GetComponent<TextMeshProUGUI>().text = "Lv. " + hero.mStatus.mLevel.ToString();
            borader.Find("EXPValue").GetComponent<TextMeshProUGUI>().text = (levelUp.Key) ? "Level up!" : hero.mStatus.mEXP.ToString() + " / " + 500 *  hero.mStatus.mLevel;
            borader.Find("Sprite").GetComponent<Image>().sprite = Resources.Load<Image>("Prefabs/UI/" + hero.mSetting.Name + "_UI").sprite;
            borader.Find("Sprite").GetComponent<Animator>().runtimeAnimatorController = Resources.Load<Animator>("Prefabs/UI/" + hero.mSetting.Name + "_UI").runtimeAnimatorController;
        }
    }
    public IEnumerator WaitForEnd()
    {
        GetComponent<Animator>().SetTrigger("Outro");
        yield return new WaitForSeconds(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime +0.5f);
        GetComponent<Animator>().ResetTrigger("Outro");
        Active(false);
    }

    public void UpdateItemList(List<GameObject> ItemList)
    {
        mItemList.Find("Gold").GetComponent<TextMeshProUGUI>().text = GameManager.s_TotalGold.ToString();
        mItemList.Find("Soul").GetComponent<TextMeshProUGUI>().text = GameManager.s_TotalSoul.ToString();
        foreach (var item in ItemList)
        {
            GameObject slot = Instantiate(Resources.Load<GameObject>("Prefabs/UI/ObtainedItem"));
            slot.transform.SetParent(mItemList.Find("ItemScroll").Find("Items"));
            slot.transform.Find("Sprite").GetComponent<Image>().sprite = item.GetComponent<Item>().Info.mSprite;
            slot.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = item.GetComponent<Item>().Info.mName;
        }
    }
}

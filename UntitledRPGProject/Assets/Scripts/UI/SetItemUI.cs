using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetItemUI : MonoBehaviour
{
    public Button mButton;
    public Image mSprite;
    public TextMeshProUGUI mName;
    public int ID;
    public Item mItem;

    public void Initialize()
    {
        mButton = GetComponent<Button>();
        mButton.onClick.RemoveAllListeners();
        mSprite = transform.Find("ItemSprite").GetComponent<Image>();
        mSprite.sprite = mItem.Info.mSprite;
        mName = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        mName.text = mItem.Name;
        GetComponent<HoverTip>().mTipToShow = "<color=yellow>" + mItem.Name + "</color>";
        if(typeof(EquipmentInfo).IsAssignableFrom(mItem.Info.GetType()))
        {
            var item = (EquipmentInfo)mItem.Info;
            foreach(var i in item.mBonusAbilities)
            {
                if(i.Type == BonusAbility.AbilityType.Magic)
                    GetComponent<HoverTip>().mTipToShow += "\n This item can use " + i.Type.ToString();
                else
                    GetComponent<HoverTip>().mTipToShow += "\n<color=green>" + i.Type.ToString() + "</color>: " + i.Value;
            }
        }
        mButton.onClick.AddListener(() => Activate());
        ID = mItem.ID;
    }

    public void Activate()
    {
        int index = UIManager.Instance.mInventoryUI.mIndex;
        var unit = PlayerController.Instance.mHeroes[index].GetComponent<InventroySystem>();
        var item = (EquipmentItem)mItem;
        AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mItemEquipSFX);
        if (unit.Equip(item))
        {
            Debug.Log("Hi");
            mName = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            mName.text = mItem.Name;
            if (mItem is Weapon || mItem is Armor)
            {
                gameObject.SetActive(false);
            }
        }
    }
}

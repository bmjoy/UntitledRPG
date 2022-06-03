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

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
    public int mAmount;
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
    }

    public void Activate()
    {
        int index = UIManager.Instance.mInventoryUI.mIndex;
        var unit = PlayerController.Instance.mHeroes[index].GetComponent<InventroySystem>();
        var item = (EquipmentItem)mItem;
        item.IsEquipped = false;
        unit.Equip(item);

        mName = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        mName.text = mItem.Name;
        if(mItem is Weapon || mItem is Armor)
        {
            Destroy(gameObject);
        }
    }
}

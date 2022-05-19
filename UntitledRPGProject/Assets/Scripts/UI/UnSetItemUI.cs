using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnSetItemUI : MonoBehaviour
{
    public Button mButton;
    public Image mSprite;
    public Item mItem;

    public void Initialize()
    {
        mButton = GetComponent<Button>();
        mButton.onClick.RemoveAllListeners();
        mSprite = GetComponent<Image>();
        mButton.onClick.AddListener(() => Activate());
    }

    public void Activate()
    {
        int index = UIManager.Instance.mInventoryUI.mIndex;
        var unit = PlayerController.Instance.mHeroes[index].GetComponent<InventroySystem>();
        switch (name)
        {
            case "Arm":
                unit.UnEquip(unit.mInventoryInfo.Arm);
                break;
            case "Body":
                unit.UnEquip(unit.mInventoryInfo.Body);
                break;
            case "Head":
                unit.UnEquip(unit.mInventoryInfo.Head);
                break;
            case "Leg":
                unit.UnEquip(unit.mInventoryInfo.Leg);
                break;
            case "Weapon":
                unit.UnEquip(unit.mInventoryInfo.mWeapon);
                break;
        }
        
        mSprite.sprite = UIManager.Instance.mInventoryUI.mEmptyImage;
    }
}

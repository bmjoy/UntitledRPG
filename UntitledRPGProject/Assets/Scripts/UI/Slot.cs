using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    private TextMeshProUGUI mName;
    private TextMeshProUGUI mDescription;
    private TextMeshProUGUI mCost;
    private GameObject mBoarder;

    private Image mItemImage;
    [SerializeField]
    private Sprite mBasicImage;    
    [SerializeField]
    private Sprite mSoldImage;
    private Button mButton;

    GameObject mMyItem = null;
    private bool isInitialized = false;

    public void Initialize(ref GameObject myItem, Transform t)
    {
        mBoarder = t.gameObject;
        mName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        mDescription = transform.Find("Status").GetComponent<TextMeshProUGUI>();
        mCost = transform.Find("Value").GetComponent<TextMeshProUGUI>();
        mItemImage = transform.Find("SlotImage").Find("Image").GetComponent<Image>();
        mButton = transform.Find("SlotImage").Find("Image").GetComponent<Button>();

        ResetItem();
        if (myItem == null)
            return;

        Type type = myItem.GetComponent<Item>().GetType();
        if(type.IsSubclassOf(typeof(EquipmentItem)))
        {
            EquipmentItem equipment = myItem.GetComponent<EquipmentItem>();
            if (equipment.isSold) return;
            mMyItem = myItem;
            InputInfo(equipment);
        }

        mButton.onClick.AddListener(Buy);
    }

    private void InputInfo(Item item)
    {
        mName.text = item.Info.mName;
        mCost.text = item.Info.mCost.ToString();
        mItemImage.sprite = item.Info.mSprite;

        object info_Object = item.Info;
        if (info_Object.GetType().IsSubclassOf((typeof(EquipmentInfo))))
        {
            EquipmentInfo info = (EquipmentInfo)info_Object;
            foreach (var ability in info.mBonusAbilities)
            {
                mDescription.text += ((ability.Type != BonusAbility.AbilityType.Magic)) ?
                    ability.Type.ToString() + ": " + ability.Value + "\n"
                    : ability.Skill.mName + "\n";
            }
        }
    }

    private void ResetItem()
    {
        mItemImage.sprite = mBasicImage;
        mButton.onClick.RemoveAllListeners();
        mName.text = string.Empty;
        mDescription.text = string.Empty;
        mCost.text = 0.ToString();
        isInitialized = true;
    }

    private void Buy()
    {
        if (mMyItem == null)
            return;
        int result = 0;
        if (!int.TryParse(mCost.text.ToString(), out result))
        {
            Debug.LogError("<color=red>Error!</color> cannot parse the value!");
            return;
        }
        if(PlayerController.Instance.mGold >= result)
        {
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mItemPurchaseSFX);
            PlayerController.Instance.mGold -= result;
            GameObject newItem = Instantiate(mMyItem);
            newItem.transform.SetParent(PlayerController.Instance.transform.Find("Bag"));
            PlayerController.Instance.mInventory.Add(newItem.GetComponent<Item>());
            newItem.GetComponent<Item>().isSold = true;
            mBoarder.transform.Find("Money").Find("Value").GetComponent<TextMeshProUGUI>().text = PlayerController.Instance.mGold.ToString();
            Destroy(mMyItem);
            ResetItem();
        }
    }

    private void OnEnable()
    {
        if (isInitialized == false)
            return;
        mButton.onClick.AddListener(Buy);
    }

    private void OnDisable()
    {
        if (isInitialized == false)
            return;
        mName.text = string.Empty;
        mDescription.text = string.Empty;
        mCost.text = string.Empty;
        mItemImage.sprite = mBasicImage;
        mButton.onClick.RemoveAllListeners();
        mMyItem = null;
    }
}

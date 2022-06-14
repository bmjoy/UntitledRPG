using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantScreen : MonoBehaviour
{
    private Transform mBoarder;
    private Transform mMoney;
    private Slot[] slots = new Slot[3];
    private Transform MyInventory;
    private Transform mItemsGroup;
    [HideInInspector]
    public Transform mSoldItemsGroup;

    void Start()
    {
        mBoarder = transform;
        mBoarder.gameObject.SetActive(false);
        MyInventory = transform.Find("MyInventory");
        mItemsGroup = MyInventory.Find("Items");
        mSoldItemsGroup = transform.Find("Sold");
        slots[0] = transform.Find("Slots").Find("Slot1").GetComponent<Slot>();
        slots[1] = transform.Find("Slots").Find("Slot2").GetComponent<Slot>();
        slots[2] = transform.Find("Slots").Find("Slot3").GetComponent<Slot>();
        mMoney = transform.Find("Money");
    }

    private void Update()
    {
        mMoney.Find("Value").GetComponent<TextMeshProUGUI>().text = PlayerController.Instance.mGold.ToString();
    }

    public void InitializeBuy(ref GameObject[] obj)
    {
        mMoney.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = PlayerController.Instance.mGold.ToString();
        for (int i = 0; i < slots.Length; ++i)
        {
            if (obj[i] == null)
                continue;
            if (obj[i].GetComponent<Item>().isSold)
                continue;
            slots[i].Initialize(ref obj[i],transform);
        }
    }

    public void InitializeSell()
    {
        foreach (var item in PlayerController.Instance.mInventory.myInventory)
        {
            EquipmentItem equipment = (EquipmentItem)item.Value;

            if (equipment.IsEquipped) continue;

            if (!equipment.isSold) continue;

            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Item"), mItemsGroup.transform.position, Quaternion.identity, mItemsGroup.transform);
            go.GetComponent<ItemUI>().Initialize(item.Key ,item.Value, ItemUI.ItemUIType.Sell);
        }
    }

    public void EnqueueSoldItem(int id, Item item, GameObject obj)
    {
        obj.transform.SetParent(mSoldItemsGroup.Find("Items"));
        obj.GetComponent<ItemUI>().Initialize(id, item, ItemUI.ItemUIType.Buy);
    }

    public void EnqueueItem(int id, Item item, GameObject obj)
    {
        obj.transform.SetParent(mItemsGroup);
        obj.GetComponent<ItemUI>().Initialize(id, item, ItemUI.ItemUIType.Sell);
    }

    public void BuyActive(bool active)
    {
        for (int i = 0; i < slots.Length; ++i)
            slots[i].gameObject.SetActive(active);
        mBoarder.gameObject.SetActive(active);
        mMoney.gameObject.SetActive(active);
        MyInventory.gameObject.SetActive(false);
        mSoldItemsGroup.gameObject.SetActive(false);
        gameObject.SetActive(active);
    }    
    
    public void SellActive(bool active)
    {
        for (int i = 0; i < slots.Length; ++i)
            slots[i].gameObject.SetActive(false);
        mBoarder.gameObject.SetActive(active);
        mMoney.gameObject.SetActive(active);
        MyInventory.gameObject.SetActive(active);
        mSoldItemsGroup.gameObject.SetActive(active);
        gameObject.SetActive(active);
    }

    private void OnDisable()
    {
        if (mSoldItemsGroup == null)
            return;

        int count = MyInventory.Find("Items").childCount;
        if (count > 0)
        {
            foreach (Transform it in MyInventory.Find("Items"))
            {
                Destroy(it.gameObject);
            }

        }

        count = mSoldItemsGroup.Find("Items").childCount;
        if(count > 0)
        {
            foreach (Transform it in mSoldItemsGroup.Find("Items"))
            {
                Destroy(it.gameObject);
            }

        }
        count = mSoldItemsGroup.Find("Objects").childCount;
        if( count > 0)
        {
            foreach (Transform it in mSoldItemsGroup.Find("Objects"))
            {
                Destroy(it.gameObject);
            }
        }

    }
}

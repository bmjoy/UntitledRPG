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
    [SerializeField]
    private GameObject mPickIcon;

    private int mSelectIndex = 0;
    private bool isSell = false;
    private bool mTransfer = false;

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
        mPickIcon.transform.SetParent(slots[0].transform);
        mMoney = transform.Find("Money");
        mSelectIndex = 0;
    }

    private void Update()
    {
        mMoney.Find("Value").GetComponent<TextMeshProUGUI>().text = PlayerController.Instance.mGold.ToString();
        if(!isSell)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && mSelectIndex > 0) mSelectIndex--;
            if (Input.GetKeyDown(KeyCode.RightArrow) && mSelectIndex < 2) mSelectIndex++;
            if (Input.GetKeyDown(UIManager.Instance.mYesKeyCode)) slots[mSelectIndex].Buy();

            mPickIcon.transform.SetParent(slots[mSelectIndex].transform);
            mPickIcon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            mPickIcon.transform.position = slots[mSelectIndex].transform.position + new Vector3(0, -10, 0);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && mSelectIndex > 0)
            {
                mSelectIndex--;
            }
            if(Input.GetKeyDown(KeyCode.R))
            {
                mTransfer = !mTransfer;
                mSelectIndex = 0;
            }

            if (mTransfer == false)
            {
                if (SellItems.Count == 0)
                {
                    mTransfer = true;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.RightArrow) && mSelectIndex < SellItems.Count - 1)
                {
                    mSelectIndex++;
                }
                if (Input.GetKeyDown(UIManager.Instance.mYesKeyCode) && SellItems.Count > 0
                    && SellItems[mSelectIndex].Sold)
                {
                    SellItems[mSelectIndex].Sell();
                    BuyItems.Add(SellItems[mSelectIndex]);
                    SellItems.Remove(SellItems[mSelectIndex]);
                    for (int i = 0; i < SellItems.Count; ++i)
                    {
                        if (SellItems[i].Sold == true)
                        {
                            mSelectIndex = i;
                            break;
                        }
                    }
                }
                if (SellItems.Count > 0)
                {
                    mPickIcon.transform.SetParent(SellItems[mSelectIndex].transform);
                    mPickIcon.transform.position = SellItems[mSelectIndex].transform.position + new Vector3(0, -1, 0);
                }
                else
                {
                    mPickIcon.transform.SetParent(transform.Find("MyInventory"));
                    mPickIcon.transform.position = transform.Find("MyInventory").position;
                }
            }
            else
            {
                if (BuyItems.Count == 0)
                {
                    mTransfer = false;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.RightArrow) && (mSelectIndex < BuyItems.Count - 1))
                    mSelectIndex++;
                if (Input.GetKeyDown(UIManager.Instance.mYesKeyCode) && BuyItems.Count > 0
    && BuyItems[mSelectIndex].Sold == false)
                {
                    BuyItems[mSelectIndex].Buy();
                    SellItems.Add(BuyItems[mSelectIndex]);
                    BuyItems.Remove(BuyItems[mSelectIndex]);
                    for (int i = 0; i < BuyItems.Count; ++i)
                    {
                        if (BuyItems[i].Sold == false)
                        {
                            mSelectIndex = i;
                            break;
                        }
                    }
                }
                if (BuyItems.Count > 0)
                {
                    mPickIcon.transform.SetParent(BuyItems[mSelectIndex].transform);
                    mPickIcon.transform.position = BuyItems[mSelectIndex].transform.position + new Vector3(0, -1, 0);
                }
                else
                {
                    mPickIcon.transform.SetParent(transform.Find("Sold"));
                    mPickIcon.transform.position = transform.Find("Sold").position;
                }
            }
        }

    }

    public void InitializeBuy(ref GameObject[] obj)
    {
        isSell = false;
        mSelectIndex = 0;
        mMoney.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = PlayerController.Instance.mGold.ToString();
        for (int i = 0; i < slots.Length; ++i)
        {
            if (obj[i] == null || obj[i].GetComponent<Item>().isSold) continue;
            slots[i].Initialize(ref obj[i],transform);
        }
    }
    List<ItemUI> SellItems = new List<ItemUI>();
    List<ItemUI> BuyItems = new List<ItemUI>();
    public void InitializeSell()
    {
        SellItems.Clear();
        BuyItems.Clear();
        foreach (var item in PlayerController.Instance.mInventory.myInventory)
        {
            if(item.GetType().IsAssignableFrom(typeof(EquipmentItem)))
            {
                EquipmentItem equipment = (EquipmentItem)item.Value;
                if (equipment.IsEquipped || !equipment.isSold) continue;
            }
            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Item"), mItemsGroup.transform.position, Quaternion.identity, mItemsGroup.transform);
            go.GetComponent<ItemUI>().Initialize(item.Key ,item.Value, ItemUI.ItemUIType.Sell);
            SellItems.Add(go.GetComponent<ItemUI>());
        }
        isSell = true;
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
        mPickIcon.transform.SetParent(slots[0].transform);
        mPickIcon.transform.position = slots[0].transform.position;
        for (int i = 0; i < slots.Length; ++i)
            slots[i].gameObject.SetActive(active);
        MyInventory.gameObject.SetActive(false);
        mSoldItemsGroup.gameObject.SetActive(false);
        Active(active);
    }    
    
    public void SellActive(bool active)
    {
        mPickIcon.transform.SetParent(transform.Find("MyInventory"));
        mPickIcon.transform.position = slots[0].transform.position;
        for (int i = 0; i < slots.Length; ++i)
            slots[i].gameObject.SetActive(false);
        MyInventory.gameObject.SetActive(active);
        mSoldItemsGroup.gameObject.SetActive(active);
        Active(active);
    }

    public void Active(bool active)
    {
        mBoarder.gameObject.SetActive(active);
        mMoney.gameObject.SetActive(active);
        gameObject.SetActive(active);
        mSelectIndex = 0;
    }

    private void OnDisable()
    {
        if (mSoldItemsGroup == null) return;
        int count = MyInventory.Find("Items").childCount;
        if (count > 0)
            foreach (Transform it in MyInventory.Find("Items"))
                Destroy(it.gameObject);
        count = mSoldItemsGroup.Find("Items").childCount;
        if(count > 0)
            foreach (Transform it in mSoldItemsGroup.Find("Items"))
                Destroy(it.gameObject);
        count = mSoldItemsGroup.Find("Objects").childCount;
        if(count > 0)
            foreach (Transform it in mSoldItemsGroup.Find("Objects"))
                Destroy(it.gameObject);
        isSell = mTransfer = false;
        mSelectIndex = 0;
        SellItems.Clear();
        BuyItems.Clear();
    }
}

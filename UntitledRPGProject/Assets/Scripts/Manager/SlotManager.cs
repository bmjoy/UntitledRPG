using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    enum MerchantType
    {
        Armor,
        Weapon
    }

    public GameObject[] myItem;
    [SerializeField]
    private MerchantType mType;

    void Start()
    {
        myItem = new GameObject[3];
        for (int i = 0; i < myItem.Length; i++)
        {
            myItem[i] = (mType == MerchantType.Armor) ? Instantiate(GameManager.Instance.mArmorPool[Random.Range(0, GameManager.Instance.mArmorPool.Length)], transform)
                : Instantiate(GameManager.Instance.mWeaponPool[Random.Range(0, GameManager.Instance.mWeaponPool.Length)], transform);
        }
    }

    public void StartBuyTrade()
    {
        UIManager.Instance.mMerchantScreen.gameObject.SetActive(true);
        UIManager.Instance.mMerchantScreen.BuyActive(true);
        UIManager.Instance.mMerchantScreen.InitializeBuy(ref myItem);
    }
    public void StartSellTrade()
    {
        UIManager.Instance.mMerchantScreen.gameObject.SetActive(true);
        UIManager.Instance.mMerchantScreen.SellActive(true);
        UIManager.Instance.mMerchantScreen.InitializeSell();
    }

    public void EndTrade()
    {
        UIManager.Instance.mMerchantScreen.BuyActive(false);
        UIManager.Instance.mMerchantScreen.SellActive(false);
        UIManager.Instance.mMerchantScreen.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < myItem.Length; ++i)
        {
            Destroy(myItem[i]);
        }
    }
}

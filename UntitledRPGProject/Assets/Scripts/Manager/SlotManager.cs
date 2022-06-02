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
            myItem[i] = (mType == MerchantType.Armor) ? Instantiate(GameManager.Instance.mArmorPool[Random.Range(0, GameManager.Instance.mArmorPool.Length)])
                : Instantiate(GameManager.Instance.mWeaponPool[Random.Range(0, GameManager.Instance.mWeaponPool.Length)]);
            myItem[i].transform.SetParent(transform, false);
        }
    }

    public void StartTrade()
    {
        UIManager.Instance.mMerchantScreen.Active(true);
        UIManager.Instance.mMerchantScreen.InitializeALL(ref myItem);
    }

    public void EndTrade()
    {
        UIManager.Instance.mMerchantScreen.Active(false);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < myItem.Length; ++i)
        {
            Destroy(myItem[i]);
        }
    }
}
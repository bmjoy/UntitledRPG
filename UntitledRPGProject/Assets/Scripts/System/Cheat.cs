using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    private List<Item> itemList = new List<Item>();
    public string mAddItemCheatCode;
    public string mRemoveAllItemCheatCode;
    public string mOverPowerCheatCode;
    public string mMoneyCheatCode;
    void Update()
    {
        if(Input.GetKeyDown(mAddItemCheatCode))
        {
            foreach (var item in items)
            {
                GameObject i = Instantiate(item);
                itemList.Add(i.GetComponent<Item>());
                i.transform.SetParent(transform, false);
            }
            foreach (var item in itemList)
            {
                item.isSold = true;
                item.transform.SetParent(PlayerController.Instance.transform.Find("Bag"));
                PlayerController.Instance.mInventory.Add(item);
            }
        }
        if(Input.GetKeyDown(mRemoveAllItemCheatCode))
        {
            foreach (var hero in PlayerController.Instance.mHeroes)
            {
                hero.GetComponent<InventroySystem>().UnEquip(hero.GetComponent<InventroySystem>().mInventoryInfo.Arm);
                hero.GetComponent<InventroySystem>().UnEquip(hero.GetComponent<InventroySystem>().mInventoryInfo.Head);
                hero.GetComponent<InventroySystem>().UnEquip(hero.GetComponent<InventroySystem>().mInventoryInfo.Weapon);
                hero.GetComponent<InventroySystem>().UnEquip(hero.GetComponent<InventroySystem>().mInventoryInfo.Body);
                hero.GetComponent<InventroySystem>().UnEquip(hero.GetComponent<InventroySystem>().mInventoryInfo.Leg);
            }
            foreach (var item in itemList)
            {
                PlayerController.Instance.mInventory.Remove(item);
            }
            itemList.Clear();
        }
        if(Input.GetKeyDown(mOverPowerCheatCode))
        {
            Status status = new Status(100, 0, 0, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500);
            foreach(GameObject gameObject in PlayerController.Instance.mHeroes)
            {
                gameObject.GetComponent<Player>().mStatus.mDamage += status.mDamage;
                gameObject.GetComponent<Player>().mStatus.mDefend += status.mDefend;
                gameObject.GetComponent<Player>().mStatus.mArmor += status.mArmor;
                gameObject.GetComponent<Player>().mStatus.mAgility += status.mAgility;
                gameObject.GetComponent<Player>().mStatus.mMagicPower += status.mMagicPower;
                gameObject.GetComponent<Player>().mStatus.mMagic_Resistance += status.mMagic_Resistance;
                gameObject.GetComponent<Player>().mStatus.mLevel += status.mLevel;
                gameObject.GetComponent<Player>().mStatus.mMaxMana += status.mMaxMana;
                gameObject.GetComponent<Player>().mStatus.mMaxHealth += status.mMaxHealth;
                gameObject.GetComponent<Player>().mStatus.mHealth += status.mHealth;
                gameObject.GetComponent<Player>().mStatus.mMana += status.mMana;
            }
        }
        if (Input.GetKeyDown(mMoneyCheatCode))
        {
            PlayerController.Instance.mGold += 10000;
        }
    }
}

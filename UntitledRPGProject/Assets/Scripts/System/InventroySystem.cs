using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class InventroySystem : MonoBehaviour
{
    public class InventoryInfo
    {
        public Weapon Weapon;
        public Armor Head;
        public Armor Body;
        public Armor Arm;
        public Armor Leg;
        public InventoryInfo()
        {
            Weapon = null;
            Head = null;
            Body = null;
            Leg = null;
            Arm = null;
        }
    }
    public InventoryInfo mInventoryInfo;
    public event System.Action mAction;
    public void Initialize()
    {
        mInventoryInfo = new InventoryInfo();
    }

    public bool Equip(object item)
    {
        bool isExisted = false;
        mAction?.Invoke();
        if (item is Weapon)
        {
            Weapon weapon = (Weapon)item;
            if(weapon.weaponType != transform.GetComponent<Unit>().mStatus.mWeaponType)
            {
                Debug.Log("It is not vaild!");
                return isExisted;
            }
            if (PlayerController.Instance.mInventory.Get(weapon.Name) == null)
                return isExisted;
            mInventoryInfo.Weapon = Check(weapon, mInventoryInfo.Weapon) as Weapon;
            return isExisted;
        }
        else if(item is Armor)
        {
            Armor armor = (Armor)item;
            if (PlayerController.Instance.mInventory.Get(armor.Name) == null)
                return isExisted;
            switch (armor.armorType)
            {
                case ArmorType.Bracer:
                    mInventoryInfo.Arm = Check(armor, mInventoryInfo.Arm) as Armor;
                    break;
                case ArmorType.BodyArmor:
                    mInventoryInfo.Body = Check(armor, mInventoryInfo.Body) as Armor;
                    break;
                case ArmorType.LegArmor:
                    mInventoryInfo.Leg = Check(armor, mInventoryInfo.Leg) as Armor;
                    break;
                case ArmorType.Helmet:
                    mInventoryInfo.Head = Check(armor, mInventoryInfo.Head) as Armor;
                    break;
            }
            return isExisted;
        }
        else
        {
            Debug.Log("<color=red> the item </color> is not vaild!");
            return isExisted;
        }
    }

    private EquipmentItem Check(EquipmentItem item, EquipmentItem current)
    {
        current = Exist(current);
        current = item;
        item.ChangeOwner(transform.GetComponent<Unit>());
        PlayerController.Instance.mInventory.Remove(current);
        current.Apply();
        UIManager.Instance.mInventoryUI.InventoryUpdate();
        return current;
    }

    public bool UnEquip(object item)
    {
        if (item is Weapon)
        {
            Weapon weapon = (Weapon)item;
            mInventoryInfo.Weapon = Exist(mInventoryInfo.Weapon) as Weapon;
            return true;
        }
        else if(item is Armor)
        {
            Armor armor = (Armor)item;
            switch (armor.armorType)
            {
                case ArmorType.Bracer:
                    mInventoryInfo.Arm = Exist(mInventoryInfo.Arm) as Armor;
                    break;
                case ArmorType.BodyArmor:
                    mInventoryInfo.Body = Exist(mInventoryInfo.Body) as Armor;
                    break;
                case ArmorType.LegArmor:
                    mInventoryInfo.Leg = Exist(mInventoryInfo.Leg) as Armor;
                    break;
                case ArmorType.Helmet:
                    mInventoryInfo.Head = Exist(mInventoryInfo.Head) as Armor;
                    break;
            }
            return true;
        }
        else
        {
            Debug.Log("<color=yellow> the item </color> is not vaild!");
            return false;
        }
    }

    private EquipmentItem Exist(EquipmentItem current)
    {
        if (current != null)
        {
            current.End();
            PlayerController.Instance.mInventory.Add(current);
            current = null;
        }
        UIManager.Instance.mInventoryUI.InventoryUpdate();
        return current;
    }

}

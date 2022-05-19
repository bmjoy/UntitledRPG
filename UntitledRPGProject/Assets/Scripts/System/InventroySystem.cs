using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventroySystem : MonoBehaviour
{
    public class InventoryInfo
    {

        public Weapon mWeapon;

        public Armor Head;
        public Armor Body;
        public Armor Arm;
        public Armor Leg;

        public InventoryInfo()
        {
            mWeapon = null;
            Head = null;
            Body = null;
            Leg = null;
            Arm = null;
        }
    }
    public InventoryInfo mInventoryInfo;
    public void Initialize()
    {
        mInventoryInfo = new InventoryInfo();
    }

    public void Equip(object item)
    {
        if (item is Weapon)
        {
            Weapon weapon = (Weapon)item;
            if(weapon.weaponType != transform.GetComponent<Unit>().mStatus.mWeaponType)
            {
                Debug.Log("It is not vaild!");
                return;
            }
            if (PlayerController.Instance.mInventory.Get(weapon.Name) == null)
                return;
            mInventoryInfo.mWeapon = Check(weapon, mInventoryInfo.mWeapon) as Weapon;
        }
        else if(item is Armor)
        {
            Armor armor = (Armor)item;
            if (PlayerController.Instance.mInventory.Get(armor.Name) == null)
                return;
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
        }
        else
            Debug.Log("<color=red> the item </color> is not vaild!");
    }

    private EquipmentItem Check(EquipmentItem item, EquipmentItem current)
    {
        current = Exist(current);
        current = item;
        item.ChangeOwner(transform.GetComponent<Unit>());
        PlayerController.Instance.mInventory.Remove(current);
        current.Apply();
        return current;
    }

    public void UnEquip(object item)
    {
        if (item is Weapon)
        {
            Weapon weapon = (Weapon)item;
            mInventoryInfo.mWeapon = Exist(mInventoryInfo.mWeapon) as Weapon;
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
        }
        else
            Debug.Log("<color=yellow> the item </color> is not vaild!");
    }

    private EquipmentItem Exist(EquipmentItem current)
    {
        if (current != null)
        {
            current.End();
            current.ChangeOwner(null);
            PlayerController.Instance.mInventory.Add(current);
            current = null;
        }
        return current;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Inventory
{
    public readonly Dictionary<string,Item> myInventory = new Dictionary<string,Item>();

    public void Add(Item item)
    {
        if ((item.GetType() == typeof(Armor) || item.GetType() == typeof(Weapon)))
        {
            if (!myInventory.ContainsKey(item.Name) && !PlayerController.Instance.CheckItemExist(item.Name))
            {
                item.Initialize();
                myInventory.Add(item.Name, item);
            }
            else
                Debug.LogWarning("<color=yellow>Warning!</color> each equipment cannot have more than one");
        }
        else if(item.GetType() == typeof(Expendables))
        {
            if(!myInventory.ContainsKey(item.Name))
            {
                item.Initialize();
                myInventory.Add(item.Name, item);
            }
            else
            {
                var i = (Expendables)Get(item.Name);
                if(i.Amount < 50)
                    myInventory[item.Name].Apply();
                else
                    Debug.LogWarning("<color=yellow>Warning!</color> expendables are reached to maximum!");
            }
        }
    }

    public void Remove(Item item)
    {
        if(myInventory.ContainsKey(item.Name))
            myInventory.Remove(item.Name);
    }

    public Item Get(string name)
    {
        return myInventory[name];
    }
}

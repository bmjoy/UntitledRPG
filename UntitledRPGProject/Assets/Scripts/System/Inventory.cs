using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Inventory
{
    public readonly List<KeyValuePair<int,Item>> myInventory = new List<KeyValuePair<int, Item>>();
    public int myIndex = 0;

    public void Add(Item item)
    {
        if ((item.GetType() == typeof(Armor) || item.GetType() == typeof(Weapon)))
        {
            item.Initialize(myIndex);
            myInventory.Add(new KeyValuePair<int, Item>(myIndex, item));
            myIndex++;
        }
        else if(item.GetType() == typeof(Expendables))
        {
            if(!myInventory.Contains(new KeyValuePair<int, Item>(item.ID, item)))
            {
                item.Initialize(myIndex);
                myInventory.Add(new KeyValuePair<int, Item>(item.ID, item));
                myIndex++;
            }
            else
            {
                var it = (Expendables)Get(item.Name);
                if(it.Amount < 50)
                    Get(item.Name).Apply();
                else
                    Debug.LogWarning("<color=yellow>Warning!</color> expendables are reached to maximum!");
            }
        }
    }

    public void Remove(Item item)
    {
        myInventory.Remove(myInventory.Find(x => x.Value.ID == item.ID));
    }

    public void Delete(Item item)
    {
        foreach(Transform it in PlayerController.Instance.transform.Find("Bag"))
        {
            if(it.GetComponent<Item>() == Get(item.Name))
            {
                GameObject.Destroy(it.gameObject);
                break;
            }
        }
        Remove(item);
    }

    public Item Get(string name)
    {
        foreach(var item in myInventory)
        {
            if(item.Value.Name == name)
                return item.Value;
        }

        return null;
    }
}

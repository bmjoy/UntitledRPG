using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expendables : Item
{
    public int Amount = 1;
    public override void Initialize(int id)
    {
        base.Initialize(id);
    }

    public override void Apply()
    {
        Amount++;
        UIManager.Instance.mInventoryUI.InventoryUpdate();
    }

    public override void End()
    {
        // Use
        Amount--;
        UIManager.Instance.mInventoryUI.InventoryUpdate();
        if(Amount <= 0)
        {
            PlayerController.Instance.mInventory.Remove(this);
            UIManager.Instance.mInventoryUI.InventoryUpdate();
            Destroy(gameObject,3.0f);
        }
    }
}

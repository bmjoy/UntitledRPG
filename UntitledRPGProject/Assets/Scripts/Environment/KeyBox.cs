using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBox : CollectableObject
{
    public int value = 1;
    bool isGained = false;
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Gain(other);
    }

    protected override void OnTriggerStay(Collider other)
    {
        Gain(other);
    }

    private void Gain(Collider other)
    {
        if (!_initialize)
            return;
        if (other.CompareTag("Player") && !isGained)
        {
            isGained = true;
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mExclamationSFX);
            if(PlayerController.Instance.mInventory.Get("Key"))
                PlayerController.Instance.mInventory.Get("Key").Apply();
            else
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Items/Expendables/Key"), PlayerController.Instance.mBag.transform);
                go.GetComponent<Item>().isSold = true;
                PlayerController.Instance.mInventory.Add(go.GetComponent<Item>());
            }

            mOwner.mCollectObjects.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}

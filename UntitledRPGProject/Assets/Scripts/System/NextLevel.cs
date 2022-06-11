using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    public bool _isBackLevel = false;
    public bool _isKeyRequired = false;

    [SerializeField]
    private Item Key;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isKeyRequired && !PlayerController.Instance.mInventory.Get(Key.Name))
                return;
            LevelManager.Instance.isNext = true;
            if (!_isBackLevel)
                StartCoroutine(LevelManager.Instance.GoNextLevel());
            else
                StartCoroutine(LevelManager.Instance.GoBackLevel());
        }
    }

}

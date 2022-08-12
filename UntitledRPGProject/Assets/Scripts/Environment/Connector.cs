using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    public GameObject mGameObject;
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.activeSelf)
        {
            if (other.GetComponent<Connector>())
            {
                other.gameObject.SetActive(false);
                Destroy(other.gameObject.GetComponent<Connector>().mGameObject);
            }
        }
    }
}

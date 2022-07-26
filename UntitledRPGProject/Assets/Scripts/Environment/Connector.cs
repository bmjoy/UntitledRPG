using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(gameObject.activeSelf)
        {
            if (collision.collider.GetComponent<Connector>())
            {
                collision.gameObject.SetActive(false);
                Destroy(collision.gameObject);
            }
        }
    }
}

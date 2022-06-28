using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private void LateUpdate()
    {
        if(PlayerController.Instance)
        {
            Vector3 newPos = PlayerController.Instance.transform.position;
            newPos.y = transform.position.y;
            transform.position = newPos;

            transform.rotation = Quaternion.Euler(90.0f, PlayerController.Instance.transform.position.y
                , 0.0f);
        }
    }
}

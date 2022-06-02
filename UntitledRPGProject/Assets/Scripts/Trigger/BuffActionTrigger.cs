using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffActionTrigger : MonoBehaviour
{
    public Buff _InitialBuff;
    void Start()
    {
        GetComponent<Unit>().SetBuff(_InitialBuff.Initialize(GetComponent<Unit>(), GetComponent<Unit>()));
    }
}

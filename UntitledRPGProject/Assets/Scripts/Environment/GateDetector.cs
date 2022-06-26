using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateDetector : MonoBehaviour
{
    private Door mDoor;
    private string mName;
    public void Initialize(Door door)
    {
        mName = (transform.name.Contains("Front")) ? "IsFrontOpen" : "IsBackOpen";
        mDoor = door;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            mDoor.SetDirection(mName);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            mDoor.SetDirection(string.Empty);
    }
}

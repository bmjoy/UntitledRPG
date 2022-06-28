using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDetector : MonoBehaviour
{
    public bool isExist = false;
    private bool isInitialized = false;
    private Room _Room;
    public void Initialize(Room room)
    {
        _Room = room;
        isInitialized = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(isInitialized)
        {
            if (other.CompareTag("Player"))
            {
                isExist = true;
                _Room.Display();
            }
        }
    }
}

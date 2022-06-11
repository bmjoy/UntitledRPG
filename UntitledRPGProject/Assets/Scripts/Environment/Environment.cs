using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public int mID;
    public string mName;

    public virtual void Initialize(int id)
    {
        mID = id;
    }
}

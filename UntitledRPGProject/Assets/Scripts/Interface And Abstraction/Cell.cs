using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public bool isVisited = false;
    public bool[] isOpened = new bool[4];

    public void Visit()
    {
        isVisited = true;
    }

    public bool GetVisited()
    {
        return isVisited;
    }

    public void Open(int i)
    {
        isOpened[i] = true;
    }
}

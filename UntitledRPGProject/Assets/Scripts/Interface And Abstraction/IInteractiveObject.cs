using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractiveObject
{
    IEnumerator Interact(Action action = null);
    void React(bool active);
    Vector3 GetPosition();
}

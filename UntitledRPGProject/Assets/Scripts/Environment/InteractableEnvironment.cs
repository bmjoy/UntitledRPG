using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableEnvironment : Environment, IInteractiveObject
{
    private GameObject mCanvas;
    [HideInInspector]
    public GameObject mInteraction;

    public virtual void Canvas_Initialize()
    {
        mCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/CanvasForNPC"), transform.position
+ new Vector3(0.0f, GetComponent<BoxCollider>().center.y + 5.0f, 0.0f), Quaternion.identity);
        mCanvas.transform.SetParent(transform);
        mCanvas.transform.localRotation = new Quaternion(0.0f, 260.0f, 0.0f, 1.0f);
        mCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.125f, 0.125f, 0.125f);
        mInteraction = mCanvas.transform.Find("Interaction").gameObject;
        mInteraction.SetActive(false);
    }

    public abstract void Reset();

    public abstract IEnumerator Interact(System.Action action = null);

    public void React(bool active)
    {
        mInteraction.SetActive(active);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}

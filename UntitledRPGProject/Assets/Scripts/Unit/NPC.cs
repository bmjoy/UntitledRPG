using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour, IInteractiveObject
{
    [HideInInspector]
    public bool mComplete = false;
    public string mName = string.Empty;
    protected bool isTrading = false;
    protected delegate IEnumerator TriggerEvent();
    protected TriggerEvent mTrigger;

    [SerializeField]
    protected List<Dialogue> m_DialogueList = new List<Dialogue>();
    protected Queue<Dialogue> m_DialogueQueue = new Queue<Dialogue>();

    [SerializeField]
    protected List<Dialogue> m_DialogueYesCase = new List<Dialogue>();
    [SerializeField]
    protected List<Dialogue> m_DialogueNoCase = new List<Dialogue>();
    [SerializeField]
    protected List<Dialogue> m_DialogueFailToTradeCase = new List<Dialogue>();
    protected Item mProperty;
    protected GameObject mCanvas;
    protected Animator mAnimator;
    public GameObject mInteraction;

    protected virtual void Start()
    {
        mCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/CanvasForNPC"), transform.position
    + new Vector3(0.0f, GetComponent<BoxCollider>().center.y + 3.5f, 0.0f), Quaternion.identity, transform);
        if(!GetComponent<Billboard>().mUseStaticBillboard)
            mCanvas.transform.localRotation = new Quaternion(0.0f, 180.0f, 0.0f, 1.0f);
        else
            mCanvas.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        mInteraction = mCanvas.transform.Find("Interaction").gameObject;
        mInteraction.SetActive(false);
        mAnimator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if(mInteraction && mInteraction.activeSelf && Vector3.Distance(transform.position, PlayerController.Instance.transform.position) > PlayerController.Instance.GetComponent<InteractSystem>().mRadius)
            mInteraction.SetActive(false);
    }

    public virtual IEnumerator Interact(Action Callback)
    {
        foreach (Dialogue dialogue in m_DialogueList)
            m_DialogueQueue.Enqueue(dialogue);
        UIManager.Instance.FadeInScreen();
        UIManager.Instance.DisplayDialogueBox(true);
        UIManager.Instance.DisplayMiniMap(false);
        UIManager.Instance.DisplaySupportKey();
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            "None",
            "None"});
        while (m_DialogueQueue.Count > 0)
        {
            var dialogue = m_DialogueQueue.Dequeue();
            UIManager.Instance.ChangeDialogueText(mName + ": " + dialogue.Text);
            switch (dialogue.Trigger)
            {
                case Dialogue.TriggerType.None:
                    yield return new WaitForSeconds(0.5f);
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    break;
                case Dialogue.TriggerType.Trade:
                    mComplete = false;
                    mTrigger = Trade;
                    break;
                case Dialogue.TriggerType.Event:
                    mComplete = false;
                    mTrigger = Event;
                    break;
                case Dialogue.TriggerType.Fail:
                case Dialogue.TriggerType.Success:
                    {
                        yield return new WaitForSeconds(0.5f);
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    }
                    // Give something
                    break;
                default:
                    break;
            }
            yield return (mTrigger != null) ? StartCoroutine(mTrigger()) : null;
            yield return (mTrigger != null) ? new WaitUntil(() => mComplete) : null;
        }
        UIManager.Instance.FadeOutScreen();
        UIManager.Instance.ChangeDialogueText("");
        UIManager.Instance.DisplayDialogueBox(false);
        UIManager.Instance.DisplayMiniMap(true);
        UIManager.Instance.DisplaySupportKey(false);
        Callback?.Invoke();
        mComplete = false;
        mTrigger = null;

    }

    public virtual IEnumerator Event()
    {
        UIManager.Instance.DisplaySupportKey(true,true,false);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Yes",
            "No",
            "None"});
        mTrigger = null;
        PlayerController.Instance.GetComponent<InteractSystem>().mRightAction += (() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        PlayerController.Instance.GetComponent<InteractSystem>().mLeftAction += (() => {
            foreach (var dialogue in m_DialogueYesCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        yield return new WaitUntil(() => mComplete);
        PlayerController.Instance.GetComponent<InteractSystem>().ResetActions();
    }

    public virtual IEnumerator Trade()
    {
        mTrigger = null;
        UIManager.Instance.DisplaySupportKey(true, true, false);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Yes",
            "No",
            string.Empty});
        PlayerController.Instance.GetComponent<InteractSystem>().mRightAction += (() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        PlayerController.Instance.GetComponent<InteractSystem>().mLeftAction += (() => {
            // Input quest?
            mComplete = true;
        });
        yield return new WaitUntil(() => mComplete);
        PlayerController.Instance.GetComponent<InteractSystem>().ResetActions();
    }

    public void React(bool active)
    {
        mInteraction.SetActive(active);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool GetComplete()
    {
        return mComplete;
    }
}
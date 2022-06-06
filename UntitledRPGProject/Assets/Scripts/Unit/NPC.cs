using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    [HideInInspector]
    public bool mComplete = false;
    public string mName = string.Empty;
    protected bool isTrading = false;
    delegate IEnumerator TriggerEvent();
    TriggerEvent mTrigger;

    [Serializable]
    public class Dialogue
    {
        public enum TriggerType
        {
            None,
            Trade,
            Event,
            Fight
        }
        [TextArea]
        public string Text = string.Empty;
        public TriggerType Trigger = TriggerType.None;
    }

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
    private GameObject mCanvas;
    public GameObject mInteraction;

    protected virtual void Start()
    {
        mCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/CanvasForNPC"), transform.position
    + new Vector3(0.0f, GetComponent<BoxCollider>().center.y + 3.5f, 0.0f), Quaternion.identity);
        mCanvas.transform.SetParent(transform);
        if(!GetComponent<Billboard>().mUseStaticBillboard)
            mCanvas.transform.localRotation = new Quaternion(0.0f, 180.0f, 0.0f, 1.0f);
        else
            mCanvas.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        mInteraction = mCanvas.transform.Find("Interaction").gameObject;
        mInteraction.SetActive(false);
    }

    public virtual IEnumerator Interact(Action Callback)
    {
        foreach (Dialogue dialogue in m_DialogueList)
        {
            m_DialogueQueue.Enqueue(dialogue);
        }
        UIManager.Instance.FadeInScreen();
        UIManager.Instance.DisplayDialogueBox(true);
        while(m_DialogueQueue.Count > 0)
        {
            var dialogue = m_DialogueQueue.Dequeue();
            UIManager.Instance.ChangeDialogueText(mName + ": " + dialogue.Text);
            switch (dialogue.Trigger)
            {
                case Dialogue.TriggerType.None:
                    yield return new WaitForSeconds(0.5f);
                    UIManager.Instance.DisplayButtonsInDialogue(false);
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
                case Dialogue.TriggerType.Fight:
                    break;
            }
            yield return (mTrigger != null) ? StartCoroutine(mTrigger()) : null;
            yield return (mTrigger != null) ? new WaitUntil(() => mComplete) : null;
        }
        UIManager.Instance.FadeOutScreen();
        UIManager.Instance.ChangeDialogueText("");
        UIManager.Instance.DisplayDialogueBox(false);
        Callback?.Invoke();
        mComplete = false;
        mTrigger = null;

    }

    public virtual IEnumerator Event()
    {
        UIManager.Instance.AddListenerNoButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.AddListenerYesButton(() => {
            // Input quest?
            mComplete = true;
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
    }

    public virtual IEnumerator Trade()
    {
        UIManager.Instance.AddListenerNoButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.AddListenerYesButton(() => {
            // Input quest?
            mComplete = true;
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
    }
}
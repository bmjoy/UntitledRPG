using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    [SerializeField]
    private NPCType m_NPCType;
    public bool mComplete = false;
    public string mName = string.Empty;
    private bool isTraded = false;
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

    [Serializable]
    public class NeedsInfo
    {
        public string Name = string.Empty;
        public int Value = 0;
        public int Amount = 0;
        [HideInInspector]
        public bool onComplete = false;
        public NeedsInfo(string n, int v, int a, bool complete = false)
        {
            Name = n;
            Value = v;
            Amount = a;
        }
    }

    [SerializeField]
    List<Dialogue> m_DialogueList = new List<Dialogue>();
    Queue<Dialogue> m_DialogueQueue = new Queue<Dialogue>();

    [SerializeField]
    List<Dialogue> m_DialogueYesCase = new List<Dialogue>();
    [SerializeField]
    List<Dialogue> m_DialogueNoCase = new List<Dialogue>();
    [SerializeField]
    List<Dialogue> m_DialogueFailToTradeCase = new List<Dialogue>();

    [SerializeField]
    private Item mProperty;

    [SerializeField]
    List<NeedsInfo> m_NeedsList = new List<NeedsInfo>();

    private void Start()
    {
        if(m_NPCType == NPCType.Hero)
        {
            mProperty = ((Companion)mProperty != null) ? (Companion)mProperty
                : Resources.Load<Companion>("Prefabs/Items/Companions/" + mName);
            Companion companion = (Companion)mProperty;
            companion.mTransform = transform;
        }
    }

    public IEnumerator Interact(Action Callback)
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
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    break;
                case Dialogue.TriggerType.Trade:
                    break;
                case Dialogue.TriggerType.Event:
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
        if(m_NPCType == NPCType.Hero && isTraded)
        {
            Destroy(gameObject, 0.5f);
        }
        StopAllCoroutines();
    }

    private IEnumerator Event()
    {
        UIManager.Instance.AddListenerNoButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true; 
        });
        UIManager.Instance.AddListenerYesButton(() => {
            foreach(var val in m_NeedsList)
            {
                switch(val.Name)
                {
                    case "Money":
                        {
                            if (val.Value <= PlayerController.Instance.mGold)
                            {
                                PlayerController.Instance.mGold -= val.Value;
                                val.onComplete = true;
                            }
                        }
                        break;
                    case "Item":
                        //TODO: Item quest
                        break;
                    default:
                        break;
                }
            }

            isTraded = m_NeedsList.TrueForAll(t => t.onComplete == true);

            if (isTraded)
            {
                foreach (var dialogue in m_DialogueYesCase)
                    m_DialogueQueue.Enqueue(dialogue);
                mProperty.Apply();
            }
            else
            {
                foreach (var dialogue in m_DialogueFailToTradeCase)
                    m_DialogueQueue.Enqueue(dialogue);
            }
            mComplete = true; 
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableEnvironment
{
    [SerializeField]
    private List<ItemDrop> mItems = new List<ItemDrop>();

    Queue<Dialogue> m_DialogueQueue = new Queue<Dialogue>();
    [SerializeField]
    protected List<Dialogue> m_DialogueYesCase = new List<Dialogue>();
    [SerializeField]
    protected List<Dialogue> m_DialogueNoCase = new List<Dialogue>();
    protected delegate IEnumerator TriggerEvent();
    protected TriggerEvent mTrigger;
    private bool _DialogueComplete = false;
    private Animator mAnimator;

    public override void Initialize(int id)
    {
        base.Initialize(id);
        Canvas_Initialize();
    }

    public override IEnumerator Interact(Action action = null)
    {
        mAnimator = transform.Find("Object").GetComponent<Animator>();
        foreach (Dialogue dialogue in mDialogue)
        {
            m_DialogueQueue.Enqueue(dialogue);
        }
        UIManager.Instance.DisplayDialogueBox(true);
        while (m_DialogueQueue.Count > 0)
        {
            var dialogue = m_DialogueQueue.Dequeue();
            UIManager.Instance.ChangeDialogueText("Jimmy" + ": " + dialogue.Text);
            switch (dialogue.Trigger)
            {
                case Dialogue.TriggerType.None:
                    {
                        yield return new WaitForSeconds(0.5f);
                        UIManager.Instance.DisplayButtonsInDialogue(false);
                        UIManager.Instance.DisplayEKeyInDialogue(true);
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    }
                    break;
                case Dialogue.TriggerType.Event:
                    UIManager.Instance.DisplayEKeyInDialogue(false);
                    _DialogueComplete = false;
                    mTrigger = Event;
                    break;
                case Dialogue.TriggerType.Success:
                    {
                        yield return new WaitForSeconds(0.5f);
                        UIManager.Instance.DisplayButtonsInDialogue(false);
                        UIManager.Instance.DisplayEKeyInDialogue(true);
                        // Give the item
                        mAnimator.SetBool("IsOpen", true);
                        AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mItemPurchaseSFX);
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                        _Completed = true;
                    }
                    break;
                case Dialogue.TriggerType.Trade:
                    break;
                case Dialogue.TriggerType.Fail:
                    {
                        yield return new WaitForSeconds(0.5f);
                        UIManager.Instance.DisplayButtonsInDialogue(false);
                        UIManager.Instance.DisplayEKeyInDialogue(true);
                        // Give the item
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                        _Completed = true;
                    }
                    break;
            }
            yield return (mTrigger != null) ? StartCoroutine(mTrigger()) : null;
            yield return (mTrigger != null) ? new WaitUntil(() => _DialogueComplete) : null;
        }
        UIManager.Instance.ChangeDialogueText("");
        UIManager.Instance.DisplayDialogueBox(false);
        UIManager.Instance.DisplayEKeyInDialogue(false);
        action?.Invoke();
        if (mItems.Count == 0 && _Completed)
        {
            mAnimator.SetBool("IsOpen", false);
        }
    }

    public override void Reset()
    {
        _Completed = false;
        mAnimator = transform.Find("Object").GetComponent<Animator>();
    }

    public IEnumerator Event()
    {
        mTrigger = null;
        UIManager.Instance.ChangeTwoButtons(UIManager.Instance.mStorage.YesButtonImage,
    UIManager.Instance.mStorage.NoButtonImage);
        UIManager.Instance.AddListenerRightButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            _DialogueComplete = true;
        });
        UIManager.Instance.AddListenerLeftButton(() => {
            foreach (var dialogue in m_DialogueYesCase)
                m_DialogueQueue.Enqueue(dialogue);
            _DialogueComplete = true;
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        yield return new WaitUntil(() => _DialogueComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : NPC
{
    SlotManager mSlotManager;
    
    protected override void Start()
    {
        base.Start();
        mSlotManager = GetComponent<SlotManager>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            mComplete = true;
    }

    public override IEnumerator Event()
    {
        mTrigger = null;
        UIManager.Instance.AddListenerNoButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.AddListenerYesButton(() => {
            foreach (var dialogue in m_DialogueYesCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
    }

    public override IEnumerator Trade()
    {
        UIManager.Instance.AddListenerExitButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        mSlotManager.StartTrade();
        UIManager.Instance.DisplayExitButtonInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        mSlotManager.EndTrade();
        UIManager.Instance.DisplayExitButtonInDialogue(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : NPC
{
    protected override void Start()
    {
        base.Start();
    }

    public override IEnumerator Event()
    {
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
        Debug.Log("Hi");
        UIManager.Instance.DisplayButtonsInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
    }

    public override IEnumerator Trade()
    {
        // TODO: make end button to finish the trade
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monk : NPC
{
    private bool isTraded = false;
    protected override void Start()
    {
        base.Start();
        Destroy(mInteraction.GetComponent<Billboard>());
        isTraded = false;
    }

    public override IEnumerator Event()
    {
        mTrigger = null;
        UIManager.Instance.ChangeTwoButtons(UIManager.Instance.mStorage.YesButtonImage,
UIManager.Instance.mStorage.NoButtonImage);
        UIManager.Instance.AddListenerRightButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.AddListenerLeftButton(() => {
            foreach (var dialogue in m_DialogueYesCase)
                m_DialogueQueue.Enqueue(dialogue);
            isTraded = true;
            mComplete = true;
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        UIManager.Instance.DisplayEKeyInDialogue(false);
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

        // TODO: Show pop-up for skill tree

        UIManager.Instance.DisplayExitButtonInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayExitButtonInDialogue(false);
        isTraded = false;
    }
}

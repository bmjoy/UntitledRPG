using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : NPC
{
    [SerializeField]
    protected Dialogue m_DialogueSuccessCase;
    [SerializeField]
    protected Dialogue m_DialogueFailCase;

    // Quest

    protected override void Start()
    {
        base.Start();
        if (mProperty != null)
            mProperty.Initialize(-1);
    }

    public override IEnumerator Interact(Action Callback)
    {
        foreach (Dialogue dialogue in m_DialogueList)
        {
            m_DialogueQueue.Enqueue(dialogue);
        }
        UIManager.Instance.FadeInScreen();
        UIManager.Instance.DisplayDialogueBox(true);
        while (m_DialogueQueue.Count > 0)
        {
            var dialogue = m_DialogueQueue.Dequeue();
            UIManager.Instance.ChangeDialogueText(mName + ": " + dialogue.Text);
            yield return new WaitForSeconds(0.5f);
            switch (dialogue.Trigger)
            {
                case Dialogue.TriggerType.None:
                    IconEnable();
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    break;
                case Dialogue.TriggerType.Event:
                    mComplete = false;
                    mTrigger = Event;
                    break;
                case Dialogue.TriggerType.Fail:
                    {
                        IconEnable();
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                        if(mProperty.GetType().IsAssignableFrom(typeof(EnemyTrap)))
                        {
                            EnemyTrap trap = (EnemyTrap)mProperty;
                            trap.mTransform = transform;
                            trap.Apply();
                            Destroy(this.gameObject, 1.0f);
                        }
                    }
                    break;
                case Dialogue.TriggerType.Success:
                    {
                        // TODO: Quest completed
                        IconEnable();
                        if (!mProperty.GetType().IsAssignableFrom(typeof(EquipmentItem)))
                            mProperty.Apply();
                        else
                        {
                            GameObject newItem = new GameObject(mProperty.Name);
                            newItem.AddComponent<EquipmentItem>();
                            newItem.GetComponent<EquipmentItem>().Info = mProperty.Info;
                            newItem.GetComponent<EquipmentItem>().Initialize(mProperty.ID);
                            PlayerController.Instance.mInventory.Add(newItem.GetComponent<Item>());
                            newItem.transform.SetParent(PlayerController.Instance.mBag.transform);
                        }
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    }
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
        UIManager.Instance.DisplayEKeyInDialogue(false);
        Callback?.Invoke();
        mComplete = false;
        mTrigger = null;
    }

    private void IconEnable()
    {
        UIManager.Instance.DisplayButtonsInDialogue(false);
        UIManager.Instance.DisplayEKeyInDialogue(true);
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
            mComplete = true;
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        UIManager.Instance.DisplayEKeyInDialogue(false);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
        //TODO: Quest?
    }
}

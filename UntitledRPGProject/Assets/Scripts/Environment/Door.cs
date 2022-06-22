using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableEnvironment, ILockable
{
    private bool isLock = false;
    private bool isOpen = false;
    private MeshRenderer mRenderer;
    private Animator mDoorAnimator;
    public GameObject mGate;
    private void Start()
    {
        Transform doorObject = transform.Find("DoorObject");
        mDoorAnimator = doorObject.GetComponent<Animator>();
        doorObject.GetComponent<MeshRenderer>().sortingOrder = 20;
        mRenderer = mGate.GetComponent<MeshRenderer>();
        mRenderer.sortingOrder = 20;
    }

    public override void Initialize(int id)
    {
        base.Initialize(id);
        Canvas_Initialize();
        //isLock = (UnityEngine.Random.Range(0, 100) <= 40) ? true : false;
    }

    public override IEnumerator Interact(Action action = null)
    {
        action?.Invoke();
        if(isOpen)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            Key key = PlayerController.Instance.mInventory.Get("Key") as Key;
            foreach (Dialogue dialogue in mDialogue)
            {
                m_DialogueQueue.Enqueue(dialogue);
            }
            if (IsLocked())
            {
                if (key != null)
                {
                    key.End();
                    UnLock();
                    isOpen = true;
                    mDoorAnimator.SetBool("IsOpen", true);
                    AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mDoorSFX);
                }
                else
                {
                    UIManager.Instance.DisplayDialogueBox(true);
                    yield return new WaitForSeconds(0.5f);
                    EnableIcon();
                    UIManager.Instance.ChangeDialogueText("Jimmy" + ": " + m_DialogueFailCase.Text);
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    UIManager.Instance.ChangeDialogueText("");
                    UIManager.Instance.DisplayDialogueBox(false);
                    UIManager.Instance.DisplayEKeyInDialogue(false);
                }
            }
            else
            {
                isOpen = true;
                mDoorAnimator.SetBool("IsOpen", true);
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mDoorSFX);
            }

        }

    }
    private void EnableIcon()
    {
        UIManager.Instance.DisplayButtonsInDialogue(false);
        UIManager.Instance.DisplayEKeyInDialogue(true);
    }
    public bool IsLocked()
    {
        return isLock;
    }

    public void UnLock()
    {
        isLock = true;
    }
    public override void Reset()
    {
        _Completed = false;
        // TODO: Close the gate
    }

}

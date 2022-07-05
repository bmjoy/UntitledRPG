using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    public float mRadius = 10.0f;
    [SerializeField]
    private float mCoolTime = 0.5f;
    private float mCurrentCoolTime = 0.0f;
    private IInteractiveObject mClosestNPC = null;
    public bool IsInteracting = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerController.Instance.onBattle) return;
        if (mCurrentCoolTime > 0.0f)
            return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, mRadius, LayerMask.GetMask("NPC"));
        if (colliders.Length == 0)
        {
            IsInteracting = false;
            return;
        }
        for (int i = 0; i < colliders.Length; ++i)
        {
            var hit = colliders[i];
            if (hit.transform.GetComponent<IInteractiveObject>().GetComplete())
                continue;
            if(Vector3.Distance(hit.transform.position, transform.position) < mRadius)
            {
                mClosestNPC = hit.transform.GetComponent<IInteractiveObject>();
                mClosestNPC.React(true);
            }
            else
            {
                hit.transform.GetComponent<IInteractiveObject>().React(false);
            }
        }

        if(mClosestNPC != null)
        {
            Vector3 pos = mClosestNPC.GetPosition();
            if (Vector3.Distance(pos, transform.position) > mRadius)
            {
                mClosestNPC.React(false);
                mClosestNPC = null;
            }
        }

    }

    private void Update()
    {
        if (PlayerController.Instance.onBattle)
            return;
        if (mCurrentCoolTime > 0.0f)
        {
            mCurrentCoolTime -= Time.deltaTime;
            return;
        }
        if (mClosestNPC != null && !IsInteracting && Input.GetKeyDown(KeyCode.E))
        {
            if (UIManager.Instance.mInventoryUI.transform.gameObject.activeSelf)
                UIManager.Instance.DisplayInventory(false);
            IsInteracting = true;
            PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
            PlayerController.Instance.mState = new IdleState();
            StartCoroutine(
                mClosestNPC.Interact(() => { IsInteracting = false; mCurrentCoolTime += mCoolTime;
                    mClosestNPC.React(false);
                    mClosestNPC = null;
                }));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    [SerializeField]
    private float mRadius = 10.0f;
    [SerializeField]
    private float mCoolTime = 0.5f;
    private float mCurrentCoolTime = 0.0f;
    private NPC mClosestNPC = null;
    public bool IsInteracting = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerController.Instance.onBattle) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, mRadius, LayerMask.GetMask("NPC"));
        if (colliders.Length == 0)
        {
            IsInteracting = false;
            return;
        }
        for (int i = 0; i < colliders.Length; ++i)
        {
            var hit = colliders[i];
            if(Vector3.Distance(hit.transform.position, transform.position) < mRadius)
            {
                mClosestNPC = hit.transform.GetComponent<NPC>();
                mClosestNPC.mInteraction.SetActive(true);
            }
            else
            {
                hit.transform.GetComponent<NPC>().mInteraction.SetActive(false);
            }
        }
        if(mClosestNPC != null)
        {
            if (Vector3.Distance(mClosestNPC.transform.position, transform.position) > mRadius)
            {
                mClosestNPC.mInteraction.SetActive(false);
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
        if (mClosestNPC && !IsInteracting && Input.GetKeyDown(KeyCode.E))
        {
            if (mClosestNPC.mComplete)
                return;
            PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
            PlayerController.Instance.mState = new IdleState();
            IsInteracting = true;
            mClosestNPC.StartCoroutine(
                mClosestNPC.Interact(() => { IsInteracting = false; mCurrentCoolTime += mCoolTime;
                    mClosestNPC = null;
                }));
        }
    }
}

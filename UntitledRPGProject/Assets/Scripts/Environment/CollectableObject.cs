using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    protected BreakableObject mOwner;
    private Vector3 mPos;
    [SerializeField]
    protected float Radius = 6.0f;
    protected bool _initialize = false;
    void Update()
    {
        if (!_initialize)
            return;
        if(Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < Radius)
            transform.position = Vector3.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * 10.0f);
    }

    public virtual IEnumerator Initialize(BreakableObject owner)
    {
        mOwner = owner;
        mPos = mOwner.transform.position;
        yield return new WaitForSeconds(1.5f);
        _initialize = true;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            transform.position = new Vector3(transform.position.x,mPos.y,transform.position.z) + new Vector3(0.0f, 0.5f, 0.0f);
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            transform.position = new Vector3(transform.position.x, mPos.y, transform.position.z) + new Vector3(0.0f, 0.5f, 0.0f);
        }
    }
}

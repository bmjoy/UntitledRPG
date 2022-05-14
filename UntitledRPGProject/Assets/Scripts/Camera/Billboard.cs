using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mCamera;
    public bool mUseStaticBillboard;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        mCamera = Camera.main;
        transform.LookAt(mCamera.transform);
        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
    }

    void LateUpdate()
    {
        if(mCamera == null)
            mCamera = Camera.main;

        if (!mUseStaticBillboard)
            transform.LookAt(mCamera.transform);
        else
            transform.rotation = mCamera.transform.rotation;
        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
    }
}

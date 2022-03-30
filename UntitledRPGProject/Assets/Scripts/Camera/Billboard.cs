using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mCamera;
    public bool mUseStaticBillboard;
    // Start is called before the first frame update
    void Start()
    {
        mCamera = Camera.main;
    }

    
    void LateUpdate()
    {
        if (!mUseStaticBillboard)
            transform.LookAt(mCamera.transform);
        else
            transform.rotation = mCamera.transform.rotation;
        transform.rotation = Quaternion.Euler(0.0f,transform.rotation.eulerAngles.y,0.0f);
    }
}

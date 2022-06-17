using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BreakableObject : InteractableEnvironment
{
    [SerializeField]
    private int mAmount = 5;
    [SerializeField]
    private float mKeyChance = 30.0f;
    private BoxCollider mCollider;
    [SerializeField]
    public List<GameObject> mCollectObjects = new List<GameObject>();
    public override IEnumerator Interact(Action action = null)
    {
        PlayerController.Instance.mModel.GetComponent<Animator>().Play("Attack");
        yield return new WaitForSeconds(0.5f);
        AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mBreakSFX);
        Material mat = transform.GetComponent<Renderer>().material;
        Color color = mat.color;
        if (!_Completed)
        {
            float a = 1.0f;
            StartCoroutine(Destorying());
            while(a > 0.01f)
            {
                Color NewColor = new Color(color.r, color.g, color.b, a);
                a -= Time.deltaTime;
                mat.SetFloat("__Surface", 1.0f);
                mat.color = NewColor;
                yield return new WaitForSeconds(0.002f);
            }
            mCollider.enabled = false;
            _Completed = true;
            mInteraction.SetActive(false);
            action?.Invoke();
        }
        yield return null;
    }

    IEnumerator Destorying()
    {
        Material mat = transform.GetComponent<Renderer>().material;
        int i = 0;
        while(mat.color.a > 0.1f)
        {
            Vector3 randomPos = transform.position + new Vector3(Random.Range(-2.8f, 2.8f),
                Random.Range(0.5f, 2.8f), Random.Range(-2.8f, 2.8f));
            GameObject f = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/RockFrag"),
                randomPos, Quaternion.identity, transform);
            Destroy(f, 0.5f);
            if (i < mCollectObjects.Count && mat.color.a < 0.5f)
            {
                var obj = mCollectObjects[i];
                obj.SetActive(true);
                StartCoroutine(obj.GetComponent<CollectableObject>().Initialize(this));
                obj.transform.position = new Vector3(randomPos.x, transform.position.y + 1.5f, randomPos.z);
                obj.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-10.0f, 10.0f), 60.5f, Random.Range(-10.0f, 10.0f)), ForceMode.Impulse);
                i++;
            }
            yield return new WaitForSeconds(0.032f);
        }
    }
    public override void Reset()
    {
        _Completed = false;
        mCollider = GetComponent<BoxCollider>();
        mCollider.enabled = true;
        mCollectObjects.Clear();
    }

    public override void Initialize(int id)
    {
        base.Initialize(id);
        Canvas_Initialize();
        mCollider = GetComponent<BoxCollider>();
        mCollectObjects.Clear();
        bool key = false;
        int amount = Random.Range(1, mAmount + 1);
        for (int i = 0; i < amount; ++i)
        {
            if(Random.Range(0.0f, 100.0f) > mKeyChance)
                NewCoin();
            else
            {
                if(!key)
                {
                    key = true;
                }
                else
                    NewCoin();
            }
        }
    }

    private void NewCoin()
    {
        GameObject coin = Instantiate(Resources.Load<GameObject>("Prefabs/Environments/Coin"), transform.position, Quaternion.identity, transform);
        coin.SetActive(false);
        mCollectObjects.Add(coin);
    }
}

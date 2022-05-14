using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        Initialize(); 
    }
    public void Initialize()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.onBattle)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("NeutralHero")))
        {
            if(Vector3.Distance(playerController.transform.position, hit.transform.position) > 5.0f)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                hit.transform.gameObject.layer = LayerMask.NameToLayer("Ally");
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/" + hit.transform.name.Substring(0,hit.transform.name.Length - 10)),transform.position,Quaternion.identity);
                go.transform.SetParent(transform);
                go.GetComponent<Unit>().ResetUnit();
                go.SetActive(false);
                playerController.mHeroes.Add(go);
                Destroy(hit.transform.gameObject, 1.0f);
            }
        }
    }
}

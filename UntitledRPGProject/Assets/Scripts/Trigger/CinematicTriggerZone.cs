using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CinematicTriggerZone : MonoBehaviour
{
    [SerializeField]
    private List<CinematicEventMethod> events = new List<CinematicEventMethod>();
    private GameObject mTarget;
    private Transform mPosition;
    private bool isMove = false;

    private void Update()
    {
        if(isMove)
        {
            mTarget.transform.position = Vector3.MoveTowards(mTarget.transform.position, mPosition.position, Time.deltaTime * 3.0f);
        }
    }

    private IEnumerator Trigger()
    {
        foreach(var e in events)
        {
            mTarget = null;
            if (e.Target && e.Type != CinematicEventMethod.CinematicEventType.None)
            {
                mTarget = e.Target.GetComponent<Spawner>().mObject;
                GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow =
GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = mTarget.transform;
            }
            mPosition = e.Position;
            isMove = false;

            switch(e.Type)
            {
                case CinematicEventMethod.CinematicEventType.FadeIn:
                    UIManager.Instance.FadeInScreen();
                    yield return new WaitForSeconds(e.Time);
                    break;
                case CinematicEventMethod.CinematicEventType.FadeOut:
                    UIManager.Instance.FadeOutScreen();
                    yield return new WaitForSeconds(e.Time);
                    break;
                case CinematicEventMethod.CinematicEventType.Dialogue:
                    {
                        if (mTarget.GetComponent<EnemyProwler>())
                        {
                            EnemyProwler prowler = mTarget.GetComponent<EnemyProwler>();
                            prowler.mTextCanvas.SetActive(true);
                            prowler.mTextCanvas.transform.Find("Borader").Find("Dialogue").GetComponent<TextMeshProUGUI>().text = e.Dialogue;
                            yield return new WaitForSeconds(e.Time);
                            prowler.mTextCanvas.SetActive(false);
                        }
                        else if (mTarget.GetComponent<PlayerController>())
                        {
                            PlayerController.Instance.mCanvas.SetActive(true);
                            PlayerController.Instance.mCanvas.transform.Find("Borader").Find("Dialogue").GetComponent<TextMeshProUGUI>().text = e.Dialogue;
                            yield return new WaitForSeconds(e.Time);
                            PlayerController.Instance.mCanvas.SetActive(false);
                        }
                    }
                    break;                
                case CinematicEventMethod.CinematicEventType.Animation:
                    {
                        if (mTarget.GetComponent<EnemyProwler>())
                        {
                            Prowler prowler = mTarget.GetComponent<EnemyProwler>();
                            prowler.mModel.GetComponent<Animator>().Play(e.AnimationName);
                        }
                        else if (mTarget.GetComponent<PlayerController>())
                            PlayerController.Instance.mModel.GetComponent<Animator>().Play(e.AnimationName);
                        yield return new WaitForSeconds(e.Time);

                    }
                    break;
                case CinematicEventMethod.CinematicEventType.Move:
                    {
                        isMove = true;
                        if (mTarget.GetComponent<EnemyProwler>())
                        {
                            EnemyProwler prowler = mTarget.GetComponent<EnemyProwler>();
                            prowler.mModel.GetComponent<Animator>().SetFloat("Speed", 1.0f);
                            yield return new WaitUntil(() => Vector3.Distance(mTarget.transform.position,
                                mPosition.position) <= 0.5f);
                            prowler.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
                        }
                        else if (mTarget.GetComponent<PlayerController>())
                        {
                            PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 1.0f);
                            yield return new WaitUntil(() => Vector3.Distance(mTarget.transform.position,
    mPosition.position) <= 0.5f);
                            PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
                        }
                        yield return new WaitForSeconds(e.Time);
                    }
                    break;
                case CinematicEventMethod.CinematicEventType.MoveAndDialogue:
                    {
                        isMove = true;
                        if (mTarget.GetComponent<EnemyProwler>())
                        {
                            EnemyProwler prowler = mTarget.GetComponent<EnemyProwler>();
                            prowler.mModel.GetComponent<Animator>().SetFloat("Speed", 1.0f);
                            prowler.mTextCanvas.SetActive(true);
                            prowler.mTextCanvas.transform.Find("Borader").Find("Dialogue").GetComponent<TextMeshProUGUI>().text
                                = e.Dialogue;
                            yield return new WaitUntil(() => Vector3.Distance(mTarget.transform.position,mPosition.position) <= 0.5f);
                            prowler.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
                            prowler.mTextCanvas.SetActive(false);
                        }
                        else if (mTarget.GetComponent<PlayerController>())
                        {
                            PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 1.0f);
                            PlayerController.Instance.mCanvas.SetActive(true);
                            PlayerController.Instance.mCanvas.transform.Find("Borader").
                                Find("Dialogue").GetComponent<TextMeshProUGUI>().text
                                = e.Dialogue;
                            yield return new WaitUntil(() => Vector3.Distance(mTarget.transform.position,
    mPosition.position) <= 0.5f);
                            PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
                            PlayerController.Instance.mCanvas.SetActive(false);
                        }
                    }
                    yield return new WaitForSeconds(e.Time);
                    break;
                case CinematicEventMethod.CinematicEventType.Teleport:
                    {
                        if (mTarget.GetComponent<Prowler>())
                        {
                            Prowler prowler = mTarget.GetComponent<Prowler>();
                            mTarget.transform.position = e.Position.position;
                        }
                        else
                            mTarget.transform.position = e.Position.position;
                        yield return new WaitForSeconds(e.Time);
                    }

                    break;
                case CinematicEventMethod.CinematicEventType.None:
                    yield return new WaitForSeconds(e.Time);
                    break;
            }
        }
        GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow =
GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = PlayerController.Instance.transform;
        GameManager.Instance.IsCinematicEvent = false;
        GameManager.Instance.ControlAllProwlers(false);
        mTarget = null;
        isMove = false;
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().isTrigger = GetComponent<BoxCollider>().enabled = false;
            GameManager.Instance.IsCinematicEvent = true;
            GameManager.Instance.ControlAllProwlers(true);
            PlayerController.Instance.mState = new IdleState();
            PlayerController.Instance.transform.GetComponentInChildren<Animator>().SetFloat("Speed", 0.0f);
            StartCoroutine(Trigger());
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 size = GetComponent<BoxCollider>().size;
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position, size);
    }
}

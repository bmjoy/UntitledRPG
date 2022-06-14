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
    private float _Speed = 0.0f;
    private Vector3 vel = Vector3.zero;
    private bool isAnotherSide = false;
    private void Update()
    {
        if(isMove)
        {
            mTarget.transform.position = Vector3.MoveTowards(mTarget.transform.position, mPosition.position, Time.deltaTime * _Speed);
        }
    }

    private IEnumerator Trigger()
    {
        foreach(var e in events)
        {
            mTarget = null;
            mPosition = e.Position;
            if (e.Target && e.Type != CinematicEventMethod.CinematicEventType.None)
            {
                mTarget = e.Target.GetComponent<Spawner>().mObject;
                GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow =
GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = mTarget.transform;
                vel = Vector3.Normalize(mPosition.position - mTarget.transform.position);
                isAnotherSide = (vel.x > 0.0f);
            }
            _Speed = e.Speed;
            isMove = false;

            switch(e.Type)
            {
                case CinematicEventMethod.CinematicEventType.FadeIn:
                    if (e.Clip)
                        AudioManager.PlaySfx(e.Clip);
                    UIManager.Instance.FadeInScreen();
                    yield return new WaitForSeconds(e.Time);
                    break;
                case CinematicEventMethod.CinematicEventType.FadeOut:
                    if (e.Clip)
                        AudioManager.PlaySfx(e.Clip);
                    UIManager.Instance.FadeOutScreen();
                    yield return new WaitForSeconds(e.Time);
                    break;
                case CinematicEventMethod.CinematicEventType.Dialogue:
                    {
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
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
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
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
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
                        isMove = true;
                        if (mTarget.GetComponent<EnemyProwler>())
                        {
                            EnemyProwler prowler = mTarget.GetComponent<EnemyProwler>();
                            prowler.mModel.GetComponent<Animator>().SetFloat("Speed", 1.0f);
                            prowler.mModel.GetComponent<SpriteRenderer>().flipX = !isAnotherSide;
                            yield return new WaitUntil(() => Vector3.Distance(mTarget.transform.position,
                                mPosition.position) <= 0.5f);
                            prowler.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
                        }
                        else if (mTarget.GetComponent<PlayerController>())
                        {
                            PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 1.0f);
                            PlayerController.Instance.mModel.GetComponent<SpriteRenderer>().flipX = !isAnotherSide;
                            yield return new WaitUntil(() => Vector3.Distance(mTarget.transform.position,
    mPosition.position) <= 0.5f);
                            PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
                        }
                        yield return new WaitForSeconds(e.Time);
                    }
                    break;
                case CinematicEventMethod.CinematicEventType.MoveAndDialogue:
                    {
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
                        isMove = true;
                        if (mTarget.GetComponent<EnemyProwler>())
                        {
                            EnemyProwler prowler = mTarget.GetComponent<EnemyProwler>();
                            prowler.mModel.GetComponent<Animator>().SetFloat("Speed", 1.0f);
                            prowler.mModel.GetComponent<SpriteRenderer>().flipX = !isAnotherSide;
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
                            PlayerController.Instance.mModel.GetComponent<SpriteRenderer>().flipX = !isAnotherSide;
                            PlayerController.Instance.mCanvas.SetActive(true);
                            PlayerController.Instance.mCanvas.transform.Find("Borader").
                                Find("Dialogue").GetComponent<TextMeshProUGUI>().text
                                = e.Dialogue;
                            yield return new WaitUntil(() => Vector3.Distance(mTarget.transform.position,
    mPosition.position) <= 0.5f);
                            PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
                            PlayerController.Instance.mCanvas.SetActive(false);
                        }
                        yield return new WaitForSeconds(e.Time);
                    }
                    break;
                case CinematicEventMethod.CinematicEventType.Teleport:
                    {
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
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
                case CinematicEventMethod.CinematicEventType.PlayCameraShake:
                    {
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
                        StartCoroutine(CameraSwitcher.Instance.PlayShakeCameraGameWorld());
                        yield return new WaitForSeconds(e.Time);
                    }
                    break;
                case CinematicEventMethod.CinematicEventType.StopCameraShake:
                    {
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
                        StartCoroutine(CameraSwitcher.Instance.StopShakeCameraGameWorld());
                        yield return new WaitForSeconds(e.Time);
                    }
                    break;
                case CinematicEventMethod.CinematicEventType.CameraZoom:
                    {
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
                        StartCoroutine(CameraSwitcher.Instance.ZoomCameraGameWorld(e.Time, e.MaxZoom, mTarget.transform.position));
                    }
                    break;
                case CinematicEventMethod.CinematicEventType.PlayMusic:
                    {
                        AudioManager.Instance.musicSource.clip = e.Clip;
                        AudioManager.Instance.musicSource.Play();
                        yield return new WaitForSeconds(e.Time);
                    }
                    break;
                case CinematicEventMethod.CinematicEventType.TargetEffect:
                    {
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
                        GameObject effect = Instantiate(e.Effect, mTarget.transform.position, Quaternion.identity);
                        Destroy(effect, e.Time);
                        yield return new WaitForSeconds(e.Time);
                    }
                    break;
                case CinematicEventMethod.CinematicEventType.PositionEffect:
                    {
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
                        GameObject effect = Instantiate(e.Effect, e.Position.position, Quaternion.identity);
                        Destroy(effect, e.Time);
                        yield return new WaitForSeconds(e.Time);
                    }
                    break;
                case CinematicEventMethod.CinematicEventType.None:
                    {
                        if (e.Clip)
                            AudioManager.PlaySfx(e.Clip);
                        yield return new WaitForSeconds(e.Time);
                    }
                    break;
            }
        }
        GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow =
GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = PlayerController.Instance.transform;
        GameManager.Instance.IsCinematicEvent = false;
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
            PlayerController.Instance.mState = new IdleState();
            PlayerController.Instance.transform.GetComponentInChildren<Animator>().SetFloat("Speed", 0.0f);
            AudioManager.FadeOutMusic();
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

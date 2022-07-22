using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public EnemySpawner _EnemySpawner;
    private EnemyProwler _EnemyProwler;

    public EnvironmentSpawner _EnvironmentSpawner;
    private BreakableObject _BreakableObject;

    [SerializeField]
    private List<Dialogue> m_DialogueList = new List<Dialogue>();
    private Queue<Dialogue> m_DialogueQueue = new Queue<Dialogue>();

    [SerializeField]
    private Dialogue m_DialogueYesCase;
    [SerializeField]
    private Dialogue m_DialogueNoCase;

    [SerializeField]
    private List<Dialogue> m_DialogueMovementList = new List<Dialogue>();

    [SerializeField]
    private Dialogue m_DialogueMoveSuccess;

    [SerializeField]
    private List<Dialogue> m_DialogueInteractList = new List<Dialogue>();

    [SerializeField]
    private Dialogue m_DialogueInteractSuccess;

    [SerializeField]
    private List<Dialogue> m_DialogueBattleList = new List<Dialogue>();

    [SerializeField]
    private Dialogue m_DialogueBattleHelp;

    [SerializeField]
    private Dialogue m_DialogueBattleSuccess;

    [SerializeField]
    private List<Dialogue> m_DialogueEndList = new List<Dialogue>();

    [SerializeField]
    private List<Dialogue> m_DialogueEndSuccessList = new List<Dialogue>();

    private bool mComplete = false;
    private bool mEvent = false;

    private bool mMoveMission = false;
    private bool mInteractMission = false;
    private bool mBattleMission = false;
    private bool mEndMission = false;
    private bool mSecondaryEndMission = false;

    [SerializeField]
    private GameObject mItem;
    private float _MoveTimer = 0.0f;
    public enum TutorialType
    {
        None,
        Intro,
        Movement,
        Interact,
        Battle,
        End,
        Death
    }
    public TutorialType Type = TutorialType.None;

    private void Awake()
    {
        GameManager.Instance.IsCinematicEvent = true;
    }

    void Start()
    {
        _MoveTimer = 0.0f;
        Type = TutorialType.None;
        StartCoroutine(BeginTutorial());
    }

    void Update()
    {
        switch (Type)
        {
            case TutorialType.None:
                break;
            case TutorialType.Intro:
                if (Input.GetKeyDown(KeyCode.E) && !mEvent)
                {
                    if (m_DialogueQueue.Count > 0)
                    {
                        var dialogue = m_DialogueQueue.Dequeue();
                        UIManager.Instance.ChangeDialogueText(dialogue.Text);
                        mEvent = (dialogue.Trigger == Dialogue.TriggerType.Event) ? true : false;
                        if(mEvent)
                            StartCoroutine(Event());
                    }
                }
                break;
            case TutorialType.Movement:
                if (Input.GetKeyDown(KeyCode.E) && !mEvent)
                {
                    if (m_DialogueQueue.Count > 0)
                    {
                        var dialogue = m_DialogueQueue.Dequeue();
                        UIManager.Instance.ChangeDialogueText(dialogue.Text);
                        
                        if (dialogue.Trigger == Dialogue.TriggerType.Event && !mMoveMission)
                        {
                            mEvent = true;
                            GameManager.Instance.IsCinematicEvent = false;
                            UIManager.Instance.DisplayTutorialIcon("Move");
                            UIManager.Instance.DisplaySupportKey(false);
                        }
                    }
                }
                if((Input.GetKey(KeyCode.DownArrow) ||
                    Input.GetKey(KeyCode.UpArrow) ||
                    Input.GetKey(KeyCode.LeftArrow) ||
                    Input.GetKey(KeyCode.RightArrow)) && !mMoveMission)
                {
                    _MoveTimer += Time.deltaTime;
                    if(_MoveTimer >= 3.0f)
                    {
                        GameManager.Instance.IsCinematicEvent = true;
                        mMoveMission = true;
                        m_DialogueQueue.Enqueue(m_DialogueMoveSuccess);
                        var dialogue = m_DialogueQueue.Dequeue();
                        UIManager.Instance.ChangeDialogueText(dialogue.Text);
                        StartCoroutine(BeginInteractTutorial());
                        PlayerController.Instance.mState = new IdleState();
                        PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
                        UIManager.Instance.DisplaySupportKey(false, false, false);
                        mEvent = false;
                    }
                }
                break;
            case TutorialType.Interact:
                if (Input.GetKeyDown(KeyCode.E) && !mEvent)
                {
                    if (m_DialogueQueue.Count > 0)
                    {
                        var dialogue = m_DialogueQueue.Dequeue();
                        UIManager.Instance.ChangeDialogueText(dialogue.Text);
                        if (dialogue.Trigger == Dialogue.TriggerType.Event && !mInteractMission)
                        {
                            mEvent = true;
                            GameManager.Instance.IsCinematicEvent = false;
                            UIManager.Instance.DisplaySupportKey(false, false, false);
                            StartCoroutine(SpawnTutorialEnvironment());
                        }
                    }
                }
                if((_BreakableObject && _BreakableObject._Completed) && !mInteractMission)
                {
                    mInteractMission = true;
                    m_DialogueQueue.Enqueue(m_DialogueInteractSuccess);
                    var dialogue = m_DialogueQueue.Dequeue();
                    UIManager.Instance.ChangeDialogueText(dialogue.Text);
                    StartCoroutine(BeginBattleTutorial());
                    _BreakableObject.mTutorialIcon.SetActive(false);
                    mEvent = false;
                }
                break;
            case TutorialType.Battle:
                if (PlayerController.Instance.IsDied)
                    Type = TutorialType.Death;

                if (Input.GetKeyDown(KeyCode.E) && !mEvent)
                {
                    if (m_DialogueQueue.Count > 0)
                    {
                        var dialogue = m_DialogueQueue.Dequeue();
                        UIManager.Instance.ChangeDialogueText(dialogue.Text);
                    }
                    else
                    {
                        if(!mBattleMission)
                        {
                            mEvent = true;
                            GameManager.Instance.IsCinematicEvent = false;
                            StartCoroutine(SpawnTutorialEnemy());
                            m_DialogueQueue.Enqueue(m_DialogueBattleHelp);
                            var dialogue = m_DialogueQueue.Dequeue();
                            UIManager.Instance.ChangeDialogueText(dialogue.Text);
                            UIManager.Instance.DisplaySupportKey(false, false, false);
                        }
                    }
                }
                if(_EnemyProwler == null && mBattleMission)
                {
                    m_DialogueQueue.Enqueue(m_DialogueBattleSuccess);
                    var dialogue = m_DialogueQueue.Dequeue();
                    UIManager.Instance.ChangeDialogueText(dialogue.Text);
                    StartCoroutine(BeginEndTutorial());
                    mEvent = false;
                }
                break;
            case TutorialType.End:
                if (Input.GetKeyDown(KeyCode.E) && !mEvent)
                {
                    if (m_DialogueQueue.Count > 0)
                    {
                        var dialogue = m_DialogueQueue.Dequeue();
                        UIManager.Instance.ChangeDialogueText(dialogue.Text);
                        if(dialogue.Trigger == Dialogue.TriggerType.Event)
                        {
                            mEvent = true;
                            GameObject i = Instantiate(mItem);
                            i.transform.SetParent(transform, false);
                            i.GetComponent<Item>().isSold = true;
                            i.transform.SetParent(PlayerController.Instance.mBag.transform);
                            PlayerController.Instance.mInventory.Add(i.GetComponent<Item>());
                            PlayerController.Instance.mHeroes[0].GetComponent<InventroySystem>().mAction += ItemMission;
                            UIManager.Instance.DisplayTutorialIcon("Item");
                            UIManager.Instance.DisplaySupportKey(false, false, false);
                        }
                    }
                    else
                    {
                        if (mEndMission)
                        {
                            PlayerController.Instance.ResetPlayerUnit();
                            LevelManager.Instance.ReturnToMainMenu();
                            Type = TutorialType.None;
                            mEvent = mInteractMission = mBattleMission = mEndMission = mMoveMission = mComplete = false;
                            _MoveTimer = 0.0f;
                            m_DialogueQueue.Clear();
                            UIManager.Instance.ChangeDialogueText("");
                            UIManager.Instance.DisplayDialogueBox(false);
                            UIManager.Instance.DisplaySupportKey(false, false, false);
                        }
                    }
                }

                if(mSecondaryEndMission)
                {
                    if (Input.GetKeyDown(KeyCode.I))
                    {
                        var dialogue = m_DialogueQueue.Dequeue();
                        UIManager.Instance.ChangeDialogueText(dialogue.Text);
                        UIManager.Instance.DisplayTutorialIcon("None");
                        mEvent = false;
                        UIManager.Instance.DisplaySupportKey(true, false, false);
                        UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty, string.Empty});
                    }
                }
                break;
            case TutorialType.Death:
                m_DialogueQueue.Clear();
                Dialogue gameoverText = new Dialogue("Wait, you died?! D: \n Ambulance! Ambulance, here!", Dialogue.TriggerType.None);
                UIManager.Instance.ChangeDialogueText(gameoverText.Text);
                Type = TutorialType.None;
                break;
            default:
                break;
        }
    }

    private void ItemMission()
    {
        mEndMission = true;
        PlayerController.Instance.mHeroes[0].GetComponent<InventroySystem>().mAction -= ItemMission;
        foreach (var t in m_DialogueEndSuccessList)
        {
            m_DialogueQueue.Enqueue(t);
        }
        UIManager.Instance.DisplayDialogueBox(true);
        var dialogue = m_DialogueQueue.Dequeue();
        UIManager.Instance.ChangeDialogueText(dialogue.Text);
        mSecondaryEndMission = true;
    }

    IEnumerator BeginTutorial()
    {
        Type = TutorialType.None;
        foreach (var t in m_DialogueList)
        {
            m_DialogueQueue.Enqueue(t);
        }
        UIManager.Instance.DisplayTutorialIcon("None");
        yield return new WaitForSeconds(3.0f);
        GameManager.Instance.IsCinematicEvent = true;
        Type = TutorialType.Intro;
        UIManager.Instance.DisplayDialogueBox(true);
        var dialogue = m_DialogueQueue.Dequeue();
        UIManager.Instance.ChangeDialogueText(dialogue.Text);
        UIManager.Instance.DisplayTutorialIcon("Interact");
    }
    IEnumerator BeginMoveTutorial()
    {
        Type = TutorialType.None;
        foreach (var t in m_DialogueMovementList)
        {
            m_DialogueQueue.Enqueue(t);
        }
        UIManager.Instance.DisplayTutorialIcon("None");
        yield return new WaitForSeconds(3.0f);
        if (fireworksTop)
        {
            fireworksTop.GetComponent<ParticleSystem>().Stop();
            Destroy(fireworksTop, 5.0f);
        }
        var dialogue = m_DialogueQueue.Dequeue();
        UIManager.Instance.ChangeDialogueText(dialogue.Text);
        Type = TutorialType.Movement;
        UIManager.Instance.DisplaySupportKey(true, false, false);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty, string.Empty});
    }

    IEnumerator BeginInteractTutorial()
    {
        Type = TutorialType.None;
        foreach (var t in m_DialogueInteractList)
        {
            m_DialogueQueue.Enqueue(t);
        }
        UIManager.Instance.DisplayTutorialIcon("None");
        yield return new WaitForSeconds(3.0f);
        var dialogue = m_DialogueQueue.Dequeue();
        UIManager.Instance.ChangeDialogueText(dialogue.Text);
        UIManager.Instance.DisplaySupportKey(true, false, false);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty, string.Empty});
        Type = TutorialType.Interact;
    }

    IEnumerator BeginBattleTutorial()
    {
        Type = TutorialType.None;
        foreach (var t in m_DialogueBattleList)
        {
            m_DialogueQueue.Enqueue(t);
        }
        UIManager.Instance.DisplayTutorialIcon("None");
        yield return new WaitForSeconds(3.0f);
        var dialogue = m_DialogueQueue.Dequeue();
        UIManager.Instance.ChangeDialogueText(dialogue.Text);
        Type = TutorialType.Battle;
        UIManager.Instance.DisplaySupportKey(true, false, false);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty, string.Empty});
    }

    IEnumerator BeginEndTutorial()
    {
        Type = TutorialType.None;
        foreach (var t in m_DialogueEndList)
        {
            m_DialogueQueue.Enqueue(t);
        }
        UIManager.Instance.DisplayTutorialIcon("None");
        yield return new WaitForSeconds(3.0f);
        var dialogue = m_DialogueQueue.Dequeue();
        UIManager.Instance.ChangeDialogueText(dialogue.Text);
        mEvent = false;
        Type = TutorialType.End;
        UIManager.Instance.DisplaySupportKey(true, false, false);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty, string.Empty});
    }

    IEnumerator SpawnTutorialEnvironment()
    {
        _EnvironmentSpawner.Spawn();
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/SpawnTutorial")
, _EnvironmentSpawner.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
        Destroy(go, 2.0f);
        _BreakableObject = _EnvironmentSpawner.mObject.GetComponent<BreakableObject>();
        UIManager.Instance.DisplayTutorialIcon("Interact");
        _BreakableObject.mTutorialIcon.SetActive(true);
        yield break;
    }

    IEnumerator SpawnTutorialEnemy()
    {
        _EnemySpawner.Spawn();
        yield return new WaitForSeconds(1.1f);
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Ghoul Summon_Effect")
, _EnemySpawner.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
        Destroy(go, 2.0f);
        _EnemyProwler = _EnemySpawner.mObject.GetComponent<EnemyProwler>();
        _EnemyProwler.mCanvas.transform.Find("Tutorial").gameObject.SetActive(true);
        _EnemyProwler.mModel.GetComponent<Animator>().Play("Spawn");
        mBattleMission = true;
    }

    GameObject fireworksTop = null;

    IEnumerator Event()
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.DisplaySupportKey(true, true, false);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Yes",
            "No",
            string.Empty});
        PlayerController.Instance.GetComponent<InteractSystem>().mRightAction += (() => {
            m_DialogueQueue.Enqueue(m_DialogueNoCase);
            mComplete = true;
        });
        PlayerController.Instance.GetComponent<InteractSystem>().mLeftAction += (() => {
            m_DialogueQueue.Enqueue(m_DialogueYesCase);
            fireworksTop = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/CelebrationEffect2"), UIManager.Instance.mAdditionalCanvas.transform.localPosition + new Vector3(0.0f, 25.0f, 0.0f), Quaternion.identity, UIManager.Instance.mAdditionalCanvas.transform);
            mComplete = true;
        });
        Transform t = UIManager.Instance.mStorage.mTutorialHereIcon.transform.parent;
        UIManager.Instance.mStorage.mTutorialHereIcon.transform.SetParent(
            UIManager.Instance.mStorage.mDialogueBox.transform);
        yield return new WaitUntil(() => mComplete);
        var dialogue = m_DialogueQueue.Dequeue();
        UIManager.Instance.ChangeDialogueText(dialogue.Text);
        mEvent = false;
        mComplete = false;
        StartCoroutine(BeginMoveTutorial());
        UIManager.Instance.mStorage.mTutorialHereIcon.transform.SetParent(t);
        PlayerController.Instance.GetComponent<InteractSystem>().ResetActions();
        UIManager.Instance.DisplaySupportKey(false, false, false);
    }
}

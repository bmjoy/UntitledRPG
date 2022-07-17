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
                            UIManager.Instance.DisplayEKeyInDialogue(false);
                            UIManager.Instance.DisplayTutorialIcon("Move");
                        }
                    }
                }
                if((Input.GetKey(KeyCode.W) ||
                    Input.GetKey(KeyCode.A) ||
                    Input.GetKey(KeyCode.D) ||
                    Input.GetKey(KeyCode.S)) && !mMoveMission)
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
                        UIManager.Instance.DisplayEKeyInDialogue(false);
                        PlayerController.Instance.mState = new IdleState();
                        PlayerController.Instance.mModel.GetComponent<Animator>().SetFloat("Speed", 0.0f);
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
                            UIManager.Instance.DisplayEKeyInDialogue(false);

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
                    UIManager.Instance.DisplayEKeyInDialogue(false);
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
                            UIManager.Instance.DisplayEKeyInDialogue(false);
                        }
                    }
                }
                if(_EnemyProwler == null && mBattleMission)
                {
                    m_DialogueQueue.Enqueue(m_DialogueBattleSuccess);
                    var dialogue = m_DialogueQueue.Dequeue();
                    UIManager.Instance.ChangeDialogueText(dialogue.Text);
                    UIManager.Instance.DisplayEKeyInDialogue(false);
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
                            UIManager.Instance.DisplayEKeyInDialogue(false);
                            i.transform.SetParent(PlayerController.Instance.mBag.transform);
                            PlayerController.Instance.mInventory.Add(i.GetComponent<Item>());
                            PlayerController.Instance.mHeroes[0].GetComponent<InventroySystem>().mAction += ItemMission;
                            UIManager.Instance.DisplayTutorialIcon("Item");
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
                        }
                    }
                }

                if(mSecondaryEndMission)
                {
                    if (Input.GetKeyDown(KeyCode.I))
                    {
                        var dialogue = m_DialogueQueue.Dequeue();
                        UIManager.Instance.ChangeDialogueText(dialogue.Text);
                        UIManager.Instance.DisplayEKeyInDialogue(true);
                        UIManager.Instance.DisplayTutorialIcon("None");
                        mEvent = false;
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
        UIManager.Instance.DisplayEKeyInDialogue(false);
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
        UIManager.Instance.DisplayEKeyInDialogue(true);
        Type = TutorialType.Movement;
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
        UIManager.Instance.DisplayEKeyInDialogue(true);
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
        UIManager.Instance.DisplayEKeyInDialogue(true);
        Type = TutorialType.Battle;
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
        UIManager.Instance.DisplayEKeyInDialogue(true);
        mEvent = false;
        Type = TutorialType.End;
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
        UIManager.Instance.ChangeTwoButtons(UIManager.Instance.mStorage.YesButtonImage,
    UIManager.Instance.mStorage.NoButtonImage);
        UIManager.Instance.AddListenerRightButton(() => {
            m_DialogueQueue.Enqueue(m_DialogueNoCase);
            mComplete = true;
        });
        UIManager.Instance.AddListenerLeftButton(() => {
            m_DialogueQueue.Enqueue(m_DialogueYesCase);
            fireworksTop = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/CelebrationEffect2"), UIManager.Instance.mAdditionalCanvas.transform.localPosition + new Vector3(0.0f, 25.0f, 0.0f), Quaternion.identity, UIManager.Instance.mAdditionalCanvas.transform);
            mComplete = true;
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        UIManager.Instance.DisplayEKeyInDialogue(false);
        UIManager.Instance.DisplayTutorialIcon("Mouse", true);
        Transform t = UIManager.Instance.mStorage.mTutorialHereIcon.transform.parent;
        UIManager.Instance.mStorage.mTutorialHereIcon.transform.SetParent(
            UIManager.Instance.mStorage.mDialogueBox.transform);
        UIManager.Instance.mStorage.mTutorialHereIcon.transform.localPosition =
            UIManager.Instance.mStorage.mLeftButton.transform.localPosition - new Vector3(0,20,0);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
        var dialogue = m_DialogueQueue.Dequeue();
        UIManager.Instance.ChangeDialogueText(dialogue.Text);
        mEvent = false;
        mComplete = false;
        StartCoroutine(BeginMoveTutorial());
        UIManager.Instance.mStorage.mTutorialHereIcon.transform.SetParent(t);
    }
}

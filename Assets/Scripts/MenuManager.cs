﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public bool isAct;
    public static MenuManager instance;
    [SerializeField] GameObject Menu_vcam;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] UI2DSprite m_NoSaveSprite;
    Vector3 originPos;
    // [SerializeField] Text m_Text;

    [Header("場景")]
    [SerializeField] string continueScene;

    [Header("轉場黑幕")]
    [Space(10), SerializeField]
    public GameObject loadingCanvas;
    public Loading loading;
    void Awake()
    {
        if (instance != null)
        { Destroy(this.gameObject); }
        else
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        loadingCanvas.SetActive(false);
        loading = loadingCanvas.GetComponent<Loading>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += LoadNewScene;
        SceneManager.sceneLoaded += LoadContinueScene;
        originPos = mainPanel.transform.localPosition;
        m_NoSaveSprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAct)
        {
            Menu_vcam.SetActive(true);
            mainPanel.SetActive(true);
        }
        else
        {
            Menu_vcam.SetActive(false);
            mainPanel.SetActive(false);
            optionsPanel.SetActive(false);
        }

        //作弊鍵
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            GameManager._instance.GameTimer.TimeReset();
            GameManager._instance.nowRingCount = 0;
            StartCoroutine(LoadScene(0));
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            GameManager._instance.GameTimer.TimeReset();
            GameManager._instance.nowRingCount = 0;
            StartCoroutine(LoadScene(1));
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            GameManager._instance.GameTimer.TimeReset();
            GameManager._instance.nowRingCount = 0;
            StartCoroutine(LoadScene(2));
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            GameManager._instance.player.transform.position = GameObject.FindGameObjectWithTag("Finish").transform.position;
        }

    }

    public void MenuActive()
    {
        MenuManager.instance.isAct = false;
    }

    public void NewGameButton()
    {
        GameManager._instance.GameTimer.TimeReset();
        GameManager._instance.nowRingCount = 0;
        continueScene = "";
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(LoadScene(0));
        }
        else
        {
            StartCoroutine(LoadScene(0));
        }
    }

    public void ContinueButton()//讀取資料
    {
        PlayerData data = SaveSystem.Load();
        continueScene = data.sceneName;
        GameManager._instance.SavePoint = new GameObject().transform;
        GameManager._instance.SavePoint.name = "TemporarySavePoint";
        GameManager._instance.SavePoint.position = new Vector3(data.x, data.y, data.z);
        GameManager._instance.SavePoint.SetParent(this.transform);
        GameManager._instance.nowRingCount = data.score;
        GameManager._instance.GameTimer.SetTime(data.time);
        GameManager._instance.ui.ShowTimeAndPoint();

        if (continueScene != "")
        { StartCoroutine(LoadScene(continueScene)); }
        else
        {
            m_NoSaveSprite.enabled = true;
            m_NoSaveSprite.gameObject.GetComponent<TweenAlpha>().enabled = true;
        }
    }

    public void ResetText()
    {
        mainPanel.transform.localPosition = originPos;
        mainPanel.transform.localPosition = originPos;
    }

    public void ChangeScene(string ns)
    {
        StartCoroutine(LoadScene(ns));
    }
    public void ChangeNextScene()
    {
        Debug.Log("Next!");
        GameManager._instance.GameTimer.TimeReset();
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void ExitButton()
    { Application.Quit(); }

    public void SetSaveScene()//暫存現在場景名稱
    {
        continueScene = SceneManager.GetActiveScene().name;
    }
    public string GetSaveScene()//存檔現在場景名稱
    {
        if (continueScene != null)
        { return continueScene; }
        else
        { return null; }
    }

    public bool CheckedIfContinueScene()//設置場景前的檢查
    {
        if (continueScene == SceneManager.GetActiveScene().name)
        { return true; }
        else
        { return false; }
    }
    IEnumerator LoadScene(string ns)
    {
        yield return null;
        loadingCanvas.SetActive(true);
        loading.animator.SetTrigger("Start");
        if (GameManager._instance.gamestate == GameState.Pause)
        { GameManager._instance.TransformGameState(); }

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(ns);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        yield return new WaitForSeconds(loading.transitionTime);
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            // m_Lable.text = " " + (asyncOperation.progress * 100) + "%";

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f && loading.IfPlayFinish())
            {
                // m_Lable.text = "Done!";
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator LoadScene(int ns)
    {
        yield return null;

        loadingCanvas.SetActive(true);
        loading.animator.SetTrigger("Start");
        if (GameManager._instance.gamestate == GameState.Pause)
        { GameManager._instance.TransformGameState(); }

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(ns);
        asyncOperation.allowSceneActivation = false;
        yield return new WaitForSeconds(loading.transitionTime);
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.90f && loading.IfPlayFinish())
            {
                // m_Lable.text = "Done!";
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    //加載新遊戲場景
    void LoadNewScene(Scene scene, LoadSceneMode mode)
    {
        if (scene == SceneManager.GetSceneByBuildIndex(0) && (continueScene == ""))
        { StartCoroutine("EnterNewScene"); }
        else if (continueScene != "")
        { StartCoroutine("EnterContinueScene"); }
        GameManager._instance.player = FindObjectOfType<SimpleCharacterControl>();
    }

    IEnumerator EnterNewScene()
    {
        MenuManager.instance.gameObject.GetComponent<TweenAlpha>().enabled = true;
        // MenuManager.instance.m_Lable.GetComponent<TweenAlpha>().enabled = true;
        loading.animator.SetTrigger("End");
        while (loading.animator.GetCurrentAnimatorStateInfo(0).IsName("UI_cross_FadeOut"))
        {
            yield return null;
        }

        yield return null;
    }
    //加載之前遊戲場景
    void LoadContinueScene(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine("EnterContinueScene");
    }
    IEnumerator EnterContinueScene()
    {
        loading.animator.SetTrigger("End");
        if (GameManager._instance.SavePoint)
        {
            FindObjectOfType<SimpleCharacterControl>().ChangePlayerPosion(GameManager._instance.SavePoint.position);
            Destroy(GameManager._instance.SavePoint.gameObject);
        }
        else
        { Debug.Log("沒有成功加載!"); }
        GameManager._instance.GameTimer.TimerSwitch(true);
        yield return null;
    }
}

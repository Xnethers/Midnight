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
    [SerializeField] UILabel m_Lable;
    // [SerializeField] Text m_Text;

    [Header("場景")]
    [SerializeField] string continueScene;

    [Header("轉場黑幕")]
    [Space(10), SerializeField, Range(1, 20)]
    private int speed;
    int i = 0;
    void Awake()
    {
        if (instance != null)
        { Destroy(this.gameObject); }
        else
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += LoadNewScene;
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
    }

    public void MenuActive()
    {
        MenuManager.instance.isAct = false;
    }

    public void NewGameButton()
    {
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
        GameManager._instance.SavePoint.position = new Vector3(data.x, data.y, data.z);
        GameManager._instance.nowRingCount = data.score;
        GameManager._instance.GameTimer.minute = data.minute;
        GameManager._instance.GameTimer.second = data.second;
        if (continueScene != "")
        { StartCoroutine(LoadScene(continueScene)); }
        else
        {
            m_Lable.text = "No save file!";
            m_Lable.gameObject.GetComponent<TweenAlpha>().enabled = true;
        }
    }

    public void ResetText()
    {
        m_Lable.text = "";
    }

    public void ChangeScene(string ns)
    {
        StartCoroutine(LoadScene(ns));
    }
    public void ChangeNextScene()
    {
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
    IEnumerator LoadScene(string ns)
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(ns);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Pro :" + asyncOperation.progress);
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            m_Lable.text = " " + (asyncOperation.progress * 100) + "%";

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                m_Lable.text = "Done!";
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator LoadScene(int ns)
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(ns);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            m_Lable.text = " " + (asyncOperation.progress * 100) + "%";
            if (asyncOperation.progress >= 0.90f)
            {
                m_Lable.text = "Done!";
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void LoadNewScene(Scene scene, LoadSceneMode mode)
    {
        if (scene == SceneManager.GetSceneByBuildIndex(0))
        { StartCoroutine("EnterNewScene"); }

    }

    IEnumerator EnterNewScene()
    {
        MenuManager.instance.gameObject.GetComponent<TweenAlpha>().enabled = true;
        MenuManager.instance.m_Lable.GetComponent<TweenAlpha>().enabled = true;
        GameManager._instance.TransformGameState();
        yield return null;
    }
}

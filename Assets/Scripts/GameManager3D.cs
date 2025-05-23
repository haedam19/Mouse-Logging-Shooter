using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MouseLog;
using UnityEngine;

public class GameManager3D : MonoBehaviour
{
    private const double MinDblClickDist = 4.0; // minimum distance two clicks must be apart (filters double-clicks)

    #region Singleton Instance
    static GameManager3D instance;
    public static GameManager3D Instance
    {
        get
        {
            if (instance == null)
                return null;
            else
                return instance;
        }
    }
    #endregion

    public enum GameState { 
        InputDisabled, // 입력을 무시해야 하는 상황
        ConditionReady, // 컨디션 진입 준비
        InCondition // 컨디션 내에서 측정 시행 중
    }

    private string gameLogfilePath; // 게임 실행 관련 로그 기록 경로

    [Header("SubManagers")]
    public EnvManager3D envManager;
    public TargetManager3D targetManager;
    public MousePosTracker3D mousePosTracker;

    [Header("GameStatus")]
    public bool playing;
    public bool inExporting;
    public float startTime;
    public ushort trialIndex;

    [Header("Statistics")]
    public int totalTrialCount;
    public int errorCount;

    #region MouseLogData
    private SessionData _sdata; // the whole session (one test); holds conditions in order
    private ConditionData _cdata; // the current condition; retrieved from the session
    private TrialData _tdata; // the current trial; retrieved from the condition

    public SessionData Session { get { return _sdata; } }
    public ConditionData Condition { get { return _cdata; } }
    public TrialData TrialData { get { return _tdata; } }
    #endregion

    #region file
    private string _fileNoExt; // full path and filename without extension
    private XmlTextWriter _writer; // XML writer -- uses _fileNoExt.xml
    #endregion

    public void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
#if UNITY_EDITOR
            gameLogfilePath = Application.dataPath;

#elif UNITY_STANDALONE_WIN
            gameLogfilePath = Application.persistentDataPath;
#endif
            LoadData();

            instance = this;
            playing = false;
            inExporting = false;
            envManager.Init();
            targetManager.Init();
            mousePosTracker.Init();

            trialIndex = 0;
            totalTrialCount = 0;
            errorCount = 0;
        }
    }

    public void StartSession(SessionData session)
    {

    }

    /// <summary> Condition 단위의 측정 시작 메소드 </summary>
    public void TestStart()
    {
        _fileNoExt = string.Format("{0}\\{1}__{2}", Application.persistentDataPath, _sdata.FilenameBase, Environment.TickCount);

        playing = true;
        Cursor.lockState = CursorLockMode.None;
        startTime = Time.time;
        Destroy(GameObject.Find("StartMessage"));
    }

    public void MouseClick()
    {

    }

    public void MouseClick(Vector2 pos, bool hitFlag, RaycastHit hitInfo)
    {
        if (_cdata != null)
        {
            TimePointR clickTimePos = new TimePointR((double)pos.x, (double)pos.y, Timer.Time);
            if (_tdata.IsStartAreaTrial || PointR.Distance((PointR)_tdata.Start, (PointR)clickTimePos) > MinDblClickDist)
            {
                if (_tdata.IsStartAreaTrial) // click was to begin the first actual trial in the current condition
                {
                    if (!_tdata.TargetContains((PointR)clickTimePos)) // click missed start target
                    {
                        DoError();
                    }
                    else // start first actual trial
                    {
                        //_tdata = _cdata[1]; // trial number 1
                        _tdata.Start = clickTimePos;
                    }
                    // Invalidate(); // paint whole form because of START label 
                }
            }
        }
    }

    public void MouseMove(MouseMove move)
    {
        
    }

    private void DoError() { } // TODO: IMPLEMENT

    private void LoadData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(Application.dataPath + "/Json/config.json");
        string fileName = "ConfigLoadLog" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

        if (textAsset != null)
        {
            string json = textAsset.text;
            SessionConfiguration config = JsonUtility.FromJson<SessionConfiguration>(json);
            if (config.isValid())
            {
                _sdata = new SessionData(config.subject, config.isCircular, new ScreenData(Screen.width, Screen.height), config.a, config.w, null, -1.0, -1.0, config.trials, config.practice);
                _cdata = _sdata[0]; // first overall condition
                _tdata = _cdata[0]; // first trial is special start-area trial at index 0
                totalTrialCount = config.trials;
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(gameLogfilePath, fileName), true))
                {
                    sw.WriteLine("Invalid config.json");
                }
                Application.Quit();
            }
        }
        else
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(gameLogfilePath, fileName), true))
            {
                sw.WriteLine("Failed to load config.json");
            }
            Application.Quit();
        }
    }
}

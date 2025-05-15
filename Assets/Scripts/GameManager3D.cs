using System.Collections;
using System.Collections.Generic;
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

    public void Awake()
    {
        if (instance == null)
        {
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
        else
            Destroy(gameObject);
    }

    /// <summary> Condition 단위의 측정 시작 메소드 </summary>
    public void TestStart()
    {
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

    public void MouseMove()
    {
        
    }

    private void DoError() { } // TODO: IMPLEMENT
}

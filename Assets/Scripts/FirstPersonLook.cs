using UnityEngine;
using UnityEngine.InputSystem;

public class FirsetPersonLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 100f;

    private Mouse Mouse { get { return Mouse.current; } }

    ControlDisplayGain gain;

    private float pitch = 0f;
    private float yaw = 0f;

    // Fields for Update Method. 마우스 입력 데이터 처리에 사용.
    private Vector2 _delta; // InputSystem을 통해 받은 delta 값
    private Vector2 _gDelta; // Gain Function 처리 후 delta 값 
    private Vector2 _lastPos; // 이전 프레임 마우스 커서 위치
    private Vector2 _currentPos; // 잠정적인 마우스 커서 위치
    private bool _isClicked;

    private void Awake()
    {
        gain = new ControlDisplayGain(ControlDisplayGain.Type.linear, 1f);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _delta = Vector2.zero;
        _lastPos = new Vector2(Screen.width / 2, Screen.height / 2);
        _currentPos = new Vector2(Screen.width / 2, Screen.height / 2);
        _isClicked = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Get mouse input data
        _delta = Mouse.delta.ReadValue(); // WM_INPUT 메시지 이용, count 단위로 측정
        _gDelta = gain.GainedDelta(_delta); // CD Gain 적용
        _lastPos = _currentPos;
        _currentPos += _gDelta;
        _isClicked = Mouse.press.wasPressedThisFrame;

        // 카메라 이동 처리
        yaw += 0.0476f * _currentPos.x;
        pitch -= 30 * _gDelta.y;
        yaw = Mathf.Clamp(yaw, -60f, 60f);
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // 마우스 움직임 이벤트 발생
        if (_delta.sqrMagnitude > 0)
        {
            GameManager3D.Instance.MouseMove();
            // int currentCond = GameManager3D.Instance.Session.condIdx;
            // GameManager3D.Instance.Session._conditions[currentCond].AddMove(_currentPos);
        }

        // 클릭 이벤트 발생
        if (_isClicked)
        {
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(transform.position
                , transform.forward
                , out hitInfo
                , 1e3f, LayerMask.NameToLayer("Target"));
            GameManager3D.Instance.MouseClick(_currentPos, hit, hitInfo);
        }

        

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
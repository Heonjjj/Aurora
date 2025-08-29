using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class DirectionManager : MonoBehaviour
{
    private static DirectionManager _instance;
    public static DirectionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DirectionManager>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("DirectionManager");
                    _instance = go.AddComponent<DirectionManager>();
                }
            }
            return _instance;
        }
    }
    void Awake()
    {
        Debug.Log("DirectionManager Awake in scene: " + gameObject.scene.name);
        if (_instance != null && _instance != this)
        {
            Debug.Log("Duplicate DirectionManager found, destroying this one: " + gameObject.name);
            transform.SetParent(null); // �θ�(Canvas)���� �и�
            Destroy(gameObject); // �̹� �ٸ� ������ �ı�
            return;
        }
        _instance = this; // ���⼭ ���
        DontDestroyOnLoad(gameObject);
    }


    Animator _animator;
    PlayerController _controller;
    [Header("IntroCamera")]
    [SerializeField] private CinemachineVirtualCamera _introCam;
    [SerializeField] private CinemachineBlendListCamera _startCam;
    [SerializeField] public CinemachineFreeLook _mainCam;


    [Header("InGameCamera")]
    private int eq;

    void Start()
    {
        _animator = SafeFetchHelper.GetOrError<Animator>(Player.Instance.gameObject);
        _controller = SafeFetchHelper.GetOrError<PlayerController>(Player.Instance.gameObject);
        _controller.LockOnInput(1);
    }
    public void Direction_Intro()
    {
        Destroy(_introCam);
        StartCoroutine(IntroSequence());
    }
    public IEnumerator IntroSequence()
    {
        if (_startCam != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            _startCam.Priority = 10;
            // ���� �ð� ���
            yield return new WaitForSecondsRealtime(4f);
            // ���� ������ �Է� Ȱ��ȭ
            _startCam.Priority = 0;
            _mainCam.Priority = 10;
            _controller.LockOnInput(0);
        }
    }

    //        StartCoroutine(DirectionManager.Instance.Sequence());
    // ȣ�Ⱑ��
    public async void Direction()
    {
        StartCoroutine(Sequence());
        await Task.Delay(100);
    }
    public IEnumerator Sequence()
    {
        if (_mainCam != null)
        {
            _controller.LockOnInput(1);
            _mainCam.Priority = 0;
            // ���� �ð� ���
            yield return new WaitForSecondsRealtime(4f);
            // ���� ������ �Է� Ȱ��ȭ
            _controller.LockOnInput(0);
            _mainCam.Priority = 10;
        }
    }


    public void OnDirection(bool start)
    {
        if (start)
        {
            _mainCam.Priority = 0;
        }
        else
        {
            _mainCam.Priority = 10;
        }
    }




    public void LockOnCam(bool canmove)
    {
        if (canmove)
        {
            // �κ��丮 ������ �� �� ī�޶� ����
            _mainCam.m_XAxis.m_InputAxisName = "";
            _mainCam.m_YAxis.m_InputAxisName = "";
        }
        else
        {
            // �κ��丮 ������ �� �� �ٽ� ������
            _mainCam.m_XAxis.m_InputAxisName = "Mouse X";
            _mainCam.m_YAxis.m_InputAxisName = "Mouse Y";
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;

public interface IDamageble
{
    void TakePhysicalDamage(int damage);
}

public enum PlayerState
{
    Idle,
    Run,
    Jump,
    Attack,
    Damaged,
    Dead
}

public class UI_Manager : MonoBehaviour
{
    private static UI_Manager _instance;
    public static UI_Manager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UI_Manager>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("UI_Manager");
                    _instance = go.AddComponent<UI_Manager>();
                }
            }
            return _instance;
        }
    }

    public PlayerM _playerM { get; private set; }
    public PlayerVM _playerVM { get; private set; }

    public InventoryM _invenM { get; private set; }
    public InventoryVM _invenVM { get; private set; }

    public PlayerV _playerV;
    public InventoryV _invenV;
    private EventMediator _mediator;

    [Header("Panel")]
    public UI_SettingPanel _settingPanel;
    public GameObject _overPanel;
    public GameObject _savePanel;
    public GameObject _startPanel;
    public GameObject _guiPanel;

    [Header("GUI")]
    public GameObject _promptText;
    public UI_ActionKey _uiaction;
    public GameObject _crosshair;
    public GameObject _conditions;
    public GameObject _quickslot;
    public GameObject _pauseButton;
    public GameObject _equipment;
    public GameObject _alertText;
    public GameObject _log;


    void Awake()
    {
        Debug.Log("UI_Manager Awake in scene: " + gameObject.scene.name);
        if (_instance != null && _instance != this)
        {
            Debug.Log("Duplicate UIManager found, destroying this one: " + gameObject.name);
            transform.SetParent(null);
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _playerM = new PlayerM();
        _playerVM = new PlayerVM(_playerM);

        _invenM = new InventoryM(24);
        _invenVM = new InventoryVM(_invenM);

        _mediator = new EventMediator(_invenM);
    }

    private void Start()
    {
        _guiPanel.SetActive(false);
        _crosshair.SetActive(false);
        _promptText.SetActive(false);
        _uiaction.gameObject.SetActive(false);
        _quickslot.SetActive(false);
        _equipment.SetActive(true);
        _conditions.SetActive(true);
        _pauseButton.SetActive(true);
        _alertText.SetActive(false);
        _log.SetActive(false);

        _settingPanel.gameObject.SetActive(false);
        _invenV.gameObject.SetActive(false);
        _overPanel.SetActive(false);
        _savePanel.SetActive(false);
        _startPanel.SetActive(true);

        _settingPanel.InitPanel();

        _invenV.gameObject.SetActive(true);
        _invenV.Init(_invenVM, _playerM);
        _invenV.gameObject.SetActive(false);

        _playerV.Init(_playerVM);
    }
}
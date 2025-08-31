using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

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
public sealed class PlayerContext : IDisposable
{
    public IPlayer Player { get; }
    public IInventory Inventory { get; }
    public PlayerVM PlayerVM { get; }
    public InventoryVM InventoryVM { get; }
    public EventMediator Mediator { get; }

    public PlayerContext(int inventorySize)
    {
        // 실제 구현체 생성
        var playerM = new PlayerM();
        var inventoryM = new InventoryM(inventorySize);

        // 인터페이스로 노출 (외부에서는 IPlayer/IInventory만 보이도록)
        Player = playerM;
        Inventory = inventoryM;

        // VM 초기화
        PlayerVM = new PlayerVM(playerM);
        InventoryVM = new InventoryVM(inventoryM);

        // Mediator 연결 (인터페이스 기반)
        Mediator = new EventMediator(Inventory, Player, new InvenActionRule());
    }

    public void Dispose()
    {
        Mediator.Dispose();
        PlayerVM.Dispose();
        InventoryVM.Dispose();
    }
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

    [Header("Views")]
    public PlayerV _playerV;
    public InventoryV _inventoryV;
    private PlayerContext _context;

    [Header("Panel")]
    public UI_SettingPanel _settingPanel;
    public UI_Intro _introPanel;
    public GameObject _overPanel;
    public GameObject _savePanel;
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

        // PlayerContext 생성 (모든 M + VM + Mediator 한 번에 준비됨)
        _context = new PlayerContext(24);
        // View 초기화 (VM과 Mediator 주입)
        _playerV.Init(_context.PlayerVM);
        _inventoryV.Init(_context.InventoryVM, _context.Mediator, _context.Player);

        // 패널 초기 상태 설정
        InitPanels();
    }

    private void Start()
    {
        // Start에서는 단순 UI 활성화/비활성화만
        _inventoryV.gameObject.SetActive(true);
        _inventoryV.gameObject.SetActive(false); // 처음에는 숨김
    }

    private void OnDestroy()
    {
        _context?.Dispose();
    }

    private void InitPanels()
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
        _inventoryV.gameObject.SetActive(false);
        _overPanel.SetActive(false);
        _savePanel.SetActive(false);
        _introPanel.gameObject.SetActive(true);

        _settingPanel.InitPanel();
    }
}
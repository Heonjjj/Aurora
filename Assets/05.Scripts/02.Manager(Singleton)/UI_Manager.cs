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
        // ���� ����ü ����
        var playerM = new PlayerM();
        var inventoryM = new InventoryM(inventorySize);

        // �������̽��� ���� (�ܺο����� IPlayer/IInventory�� ���̵���)
        Player = playerM;
        Inventory = inventoryM;

        // VM �ʱ�ȭ
        PlayerVM = new PlayerVM(playerM);
        InventoryVM = new InventoryVM(inventoryM);

        // Mediator ���� (�������̽� ���)
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

        // PlayerContext ���� (��� M + VM + Mediator �� ���� �غ��)
        _context = new PlayerContext(24);
        // View �ʱ�ȭ (VM�� Mediator ����)
        _playerV.Init(_context.PlayerVM);
        _inventoryV.Init(_context.InventoryVM, _context.Mediator, _context.Player);

        // �г� �ʱ� ���� ����
        InitPanels();
    }

    private void Start()
    {
        // Start������ �ܼ� UI Ȱ��ȭ/��Ȱ��ȭ��
        _inventoryV.gameObject.SetActive(true);
        _inventoryV.gameObject.SetActive(false); // ó������ ����
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
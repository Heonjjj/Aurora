using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{ 
    [Header("Movement")]
    public float speed = 2f;
    public float runSpeed = 4f;
    public float smooth = 20f;
    public float gravity = -9.81f;

    [Header("Object")]
    public bool toggleCameraRotation;
    [SerializeField]private List<GameObject> flashLights;

    CharacterController _controller;
    PlayerInput _input;
    Animator _animator;
    Camera _camera;
    UI_ActionKey _uiAction;
    UI_SettingPanel _settingPanel;

    Vector2 dir;
    Vector3 velocity;
    bool isGrounded;
    bool isRun;
    bool isIdle;
    float nextIdleTime = 10f;

    private void Awake()
    {
        _controller = SafeFetchHelper.GetOrError<CharacterController>(gameObject);
        _animator = SafeFetchHelper.GetOrError<Animator>(gameObject);
        _camera = SafeFetchHelper.GetOrCreate<Camera>("Main Camera");
        _input = new PlayerInput();
    }
    private void OnEnable()
    {
        _input.Player.Move.performed += OnMove;
        _input.Player.Move.canceled += OnMove;
        _input.Player.Run.started += ctx => isRun = true;
        _input.Player.Run.canceled += ctx => isRun = false;
        _input.Player.Jump.started += OnJump;
        _input.Player.Flash.started += OnFlash;
        _input.Player.CameraToggle.started += OnToggleCamera;
        _input.Player.Action.performed += OnAction;
        _input.Player.Action.canceled += OnAction;
        _input.Player.Interaction.started += OnInteraction;
        _input.Player.Menu.started += OnMenu;
        _input.Player.Inventory.started += OnInventory;
        _input.Player.PotalGun.started += OnPotalGun;
        _input.Player.MouseL.started += OnMouseL;
        _input.Player.MouseR.started += OnMouseR;
        _input.Player.Enable();
    }
    private void Start()
    {
        _uiAction = SafeFetchHelper.GetChild<UI_ActionKey>(UI_Manager.Instance.gameObject);
        _settingPanel = SafeFetchHelper.GetChild<UI_SettingPanel>(UI_Manager.Instance.gameObject);
    }

    private void Update()
    {
        isGrounded = _controller.isGrounded; 
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        else
            velocity.y += gravity * Time.deltaTime;

        _controller.Move( (new Vector3(0f, velocity.y, 0f)) * Time.deltaTime);
        Move();

        if (isIdle && Time.time > nextIdleTime) //10초마다 랜덤 idle 재생
        {
            int random = Random.Range(0, 4);  // 0~3
            _animator.SetInteger("animBaseInt", random);
            _animator.SetTrigger("animTrigger");
            nextIdleTime = Time.time + 10f; // 다시 10초 뒤로 설정
        }
    }
    private void LateUpdate()
    {
        if(toggleCameraRotation)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp
                (transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smooth);
        }
    }
    private void OnDisable() => _input.Player.Disable();

    void Move()
    {
        float finalSpeed = isRun ? runSpeed : speed;

        Vector3 camForward = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(_camera.transform.right, new Vector3(1, 0, 1)).normalized;

        Vector3 moveDir = camForward * dir.y + camRight * dir.x;
        moveDir.Normalize();

        Vector3 move = moveDir * finalSpeed + new Vector3(0, velocity.y, 0);
        _controller.Move(move * Time.deltaTime);

        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smooth);
        }

        float percent = (isRun ? 1 : 0.5f) * new Vector2(dir.x, dir.y).magnitude;
        _animator.SetFloat("Blend", percent, 0.3f, Time.deltaTime);
        
        isIdle = (percent < 0.01f); // Idle 판정
        _animator.SetBool("IsMove", !isIdle);
    }


    public void LockOnInput(int value)
    {
        if(value==1)
        {
            _input.Player.Disable();
            _input.UI.Enable();
        }
        else
        {
            _input.UI.Disable();
            _input.Player.Enable();
        }
    }


    void OnMove(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector2>();
    }
    void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            _animator.SetTrigger("IsJump");
            velocity.y = Mathf.Sqrt(-gravity);
        }
    }
    void OnToggleCamera(InputAction.CallbackContext context) //Alt
    {
        if (context.started) 
            toggleCameraRotation = !toggleCameraRotation; 
    }

    void OnFlash(InputAction.CallbackContext context) // Q, 이렇게밖에안됨 건들지마, 이렇게안하면 뛰면서 총꺼냄
    {
        int state = _animator.GetInteger("IsFlash");

        if (state == 0)
        {
            _animator.SetInteger("IsFlash", 1);
        }
        else if (state == 2)
        {
            _animator.SetInteger("IsFlash", 3);
        }
    }
    void EquipFlash(int state)
    {
        if (state == 1) 
        {
            _animator.SetInteger("IsFlash", 2);
            _animator.SetLayerWeight(1, 1f);
            foreach (var light in flashLights)
                light.SetActive(true);
        }
        else if (state == 0) 
        {
            _animator.SetInteger("IsFlash", 0);
            _animator.SetLayerWeight(1, 0f);
            foreach (var light in flashLights)
                light.SetActive(false);
        }
    }
    void OnAction(InputAction.CallbackContext context) // Z
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _uiAction.gameObject.SetActive(true); 
            Time.timeScale = 0.2f;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _uiAction.gameObject.SetActive(false); 
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void OnInteraction(InputAction.CallbackContext context) // E
    {
        _animator.SetTrigger("IsInteraction");
    }

    void OnMenu(InputAction.CallbackContext context) // ESC
    {
        _settingPanel.OnToggleSettings();
    }
    void OnInventory(InputAction.CallbackContext context) // Tap
    {
        _settingPanel.OnToggleInventory();
    }



    bool toggle = false;
    void OnPotalGun(InputAction.CallbackContext context) //Ctrl
    {
        toggle = !toggle;
        _animator.SetLayerWeight(2, toggle ? 1f : 0f);
        _animator.SetBool("IsGun", toggle);
    }
    void OnMouseL(InputAction.CallbackContext context)
    {
        _animator.SetTrigger("IsShoot");
    }
    void OnMouseR(InputAction.CallbackContext context)
    {
        _animator.SetTrigger("IsShoot");
    }
}
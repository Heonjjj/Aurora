using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ToggleCameras : MonoBehaviour
{
    public Camera playerCam;
    public Camera keypadCam;           // KeypadCam (Target Display = 1)
    public string playerTag = "Player";
    public bool autoEnterOnTrigger = true;
    public bool exitOnTriggerExit = true;
    public KeyCode exitKey = KeyCode.Escape;

    bool inside, inFocus;
    AudioListener playerAL, keypadAL;

    void Reset() { GetComponent<Collider>().isTrigger = true; }
    void Awake()
    {
        if (!playerCam) playerCam = Camera.main;

        playerAL = playerCam ? playerCam.GetComponent<AudioListener>() : null;
        keypadAL = keypadCam ? keypadCam.GetComponent<AudioListener>() : null;

        if (keypadCam) keypadCam.enabled = false;
        if (keypadAL) keypadAL.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        inside = true; Debug.Log("Enter trigger");
        if (autoEnterOnTrigger) Enter();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        inside = false; Debug.Log("Exit trigger");
        if (exitOnTriggerExit) Exit();
    }

    void Update() 
    {
        if (inFocus && Input.GetKeyDown(exitKey)) Exit();
       
    }

    public void Enter()
    {
        DirectionManager.Instance.OnDirection(true);
        if (inFocus) return; inFocus = true;
        if (playerCam) { playerCam.enabled = false; if (playerAL) playerAL.enabled = false; }
        if (keypadCam) { keypadCam.enabled = true; if (keypadAL) keypadAL.enabled = true; }

        DirectionManager.Instance?.LockOnCam(true);
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
    }

    public void Exit()
    {
        DirectionManager.Instance.OnDirection(false);
        if (!inFocus) return; inFocus = false;
        if (keypadCam) { keypadCam.enabled = false; if (keypadAL) keypadAL.enabled = false; }
        if (playerCam) { playerCam.enabled = true; if (playerAL) playerAL.enabled = true; }

        DirectionManager.Instance?.LockOnCam(false);
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }
 
}


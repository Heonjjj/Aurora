using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EventSecurity : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    GameObject securityGuard;
    private bool hasTriggered = false;
    public CinemachineFreeLook _cinematicCam;



    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        securityGuard = Resources.Load<GameObject>("SecurityGuard");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            Debug.Log("Security call");
            Instantiate(securityGuard, new Vector3(7, -7, -19), Quaternion.Euler(0, 0, 0));

            DirectionManager.Instance.Direction();

            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip footstepClip;
    private AudioSource audioSource;
    private CharacterController controller;
    public float footstepThreshold;
    public float footstepRate;
    private float footstepTime;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = footstepClip;
        audioSource.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        if (controller.isGrounded && speed > footstepThreshold)
        {
            if(Time.time -  footstepTime > footstepRate)
            {
                footstepTime = Time.time;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
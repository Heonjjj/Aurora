using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMovingStatue : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

    private bool hasTriggered = false;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            animator.SetTrigger("EnterCollider");
            audioSource.PlayOneShot(audioSource.clip);

            hasTriggered = true;
        }
    }
}

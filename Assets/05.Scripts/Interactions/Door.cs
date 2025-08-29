using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���¸� �����ϱ� ���� enum ����
public enum DoorState
{
    Closed,
    Open,
}

public class Door : MonoBehaviour
{
    private DoorState state;
    private Animator animator;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (animator != null)
        {
            // Open �����϶� �ִϸ������� �Ķ���͸� true�� ����
            animator.SetBool("DoorOpen", state == DoorState.Open);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            state = DoorState.Open;
            audioSource.PlayOneShot(audioSource.clip);
            Debug.Log("Door is now Open");
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            state = DoorState.Closed;
            audioSource.PlayOneShot(audioSource.clip);
            Debug.Log("Door is now Closed");
            return;
        }
    }

    // �ܺο��� �Ʒ��� ���� ȣ���Ͽ� ���¸� ����
    // curInteractGameObject.GetComponent<Door>().SetState();
    //public void SetState()
    //{
    //    switch (state)
    //    {
    //        // ���� Oen ���¶�� Closed�� ����
    //        case DoorState.Open:
    //            state = DoorState.Closed;
    //            audioSource.PlayOneShot(audioSource.clip);
    //            Debug.Log("Door is now Closed");
    //            return;

    //        // ���� Closed ���¶�� Open���� ����
    //        case DoorState.Closed:
    //            state = DoorState.Open;
    //            audioSource.PlayOneShot(audioSource.clip);
    //            Debug.Log("Door is now Open");
    //            return;
    //    }
    //}
}

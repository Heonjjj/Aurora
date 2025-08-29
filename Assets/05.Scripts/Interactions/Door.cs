using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 상태를 구별하기 위한 enum 선언
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
            // Open 상태일때 애니메이터의 파라미터를 true로 설정
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

    // 외부에서 아래와 같이 호출하여 상태를 변경
    // curInteractGameObject.GetComponent<Door>().SetState();
    //public void SetState()
    //{
    //    switch (state)
    //    {
    //        // 현재 Oen 상태라면 Closed로 변경
    //        case DoorState.Open:
    //            state = DoorState.Closed;
    //            audioSource.PlayOneShot(audioSource.clip);
    //            Debug.Log("Door is now Closed");
    //            return;

    //        // 현재 Closed 상태라면 Open으로 변경
    //        case DoorState.Closed:
    //            state = DoorState.Open;
    //            audioSource.PlayOneShot(audioSource.clip);
    //            Debug.Log("Door is now Open");
    //            return;
    //    }
    //}
}


using System.Collections;
using UnityEngine;

public class DoorController_keypad2 : MonoBehaviour
{
    public Animator animator;
    public float autoCloseDelay = 0f; // 자동으로 닫고 싶으면 > 0으로
    static readonly int DoorOpen = Animator.StringToHash("DoorOpen");
    Coroutine co;

    public void Open()                // 키패드 정답 시 호출할 함수
    {
        if (co != null) StopCoroutine(co);
        animator.SetBool(DoorOpen, true);  // 그래프가 Bool DoorOpen 열리도록 되어 있을 때

        if (autoCloseDelay > 0)
            co = StartCoroutine(AutoClose());
    }

    public void Close()
    {
        if (co != null) StopCoroutine(co);
        animator.SetBool(DoorOpen, false);
    }

    IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        Close();
    }
}


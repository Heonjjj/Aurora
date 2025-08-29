
using System.Collections;
using UnityEngine;

public class DoorController_keypad2 : MonoBehaviour
{
    public Animator animator;
    public float autoCloseDelay = 0f; // �ڵ����� �ݰ� ������ > 0����
    static readonly int DoorOpen = Animator.StringToHash("DoorOpen");
    Coroutine co;

    public void Open()                // Ű�е� ���� �� ȣ���� �Լ�
    {
        if (co != null) StopCoroutine(co);
        animator.SetBool(DoorOpen, true);  // �׷����� Bool DoorOpen �������� �Ǿ� ���� ��

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


using System.Collections;
using UnityEngine;

public class Tosting : MonoBehaviour
{
    [Header("Refer to an object that exists in the scene")]
    public GameObject itemA;         
    public GameObject itemB;         
    public float showSeconds = 5f;
    public bool oneShot = true;

    bool busy, done;

    // EŰ/����Ƽ �̺�Ʈ/Ű�е忡�� �� �޼��� ȣ��
    public void Interact()
    {
        if (busy || (oneShot && done)) return;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        busy = true;

        // ��Ÿ��
        if (itemA) itemA.SetActive(true);

        // 5�� ���
        yield return new WaitForSeconds(showSeconds);

        // ����� ��Ÿ��
        if (itemA) itemA.SetActive(false);
        if (itemB) itemB.SetActive(true);

        done = true;
        busy = false;
    }
}

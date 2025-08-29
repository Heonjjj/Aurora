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

    // E키/유니티 이벤트/키패드에서 이 메서드 호출
    public void Interact()
    {
        if (busy || (oneShot && done)) return;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        busy = true;

        // 나타남
        if (itemA) itemA.SetActive(true);

        // 5초 대기
        yield return new WaitForSeconds(showSeconds);

        // 숨기고 나타남
        if (itemA) itemA.SetActive(false);
        if (itemB) itemB.SetActive(true);

        done = true;
        busy = false;
    }
}

using TMPro;
using UnityEngine;

public class LockedDoorTrigger : MonoBehaviour
{
    public GameObject wallCollider;
    public string keyName;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        // 퍼즐매니저의 키체크 함수를 호출
        if (PuzzleManager.Instance.KeyCheck(other, wallCollider, keyName))
        {
            // 오브젝트가 파괴되기 직전에 임시오브젝트 생성(사운드 꺼질때까지)
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);

            // 임시 오브젝트를 파괴
            Destroy(gameObject);
        }

        return;
    }
}
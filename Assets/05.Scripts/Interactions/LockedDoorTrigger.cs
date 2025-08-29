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
        // ����Ŵ����� Űüũ �Լ��� ȣ��
        if (PuzzleManager.Instance.KeyCheck(other, wallCollider, keyName))
        {
            // ������Ʈ�� �ı��Ǳ� ������ �ӽÿ�����Ʈ ����(���� ����������)
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);

            // �ӽ� ������Ʈ�� �ı�
            Destroy(gameObject);
        }

        return;
    }
}
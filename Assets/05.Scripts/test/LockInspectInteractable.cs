using UnityEngine;

public class LockInspectInteractable : MonoBehaviour
{
    [TextArea] public string displayName = "�ڹ���";
    // public ItemInspectManager inspect;       // ���� �ν���Ʈ �Ŵ���
    public GameObject inspectPrefab;         // �ν���Ʈ�� ������ (������ ���� ����)
    public bool cloneFromThis = false;       // true�� ���� ������Ʈ �����ؼ� ���

    public string DisplayName => displayName;

    /* public void DoInteract()
    {
        if (!inspect) { Debug.LogWarning("ItemInspectManager ������"); return; }
        if (cloneFromThis) inspect.Open(gameObject);
        else inspect.Open(inspectPrefab);
    }*/
}

using UnityEngine;

public class LockInspectInteractable : MonoBehaviour
{
    [TextArea] public string displayName = "자물쇠";
    // public ItemInspectManager inspect;       // 씬의 인스펙트 매니저
    public GameObject inspectPrefab;         // 인스펙트용 프리팹 (깨끗한 버전 권장)
    public bool cloneFromThis = false;       // true면 현재 오브젝트 복제해서 띄움

    public string DisplayName => displayName;

    /* public void DoInteract()
    {
        if (!inspect) { Debug.LogWarning("ItemInspectManager 미지정"); return; }
        if (cloneFromThis) inspect.Open(gameObject);
        else inspect.Open(inspectPrefab);
    }*/
}

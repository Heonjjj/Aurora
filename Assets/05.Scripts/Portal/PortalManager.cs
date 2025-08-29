using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [Header("Prefabs (A/B 서로 다른 프리팹)")]
    public Portal portalPrefabA; // 파랑(A)
    public Portal portalPrefabB; // 빨강(B)

    [Header("Runtime (읽기용)")]
    public Portal portalA;
    public Portal portalB;

    void Awake()
    {
        if (!portalPrefabA || !portalPrefabB)
        {
            Debug.LogError("[PortalManager] portalPrefab 미할당!");
            enabled = false;
            return;
        }

        /*
        if (!portalA) portalA = Instantiate(portalPrefabA, new Vector3(0, -10, 0), Quaternion.identity, transform);
        if (!portalB) portalB = Instantiate(portalPrefabB, new Vector3(2, -10, 0), Quaternion.identity, transform);
        */

        // 서로 연결 - 이제 설치할 때로 옮김
        // LinkBoth();
    }
    
    // === 외부에서 호출: PlayerController_Portal.cs가 사용 ===
    public void PlaceA(Vector3 pos, Quaternion rot) => Place(ref portalA, portalPrefabA, pos, rot);
    /*
    {
        Debug.Log("포탈 A 설치");

        // 기존 A 있으면 파괴 → 새로 생성
        if (portalA) Destroy(portalA.gameObject);
        portalA = Instantiate(portalPrefabA, pos, rot, transform);
        LinkBoth(); // B가 있다면 서로 target 재설정
    }
    */

    public void PlaceB(Vector3 pos, Quaternion rot) => Place(ref portalB, portalPrefabB, pos, rot);
    /*
    {
        Debug.Log("포탈 B 설치");

        if (portalB) Destroy(portalB.gameObject);
        portalB = Instantiate(portalPrefabB, pos, rot, transform);
        LinkBoth();
    }
    */

    // === 내부 유틸 ===
    void Place(ref Portal slot, Portal prefab, Vector3 pos, Quaternion rot)
    {
        // 기존 A/B 있으면 파괴
        if (slot) Destroy(slot.gameObject);

        // 새로 생성(처음 배치)
        slot = Instantiate(prefab, pos, rot, transform);

        // 두 개 다 있을 때만 링크, 아니면 링크 해제
        LinkBoth();
    }

    void LinkBoth()
    {
        if (portalA && portalB)
        {
            portalA.target = portalB;
            portalB.target = portalA;
        }
        else
        {
            // 한쪽만 있으면 텔레포트 불가
            if (portalA) portalA.target = null;
            if (portalB) portalB.target = null;
        }
    }
}

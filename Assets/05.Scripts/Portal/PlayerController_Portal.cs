using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class oirtalvsvs : MonoBehaviour
{
    [Header("Portal")]
    [SerializeField] bool yawOnlyOnTeleport = true;    // 상하각 고정, 좌우(Yaw)만 맞춤
    [SerializeField] bool skipGravityOneFrame = true;  // 텔레포트 프레임 중력 스킵

    // 내부 플래그 (중력 1프레임 스킵용)
    bool _skipGravityThisFrame;

    [Header("Portal (Placement)")]
    [SerializeField] PortalManager portalManager;             // 인스펙터 할당
    [SerializeField] GameObject crosshair;                    // Ctrl로 표시/비표시
    //[SerializeField] LayerMask portalSurfaceMask;             // 설치 가능 표면
    //[SerializeField] LayerMask portalObstructMask;            // 설치 공간 방해 레이어
    [SerializeField] float maxPlaceDistance = 40f;            // 레이 거리
    [SerializeField] float portalDepthOffset = 0.01f;         // 표면 안 파고들지 않게

    // 포탈 전방 오브젝트 체크 (설치 대상 오브젝트와의 거리) 살짝 앞에서 검사할 예정
    [SerializeField] Vector3 portalHalfExtents = new Vector3 (0.5f, 1.0f, 0.05f); // 포탈 대략 크기    
    [SerializeField] float forwardClearance = 0.02f;
    // 조준 모드 (Ctrl로 토글)
    bool _portalMode;

    [SerializeField] float minExitUpSpeed = 3f;         // 바닥 포탈에서 최소 상승 속도
    [SerializeField, Range(0f, 1f)] float floorDot = 0.5f; // 출구가 '위쪽'을 향한다고 볼 임계값(코사인)
    Camera _camera;

    // hooks
    private void OnEnablePortal()
    {
        _portalMode = false;
        if (crosshair) crosshair.SetActive(false);
    }
    private void OnDisablePortal()
    {
        _portalMode = false;
        if (crosshair) crosshair.SetActive(false);
        _skipGravityThisFrame = false;
    }
    private void UpdatePortal() { /* 필요 시 사용*/ }

    // Portal.cs가 호출
    public void OnTeleported(Transform from, Transform to)
    {
        // 회전 보정
        if (yawOnlyOnTeleport)
        {
            Quaternion delta = to.rotation * Quaternion.Inverse(from.rotation);
            float newYaw = (delta * transform.rotation).eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, newYaw, 0f);
        }
        // 전체 회전 보존이 필요하면 위 블록을 끄고, Portal.cs가 세팅한 outRot을 그대로 사용하면 됨.

        // 중력 1프레임 스킵 (툭 떨어지는 느낌 방지)
        if (skipGravityOneFrame) _skipGravityThisFrame = true;

        // 출구 방향 킥 (CharacterController용)
        float d = Vector3.Dot(to.forward, Vector3.up);
        if (d >= floorDot)
        {
            // 바닥 포탈: 위로 최소 속도 확보
            //if (velocity.y < minExitUpSpeed) velocity.y = minExitUpSpeed;
        }
        else if (d <= -floorDot)
        {
            // 천장 포탈: 아래로 최소 속도 확보 (원하면 사용)
            //if (velocity.y > -minExitUpSpeed) velocity.y = -minExitUpSpeed;
        }

        // 만약 이후에 수평 속도를 직접 관리한다면, 아래처럼 회전시켜 주세요:
        // velocity = to.TransformDirection(from.InverseTransformDirection(velocity));
    }

    // 우클릭 = A(파랑), 좌클릭 = B(빨강)
    void PlacePortal(bool isA)
    {
        // Debug.Log("포탈 설치 메서드 진입");

        if (!TryRebindPortalManager()) //필요할 때만 리바인드 시도
        {
            Debug.LogWarning("[Portal] PortalManager not found in current scene.");
            return;
        }


        // 화면 중앙(조준선) 레이
        Ray ray = GetRayFromCrosshair(_camera, crosshair);

        // Portal, Blind 레이어 제외 전부 허용, 충돌체 정보 저장
        int portalLayer = LayerMask.NameToLayer("Portal");
        int blindLayer = LayerMask.NameToLayer("Blind");
        int mask = Physics.DefaultRaycastLayers;
        if (portalLayer >= 0) mask &= ~(1 << portalLayer);
        if (blindLayer >= 0) mask &= ~(1 << blindLayer);

        // RaycastAll로 전부 맞춘 뒤, 가장 가까운 유효 히트를 선택
        var hits = Physics.RaycastAll(ray, maxPlaceDistance, mask, QueryTriggerInteraction.Ignore);
        if (hits.Length == 0) return;       // 충돌 없을 시 리턴
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // 유효 태그 찾기 - 없으면 메서드 종료
        RaycastHit hit = default;
        bool found = false;
        foreach (var h in hits)
        {
            // 내 플레이어 콜라이더면 건너뛰기
            if (h.collider.GetComponentInParent<PlayerController>() == this) continue;

            // 블라인드(창문)이면 건너뛰기
            if (HasTagInParents(h.collider.transform, "Blind")) continue;

            // 부모까지 올라가며 PortalSurface 태그 확인, 없으면 건너뛰기
            if (!HasTagInParents(h.collider.transform, "PortalSurface")) continue;

            // 본인만 PortalSurface 태그 확인
            //if (!h.collider.transform.CompareTag("PortalSurface")) continue;


            hit = h;
            found = true;
            break;
        }

        if (!found)
        {
            // Debug.Log("[Portal] No valid surface in ray path (player or no PortalSurface tag).");
            return;
        }

        /*
        // 맞은 콜라이더 정보 디버깅
        var col = hit.collider;
        Debug.Log($"[Portal] Hit='{col.name}', tag='{col.tag}', layer='{LayerMask.LayerToName(col.gameObject.layer)}', path='{GetTransformPath(col.transform)}'");
        */

        // 표면 법선에 수직 정렬
        Vector3 pos = hit.point + hit.normal * portalDepthOffset;
        Quaternion baseRot = Quaternion.LookRotation(-hit.normal, Vector3.up);
        Quaternion rot = baseRot * Quaternion.Euler(0f, 180f, 0f);  // 프리팹에 맞게 y축 기준으로 뒤집기

        // 공간 여유 체크
        Vector3 checkCenter = pos + rot * Vector3.forward * (portalHalfExtents.z + forwardClearance);
        if (Physics.CheckBox(checkCenter, portalHalfExtents, rot, mask, QueryTriggerInteraction.Ignore))
        {
            var cols = Physics.OverlapBox(checkCenter, portalHalfExtents, rot, mask, QueryTriggerInteraction.Ignore);
            foreach (var c in cols)
            {
                // 기존 포탈/내 플레이어/블라인드는 무시
                if (c.GetComponentInParent<Portal>() != null) continue;
                if (c.GetComponentInParent<PlayerController>() == this) continue;
                if (HasTagInParents(c.transform, "Blind")) continue;
                
                Debug.Log($"[Portal] obstructed by {c.name} (layer={LayerMask.LayerToName(c.gameObject.layer)})");
            }
            return;
        }

        if (isA) portalManager.PlaceA(pos, rot);  // 파랑
        else portalManager.PlaceB(pos, rot);  // 빨강
    }

    Ray GetRayFromCrosshair(Camera cam, GameObject crosshair)
    {
        if (crosshair == null)
            return cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        var rt = crosshair.GetComponent<RectTransform>();
        if (rt == null)
            return cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        var canvas = rt.GetComponentInParent<Canvas>();
        Vector3 screenPos;

        // Screen Space - Overlay 는 월드→스크린 변환 불필요
        if (canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            screenPos = rt.position;
        else
            screenPos = RectTransformUtility.WorldToScreenPoint(cam, rt.position);

        return cam.ScreenPointToRay(screenPos);
    }

    bool HasTagInParents(Transform t, string tagName)
    {
        while (t != null)
        {
            if (t.CompareTag(tagName)) return true;
            t = t.parent;
        }
        return false;
    }

    bool TryRebindPortalManager()
    {
        // 이미 유효하게 연결되어 있으면 통과
        if (portalManager != null && portalManager.gameObject && portalManager.gameObject.scene.IsValid())
            return true;

        // 활성 씬에서만 탐색
        var pm = FindPortalManagerInActiveScene();
        if (pm != null)
        {
            portalManager = pm;
            Debug.Log($"[Portal] Rebound PortalManager: {pm.name}");
            return true;
        }

        return false;
    }

    PortalManager FindPortalManagerInActiveScene()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid()) return null;

        var roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            var pm = roots[i].GetComponentInChildren<PortalManager>(true); // 비활성 포함
            if (pm != null) return pm;
        }
        return null;
    }
}

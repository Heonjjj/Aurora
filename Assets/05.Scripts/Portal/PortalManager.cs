using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [Header("Prefabs (A/B ���� �ٸ� ������)")]
    public Portal portalPrefabA; // �Ķ�(A)
    public Portal portalPrefabB; // ����(B)

    [Header("Runtime (�б��)")]
    public Portal portalA;
    public Portal portalB;

    void Awake()
    {
        if (!portalPrefabA || !portalPrefabB)
        {
            Debug.LogError("[PortalManager] portalPrefab ���Ҵ�!");
            enabled = false;
            return;
        }

        /*
        if (!portalA) portalA = Instantiate(portalPrefabA, new Vector3(0, -10, 0), Quaternion.identity, transform);
        if (!portalB) portalB = Instantiate(portalPrefabB, new Vector3(2, -10, 0), Quaternion.identity, transform);
        */

        // ���� ���� - ���� ��ġ�� ���� �ű�
        // LinkBoth();
    }
    
    // === �ܺο��� ȣ��: PlayerController_Portal.cs�� ��� ===
    public void PlaceA(Vector3 pos, Quaternion rot) => Place(ref portalA, portalPrefabA, pos, rot);
    /*
    {
        Debug.Log("��Ż A ��ġ");

        // ���� A ������ �ı� �� ���� ����
        if (portalA) Destroy(portalA.gameObject);
        portalA = Instantiate(portalPrefabA, pos, rot, transform);
        LinkBoth(); // B�� �ִٸ� ���� target �缳��
    }
    */

    public void PlaceB(Vector3 pos, Quaternion rot) => Place(ref portalB, portalPrefabB, pos, rot);
    /*
    {
        Debug.Log("��Ż B ��ġ");

        if (portalB) Destroy(portalB.gameObject);
        portalB = Instantiate(portalPrefabB, pos, rot, transform);
        LinkBoth();
    }
    */

    // === ���� ��ƿ ===
    void Place(ref Portal slot, Portal prefab, Vector3 pos, Quaternion rot)
    {
        // ���� A/B ������ �ı�
        if (slot) Destroy(slot.gameObject);

        // ���� ����(ó�� ��ġ)
        slot = Instantiate(prefab, pos, rot, transform);

        // �� �� �� ���� ���� ��ũ, �ƴϸ� ��ũ ����
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
            // ���ʸ� ������ �ڷ���Ʈ �Ұ�
            if (portalA) portalA.target = null;
            if (portalB) portalB.target = null;
        }
    }
}

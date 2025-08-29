using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SafeFetchHelper
{

    //this�ϸ� ��ü�����ؿ�, early return���ϸ� �̻��Ѱ����� ����
    //get, getchild(true), findtype, add

    // -----------------------------
    // Required (������ ���� �α�)
    // -----------------------------

    /// <summary>
    /// GameObject���� �ʼ� ������Ʈ ��������.
    /// ������ ���� �α� ��� �� null ��ȯ.
    /// </summary>
    public static T GetOrError<T>(GameObject go) where T : Component
    {
        if (go.TryGetComponent<T>(out var comp))
            return comp;

        Debug.LogError($"[{go.name}]�� {typeof(T).Name} ������Ʈ�� �ʿ������� �����ϴ�!", go);
        return null;
    }

    /// <summary>
    /// GameObject �� �ڽĿ��� �ʼ� ������Ʈ ��������.
    /// ������ ���� �α� ��� �� null ��ȯ.
    /// </summary>
    public static T GetChild<T>(GameObject go) where T : Component
    {
        var comp = go.GetComponentInChildren<T>(true);
        if (comp != null) return comp;

        Debug.LogError($"[{go.name}] (�ڽ� ����)���� {typeof(T).Name} ������Ʈ�� ã�� �� �����ϴ�!", go);
        return null;
    }

    /// <summary>
    /// GameObject�� ������ AddComponent �ؼ� ������ �����ϱ�.
    /// </summary>
    public static T GetOrAdd<T>(GameObject go) where T : Component
    {
        if (go.TryGetComponent<T>(out var comp))
            return comp;

        comp = go.AddComponent<T>();
        Debug.LogWarning($"[{go.name}]�� {typeof(T).Name}��(��) ���� ���� �߰��߽��ϴ�.", go);
        return comp;
    }

    /// <summary>
    /// �� ��ü���� ã�ƺ��� ������ ���� ���� (DontDestroyOnLoad).
    /// �̱��� �Ŵ��� ���Ͽ� ����.
    /// </summary>
    public static T GetOrCreate<T>(string objectName = null) where T : Component
    {
        var instance = Object.FindObjectOfType<T>();
        if (instance != null) return instance;

        string goName = objectName ?? typeof(T).Name;
        var go = new GameObject(goName);
        instance = go.AddComponent<T>();
        Object.DontDestroyOnLoad(go);

        Debug.LogWarning($"���� {typeof(T).Name}��(��) ���� ���� �����߽��ϴ�: {goName}");
        return instance;
    }

    // -----------------------------
    // TryGet (������ false ��ȯ, �α� ����) , �����ϰ԰�������
    // -----------------------------

    /// <summary>
    /// GameObject���� �����ϰ� ������Ʈ �������� (������ false).
    /// </summary>
    public static bool TryGet<T>(GameObject go, out T comp) where T : Component
    {
        return go.TryGetComponent(out comp);
    }

    /// <summary>
    /// GameObject �� �ڽĿ��� �����ϰ� ������Ʈ �������� (������ false).
    /// </summary>
    public static bool TryGetInChildren<T>(GameObject go, out T comp) where T : Component
    {
        comp = go.GetComponentInChildren<T>(true);
        return comp != null;
    }

    /// <summary>
    /// �� ��ü���� �����ϰ� ������Ʈ �������� (������ false).
    /// </summary>
    public static bool TryFindInScene<T>(out T comp) where T : Component
    {
        comp = Object.FindObjectOfType<T>();
        return comp != null;
    }
}

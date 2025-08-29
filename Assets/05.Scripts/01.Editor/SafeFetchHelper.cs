using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SafeFetchHelper
{

    //this하면 물체참조해옴, early return안하면 이상한곳에서 터짐
    //get, getchild(true), findtype, add

    // -----------------------------
    // Required (없으면 에러 로그)
    // -----------------------------

    /// <summary>
    /// GameObject에서 필수 컴포넌트 가져오기.
    /// 없으면 에러 로그 출력 후 null 반환.
    /// </summary>
    public static T GetOrError<T>(GameObject go) where T : Component
    {
        if (go.TryGetComponent<T>(out var comp))
            return comp;

        Debug.LogError($"[{go.name}]에 {typeof(T).Name} 컴포넌트가 필요하지만 없습니다!", go);
        return null;
    }

    /// <summary>
    /// GameObject 및 자식에서 필수 컴포넌트 가져오기.
    /// 없으면 에러 로그 출력 후 null 반환.
    /// </summary>
    public static T GetChild<T>(GameObject go) where T : Component
    {
        var comp = go.GetComponentInChildren<T>(true);
        if (comp != null) return comp;

        Debug.LogError($"[{go.name}] (자식 포함)에서 {typeof(T).Name} 컴포넌트를 찾을 수 없습니다!", go);
        return null;
    }

    /// <summary>
    /// GameObject에 없으면 AddComponent 해서 강제로 보장하기.
    /// </summary>
    public static T GetOrAdd<T>(GameObject go) where T : Component
    {
        if (go.TryGetComponent<T>(out var comp))
            return comp;

        comp = go.AddComponent<T>();
        Debug.LogWarning($"[{go.name}]에 {typeof(T).Name}이(가) 없어 새로 추가했습니다.", go);
        return comp;
    }

    /// <summary>
    /// 씬 전체에서 찾아보고 없으면 새로 생성 (DontDestroyOnLoad).
    /// 싱글톤 매니저 패턴에 유용.
    /// </summary>
    public static T GetOrCreate<T>(string objectName = null) where T : Component
    {
        var instance = Object.FindObjectOfType<T>();
        if (instance != null) return instance;

        string goName = objectName ?? typeof(T).Name;
        var go = new GameObject(goName);
        instance = go.AddComponent<T>();
        Object.DontDestroyOnLoad(go);

        Debug.LogWarning($"씬에 {typeof(T).Name}이(가) 없어 새로 생성했습니다: {goName}");
        return instance;
    }

    // -----------------------------
    // TryGet (없으면 false 반환, 로그 없음) , 안전하게가져오기
    // -----------------------------

    /// <summary>
    /// GameObject에서 안전하게 컴포넌트 가져오기 (없으면 false).
    /// </summary>
    public static bool TryGet<T>(GameObject go, out T comp) where T : Component
    {
        return go.TryGetComponent(out comp);
    }

    /// <summary>
    /// GameObject 및 자식에서 안전하게 컴포넌트 가져오기 (없으면 false).
    /// </summary>
    public static bool TryGetInChildren<T>(GameObject go, out T comp) where T : Component
    {
        comp = go.GetComponentInChildren<T>(true);
        return comp != null;
    }

    /// <summary>
    /// 씬 전체에서 안전하게 컴포넌트 가져오기 (없으면 false).
    /// </summary>
    public static bool TryFindInScene<T>(out T comp) where T : Component
    {
        comp = Object.FindObjectOfType<T>();
        return comp != null;
    }
}

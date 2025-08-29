using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    //private UIManager uiManager;

    public static ResourceManager Instance
    {
        get
        {
            // 할당되지 않았을 때, 외부에서 ResourceManager.Instance 로 접근하는 경우
            // 게임 오브젝트를 만들어주고 ResourceManager 스크립트를 AddComponent로 붙여준다.
            if (_instance == null)
            {
                // 게임오브젝트가 없어도 시작시 없는걸 확인후 매니저를 게임오브젝트로 생성해줌
                _instance = new GameObject("ResourceManager").AddComponent<ResourceManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Awake가 호출 될 때라면 이미 매니저 오브젝트는 생성되어 있는 것이고, '_instance'에 자신을 할당
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 오브젝트가 존재하는 경우 '자신'을 파괴해서 중복방지
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        //uiManager = UIManager.Instance;
    }

    // 프리팹을 생성하고 초기화하는 메서드 (UI 폴더용)
    // T InstantiateUI<T> : 제네릭이란뜻T + 함수이름 + <호출할 곳에서 쓸 제네릭 타입>
    // 매개변수 : 프리펩(이름), 자식으로붙일 부모오브젝트(위치)
    // where T : BaseUI <- BaseUI를 상속받는,
    //public T InstantiateUI<T>(string prefabName, Transform parent) where T : BaseUI
    //{
    //    // Resources(루트폴더) 기준 경로 설정
    //    // 경로에서 이름이 일치하는 프리펩 가져와서 prefab 변수에 저장 (정보만 저장된 상태)
    //    GameObject prefab = Resources.Load<GameObject>("UI/" + prefabName);
    //
    //    // prefab 변수에 저장된 내용을 씬에다가 오브젝트로 생성 (Instantiate함수)
    //    GameObject uiObject = Instantiate(prefab, parent);
    //
    //    // BaseUI(T조건) 컴포넌트를 찾아 uiComponent 변수에 저장
    //    T uiComponent = uiObject.GetComponent<T>();
    //
    //    // Init 함수 호출하여 초기화
    //    if (uiComponent != null)
    //    {
    //        //uiComponent.Init(uiManager);
    //    }
    //
    //    return uiComponent;
    //}
}

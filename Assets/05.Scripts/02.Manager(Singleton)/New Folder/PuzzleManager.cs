using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    private static PuzzleManager _instance;
    public InventoryVM inventoryViewModel;
    private string keyName;

    public static PuzzleManager Instance
    {
        get
        {
            // 할당되지 않았을 때, 외부에서 PuzzleManager.Instance 로 접근하는 경우
            // 게임 오브젝트를 만들어주고 PuzzleManager 스크립트를 AddComponent로 붙여준다.
            if (_instance == null)
            {
                // 게임오브젝트가 없어도 시작시 없는걸 확인후 매니저를 게임오브젝트로 생성해줌
                _instance = new GameObject("PuzzleManager").AddComponent<PuzzleManager>();
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

        inventoryViewModel = UI_Manager.Instance._invenVM;
    }

    // Key 있어야 열리는 퍼즐
    public bool KeyCheck(Collider other, GameObject wall, string keyName)
    {
        if (other.CompareTag("Player"))
        {
            // 이제 inventoryViewModel 변수를 통해 HasItem 함수를 호출할 수 있습니다.

            //Destroy(wall);
            //Debug.Log("문을 열었습니다.");
            //return true;
        }
        return false;
    }
}

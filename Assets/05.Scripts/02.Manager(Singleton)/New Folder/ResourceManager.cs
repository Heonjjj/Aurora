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
            // �Ҵ���� �ʾ��� ��, �ܺο��� ResourceManager.Instance �� �����ϴ� ���
            // ���� ������Ʈ�� ������ְ� ResourceManager ��ũ��Ʈ�� AddComponent�� �ٿ��ش�.
            if (_instance == null)
            {
                // ���ӿ�����Ʈ�� ��� ���۽� ���°� Ȯ���� �Ŵ����� ���ӿ�����Ʈ�� ��������
                _instance = new GameObject("ResourceManager").AddComponent<ResourceManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Awake�� ȣ�� �� ����� �̹� �Ŵ��� ������Ʈ�� �����Ǿ� �ִ� ���̰�, '_instance'�� �ڽ��� �Ҵ�
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // �̹� ������Ʈ�� �����ϴ� ��� '�ڽ�'�� �ı��ؼ� �ߺ�����
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        //uiManager = UIManager.Instance;
    }

    // �������� �����ϰ� �ʱ�ȭ�ϴ� �޼��� (UI ������)
    // T InstantiateUI<T> : ���׸��̶���T + �Լ��̸� + <ȣ���� ������ �� ���׸� Ÿ��>
    // �Ű����� : ������(�̸�), �ڽ����κ��� �θ������Ʈ(��ġ)
    // where T : BaseUI <- BaseUI�� ��ӹ޴�,
    //public T InstantiateUI<T>(string prefabName, Transform parent) where T : BaseUI
    //{
    //    // Resources(��Ʈ����) ���� ��� ����
    //    // ��ο��� �̸��� ��ġ�ϴ� ������ �����ͼ� prefab ������ ���� (������ ����� ����)
    //    GameObject prefab = Resources.Load<GameObject>("UI/" + prefabName);
    //
    //    // prefab ������ ����� ������ �����ٰ� ������Ʈ�� ���� (Instantiate�Լ�)
    //    GameObject uiObject = Instantiate(prefab, parent);
    //
    //    // BaseUI(T����) ������Ʈ�� ã�� uiComponent ������ ����
    //    T uiComponent = uiObject.GetComponent<T>();
    //
    //    // Init �Լ� ȣ���Ͽ� �ʱ�ȭ
    //    if (uiComponent != null)
    //    {
    //        //uiComponent.Init(uiManager);
    //    }
    //
    //    return uiComponent;
    //}
}

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
            // �Ҵ���� �ʾ��� ��, �ܺο��� PuzzleManager.Instance �� �����ϴ� ���
            // ���� ������Ʈ�� ������ְ� PuzzleManager ��ũ��Ʈ�� AddComponent�� �ٿ��ش�.
            if (_instance == null)
            {
                // ���ӿ�����Ʈ�� ��� ���۽� ���°� Ȯ���� �Ŵ����� ���ӿ�����Ʈ�� ��������
                _instance = new GameObject("PuzzleManager").AddComponent<PuzzleManager>();
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

        inventoryViewModel = UI_Manager.Instance._invenVM;
    }

    // Key �־�� ������ ����
    public bool KeyCheck(Collider other, GameObject wall, string keyName)
    {
        if (other.CompareTag("Player"))
        {
            // ���� inventoryViewModel ������ ���� HasItem �Լ��� ȣ���� �� �ֽ��ϴ�.

            //Destroy(wall);
            //Debug.Log("���� �������ϴ�.");
            //return true;
        }
        return false;
    }
}

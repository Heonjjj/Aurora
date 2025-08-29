using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Player>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("Player");
                    _instance = go.AddComponent<Player>();
                }
            }
            return _instance;
        }
    }

    public PlayerController _controller;

    private void Awake()
    {
        Debug.Log("Player Awake in scene: " + gameObject.scene.name);
        if (_instance != null && _instance != this)
        {
            Debug.Log("Duplicate Player found, destroying this one: " + gameObject.name);
            transform.SetParent(null); // �θ�(Canvas)���� �и�
            Destroy(gameObject); // �̹� �ٸ� ������ �ı�
            return;
        }
        _instance = this; // ���⼭ ���
        DontDestroyOnLoad(gameObject);

        _controller = SafeFetchHelper.GetOrError<PlayerController>(gameObject);
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// ������ UI ���¸� �����ϴ� ������
public enum GameState
{
    Intro,
    Playing,
    Paused,
    GameOver,
}

public partial class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    // GameState Ÿ���� ����ϴ� �̺�Ʈ �Լ� OnGameStateChanged�� ����
    public static event Action<GameState> OnGameStateChanged;
    private GameState _currentState;
    bool IsPause = false;

    // ���� ���� ���� �� ���� ����
    private Scene currentScene;

    public static GameManager Instance
    {
        get
        {
            // �Ҵ���� �ʾ��� ��, �ܺο��� GameManager.Instance �� �����ϴ� ���
            // ���� ������Ʈ�� ������ְ� GameManager ��ũ��Ʈ�� AddComponent�� �ٿ��ش�.
            if (_instance == null)
            {
                // ���ӿ�����Ʈ�� ��� ���۽� ���°� Ȯ���� �Ŵ����� ���ӿ�����Ʈ�� ��������
                _instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return _instance;
        }
    }

    public GameState CurrentGameState
    {
        // CurrentGameState ȣ���� ���� _currentState ��ȯ
        get { return _currentState; }
        private set
        {
            // CurrentGameState ȣ���� ������ �ѱ� ����(value)�� _currentState ������ ����
            _currentState = value;
            // OnGameStateChanged��� �Լ��� ���� ������ �ִٸ�? ���� �̺�Ʈ �߻� (Invoke)
            OnGameStateChanged?.Invoke(_currentState);
        }
    }

    // Player �ν��Ͻ��� �����ϱ� ���� Instance �Լ�

    private void Awake()
    {
        var direction = DirectionManager.Instance;
        var audio = AudioManager.Instance;


        // Awake�� ȣ�� �� ����� �̹� �Ŵ��� ������Ʈ�� �����Ǿ� �ִ� ���̰�, '_instance'�� �ڽ��� �Ҵ�
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // ���Ŵ��� ���ε� �̺�Ʈ�� ���ӸŴ����� OnSceneLoaded �Լ��� ����
        }
        else
        {
            // �̹� ������Ʈ�� �����ϴ� ��� '�ڽ�'�� �ı��ؼ� �ߺ�����
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        // ���� ���� �����ͼ� ������ ����
        currentScene = SceneManager.GetActiveScene();
    }
    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    // GameState ��ȯ �Լ�
    public void ChangeGameState(GameState newState)
    {
        CurrentGameState = newState;
    }

    // ���� ����
    public void StartGame()
    {
        // SceneManagement�� ���� 1�� ������ ��ȯ
        SceneManager.LoadScene(1);
    }

    // ���� �Ͻ�����
    public void PauseGame()
    {
        // GameState ���¸� Paused�� ��ȯ
        ChangeGameState(GameState.Paused);
        Debug.Log($"GameState : {_currentState}");

        // ���� ����
        if (IsPause == false)
        {
            Time.timeScale = 0;
            IsPause = true;
            return;
        }
    }

    // ���� �Ͻ�����
    public void ReturnGame()
    {
        // GameState ���¸� Playing���� ��ȯ
        ChangeGameState(GameState.Playing);

        // ���� ����
        if (IsPause == true)
        {
            Time.timeScale = 1;
            IsPause = false;
            return;
        }
    }

    // ���� ����
    public void GameOver()
    {
        // GameState ���¸� GameOver�� ��ȯ
        ChangeGameState(GameState.GameOver);
    }

    // SceneManager.sceneLoaded ��� �̺�Ʈ�� ���� �� �Լ�
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �ε�� ���� buildIndex�� ���� UI ���¸� �ٸ��� ����
        //switch (scene.buildIndex)
        //{
        //    case 0:
        //        ChangeGameState(GameState.Intro);
        //        break;
        //    case 1:
        //        ChangeGameState(GameState.Playing);
        //        break;
        //}

        // �� ���� �̸��� "00"�� ���
        if (scene.name == "IntroScene")
        {
            DirectionManager.Instance.Direction_Intro();
            
        }

        // �� ���� �̸��� "00"�� ���
        if (scene.name == "MainScene_Floor2")
        {
            DirectionManager.Instance._mainCam.gameObject.SetActive(true);
            Player.Instance._controller.LockOnInput(0);
            Cursor.lockState = CursorLockMode.Locked;

            // AudioManager�� bgmGame2 ������ �Ҵ�� ����� Ŭ���� ������ ���
            AudioManager.Instance.bgmSource.clip = AudioManager.Instance.bgmGame;
            // �Ҹ� ���
            AudioManager.Instance.bgmSource.Play();
        }
        // �� ���� �̸��� "00"�� ���
        else if (scene.name == "MainScene_Floor1")
        {
            // AudioManager�� bgmGame2 ������ �Ҵ�� ����� Ŭ���� ������ ���
            AudioManager.Instance.bgmSource.clip = AudioManager.Instance.bgmGame;
            // �Ҹ� ���
            AudioManager.Instance.bgmSource.Play();

            Debug.Log("��ǥ����");
            // �÷��̾��� ��ġ�� Ư�� ��ǥ�� ����
            Player.Instance.transform.position = new Vector3(7, -6, -18);
        }
        else if (scene.name == "EndingScene")
        {
            // AudioManager�� bgmGame2 ������ �Ҵ�� ����� Ŭ���� ������ ���
            AudioManager.Instance.bgmSource.clip = AudioManager.Instance.bgmGame2;
            // �Ҹ� ���
            AudioManager.Instance.bgmSource.Play();
        }
    }

    private void OnDestroy()
    {
        // ������Ʈ�� �ı��� �� �̺�Ʈ ��� ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

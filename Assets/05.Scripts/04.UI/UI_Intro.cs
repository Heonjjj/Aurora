using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class UI_Intro : MonoBehaviour
{
    public GameObject _startPanel;
    public Button startButton;

    GameObject _guiPanel;
    VideoPlayer _videoPlayer;

    private void Start()
    {
        _videoPlayer = SafeFetchHelper.GetChild<VideoPlayer>(gameObject);
        _videoPlayer.Play();
        _guiPanel = UI_Manager.Instance._guiPanel;

        if (startButton != null)
            startButton.onClick.AddListener(OnStart);
    }

    public void OnStart()
    {
        _startPanel.SetActive(false);
        _guiPanel.SetActive(true);
        DirectionManager.Instance.Direction_Intro();
        AudioManager.Instance.PlayBGM("Start");
        _videoPlayer.Stop();

        //SceneManager.LoadScene("MainScene_Floor2");
    }

}
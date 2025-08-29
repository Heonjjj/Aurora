using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = UnityEngine.SceneManagement.Scene;
using Cysharp.Threading.Tasks;
using UnityEngine.Video;

public class UI_SettingPanel : MonoBehaviour
{
    public Slider _bgmSlider;
    public Slider _sfxSlider;
    public PostProcessVolume _postProcessing;

    GameObject _savePanel;
    InventoryV _invenV;

    public void InitPanel()
    {
        _savePanel = UI_Manager.Instance._savePanel;
        _invenV = SafeFetchHelper.GetChild<InventoryV>(UI_Manager.Instance.gameObject);
        _postProcessing.enabled = false;
    }

    public void OpenUI()
    {
        Time.timeScale = 0f;
        DirectionManager.Instance.LockOnCam(true);
        Cursor.lockState = CursorLockMode.None;
        _postProcessing.enabled = true;
    }
    public async void CloseUI()
    {
        Time.timeScale = 1f;
        DirectionManager.Instance.LockOnCam(false);
        Cursor.lockState = CursorLockMode.Locked;
        _postProcessing.enabled = false;
    }

    public void OnToggleSettings()
    {
        if (!gameObject.activeSelf)
        {
            OpenUI();
            gameObject.SetActive(true);

            // 리스너 해제 (안 해도 되지만 깔끔하게 하려면 유지)
            _bgmSlider.onValueChanged.RemoveListener(OnBgmSlider);
            _sfxSlider.onValueChanged.RemoveListener(OnSfxSlider);
        }
        else
        {
            CloseUI();
            gameObject.SetActive(false);

            // 슬라이더 값 세팅
            _bgmSlider.value = AudioManager.Instance.bgmVolume;
            _sfxSlider.value = AudioManager.Instance.sfxVolume;
            // 리스너 등록, 실시간반영용
            _bgmSlider.onValueChanged.AddListener(OnBgmSlider);
            _sfxSlider.onValueChanged.AddListener(OnSfxSlider);

        }
    }
    public void OnToggleInventory()
    {
        if (!_invenV.gameObject.activeSelf)
        {
            OpenUI();
            _invenV.gameObject.SetActive(true);
        }
        else
        {
            CloseUI();
            _invenV.gameObject.SetActive(false);
        }
    }



    public void OnCloseTheScene()
    {
        SceneManager.LoadScene(0);
    }

    public void OnSave()
    {
        _savePanel.SetActive(!_savePanel.activeSelf);
    }
    public void OnLoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // 현재 씬 인덱스
        SceneManager.LoadScene(currentSceneIndex + 1); // 다음 씬으로 이동
    }
    public void OnGodMode()
    {

    }

    public void OnBgmSlider(float value)
    {
        AudioManager.Instance.SetBgmVolume(value);
    }
    public void OnSfxSlider(float value)
    {
        AudioManager.Instance.SetSfxVolume(value);
    }
    public void OnGameOver()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 플레이 모드 종료
#else
              Application.Quit(); // 빌드된 게임 종료
#endif
    }
}
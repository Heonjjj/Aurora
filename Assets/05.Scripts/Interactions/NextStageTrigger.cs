using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStageTrigger : MonoBehaviour
{
    // 현재 씬을 저장 할 변수 세팅
    private Scene currentScene;

    private void Awake()
    {
        // 현재 씬을 가져와서 변수에 저장
        currentScene = SceneManager.GetActiveScene();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 콜라이더에 닿은게 "플레이어"라면,
        if (other.CompareTag("Player"))
        {
            int scenebuildIndex = currentScene.buildIndex;

            // 현재 씬번호의 다음 씬을 불러옴
            SceneManager.LoadScene(scenebuildIndex + 1);
        }
    }
}

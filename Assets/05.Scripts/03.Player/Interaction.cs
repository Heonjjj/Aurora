using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Interaction : MonoBehaviour //itemobject랑 상호작용만 하는 클래스
{
    [Header("UI")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TMP_Text promptText;

    [Header("Interaction Settings")]
    public float interactionRadius = 2f; // 상호작용 범위
    private IInteractable nearestItem;

    private void Start()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);

        // UniTask 루프 시작
        CheckInteractablesLoop().Forget();
    }

    private void Update()
    {
        // E키 입력 시 상호작용
        if (Input.GetKeyDown(KeyCode.E) && nearestItem != null)
        {
            nearestItem.OnInteract();
        }

        // UI 표시
        if (nearestItem != null)
        {
            promptPanel.SetActive(true);
            promptText.text = nearestItem.GetInteractPrompt();
        }
        else
        {
            promptPanel.SetActive(false);
        }
    }

    private async UniTaskVoid CheckInteractablesLoop()
    {
        while (this != null) // 오브젝트 파괴되면 자동종료
        {
            nearestItem = FindNearestInteractable();

            // 0.3초 대기
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }

    private IInteractable FindNearestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius);

        IInteractable closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var item = hit.GetComponent<IInteractable>();
            if (item != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = item;
                }
            }
        }

        return closest;
    }

    // 선택 사항: 시각화용
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
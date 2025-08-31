using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Interaction : MonoBehaviour //itemobject�� ��ȣ�ۿ븸 �ϴ� Ŭ����
{
    [Header("UI")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TMP_Text promptText;

    [Header("Interaction Settings")]
    public float interactionRadius = 2f; // ��ȣ�ۿ� ����
    private IInteractable nearestItem;

    private void Start()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);

        // UniTask ���� ����
        CheckInteractablesLoop().Forget();
    }

    private void Update()
    {
        // EŰ �Է� �� ��ȣ�ۿ�
        if (Input.GetKeyDown(KeyCode.E) && nearestItem != null)
        {
            nearestItem.OnInteract();
        }

        // UI ǥ��
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
        while (this != null) // ������Ʈ �ı��Ǹ� �ڵ�����
        {
            nearestItem = FindNearestInteractable();

            // 0.3�� ���
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

    // ���� ����: �ð�ȭ��
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
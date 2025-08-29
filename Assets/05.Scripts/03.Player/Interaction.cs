using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Interaction : MonoBehaviour //itemobject�� ��ȣ�ۿ븸 �ϴ� Ŭ����
{
    [Header("UI")]
    private GameObject promptText;
    private TMP_Text text;

    [Header("Interaction Settings")]
    public float interactionRadius = 2f; // ��ȣ�ۿ� ����
    private List<IInteractable> itemsInRange = new List<IInteractable>();

    private void Start()
    {
        promptText = UI_Manager.Instance._promptText;
        text = promptText.GetComponentInChildren<TMP_Text>();
        promptText.SetActive(false);
    }

    private void Update()
    {
        // �� ������ �÷��̾� �ֺ� ���� üũ, üũ�ϴ½ð� ���翩���ҵ� 0.5�ʿ� 1��?
        itemsInRange.Clear();
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius);
        foreach (var hit in hits)
        {
            var item = hit.GetComponent<IInteractable>();
            if (item != null)
            {
                itemsInRange.Add(item);
            }
        }

        // ���� �� �������� ������ ������Ʈ ǥ��
        if (itemsInRange.Count > 0)
        {
            promptText.SetActive(true);
            text.text = itemsInRange[itemsInRange.Count - 1].GetInteractPrompt(); // ���� ����� ������
        }
        else
        {
            promptText.SetActive(false);
        }

        // E Ű �Է� �� ���� ����� ������ ��ȣ�ۿ�
        if (Input.GetKeyDown(KeyCode.E) && itemsInRange.Count > 0)
        {
            var item = itemsInRange[itemsInRange.Count - 1];
            item.OnInteract(); // Player ����
            return;
        }
    }

    // ���� ����: �ð�ȭ��
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
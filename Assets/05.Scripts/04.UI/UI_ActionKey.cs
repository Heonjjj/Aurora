using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActionKey : MonoBehaviour
{
    public RectTransform parent; // ���� ��ġ�� �θ�
    public float radius = 100f;  // �� ������
    public float duration = 0.5f; // �������� �ð�

    private void OnEnable()
    {
        StartCoroutine(AnimateCircle());
    }

    IEnumerator AnimateCircle()
    {
        int childCount = parent.childCount;

        Vector3[] startPositions = new Vector3[childCount];
        Vector3[] targetPositions = new Vector3[childCount];

        // ��ǥ ��ġ ���
        for (int i = 0; i < childCount; i++)
        {
            float angle = i * Mathf.PI * 2f / childCount;
            targetPositions[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            startPositions[i] = Vector3.zero; // �߾ӿ��� ����
            parent.GetChild(i).localPosition = startPositions[i];
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            for (int i = 0; i < childCount; i++)
            {
                parent.GetChild(i).localPosition = Vector3.Lerp(startPositions[i], targetPositions[i], t);
            }

            yield return null;
        }

        // ���� ��ġ ����
        for (int i = 0; i < childCount; i++)
        {
            parent.GetChild(i).localPosition = targetPositions[i];
        }
    }
}
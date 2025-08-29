using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActionKey : MonoBehaviour
{
    public RectTransform parent; // 원형 배치할 부모
    public float radius = 100f;  // 원 반지름
    public float duration = 0.5f; // 펼쳐지는 시간

    private void OnEnable()
    {
        StartCoroutine(AnimateCircle());
    }

    IEnumerator AnimateCircle()
    {
        int childCount = parent.childCount;

        Vector3[] startPositions = new Vector3[childCount];
        Vector3[] targetPositions = new Vector3[childCount];

        // 목표 위치 계산
        for (int i = 0; i < childCount; i++)
        {
            float angle = i * Mathf.PI * 2f / childCount;
            targetPositions[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            startPositions[i] = Vector3.zero; // 중앙에서 시작
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

        // 최종 위치 보정
        for (int i = 0; i < childCount; i++)
        {
            parent.GetChild(i).localPosition = targetPositions[i];
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // 실행 안 해도 에디터에서 동작
public class UI_CircleLayout : MonoBehaviour
{
    public float radius = 100f;
    public float startAngle = 0f;
    public bool clockwise = true;

    void OnValidate()
    {
        Arrange();
    }

    void Update()
    {
        // 실행 중일 때도 정렬 유지
        if (Application.isPlaying)
            Arrange();
    }

    void Arrange()
    {
        int count = transform.childCount;
        if (count == 0) return;

        float angleStep = 360f / count;
        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            if (child == null) continue;

            float angle = startAngle + (clockwise ? -angleStep * i : angleStep * i);
            float rad = angle * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            child.localPosition = pos;
        }
    }

    [ContextMenu("Arrange Children In Circle")]
    void ArrangeChildren()
    {
        Arrange();
    }
}

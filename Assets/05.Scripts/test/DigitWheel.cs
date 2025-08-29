using System;
using System.Collections;   // �ڷ�ƾ��
using UnityEngine;

[ExecuteAlways]                 // �����Ϳ����� �ٷ� �ݿ�
[DisallowMultipleComponent]
public class DigitWheel : MonoBehaviour
{
    [Header("Value")]
    [Range(0, 9)] public int startValue = 0;
    public int Value { get; private set; }

    [Header("Visual")]
    public Transform visual;        // ���� �� Transform 
    public float stepDegrees = 36f; // 360/10
    public float rotateSpeed = 540f;
    public bool smoothRotate = true; 

    public Action onChanged;        

    float targetAngleY;
    bool rotating;

    void Awake()
    {
        // ������/��Ÿ�� ��ο��� ���� �� ����
        SetValue(startValue, instant: !Application.isPlaying);
    }

    void Start()
    {
       
        SetValue(startValue, instant: true);
    }

    // �ν����Ϳ��� �� �ٲٸ� ��� �ݿ�
    void OnValidate()
    {
        if (stepDegrees <= 0f) stepDegrees = 36f;
        SetValue(Mathf.Clamp(startValue, 0, 9), instant: true);
    }

    public void Next() => SetValue((Value + 1) % 10);
    public void Prev() => SetValue((Value + 9) % 10);

    public void SetValue(int v, bool instant = false)
    {
        v = Mathf.Clamp(v, 0, 9);
        if (!instant && v == Value) return;

        Value = v;
        onChanged?.Invoke();

        if (!visual) return;

        targetAngleY = +Value * stepDegrees;   

        if (instant || !Application.isPlaying || !smoothRotate)
        {
            var e = visual.localEulerAngles;
            e.y = targetAngleY;
            visual.localEulerAngles = e;
        }
        else
        {
            if (!rotating) StartCoroutine(RotateRoutineY());
        }
    }

    IEnumerator RotateRoutineY()
    {
        rotating = true;
        while (true)
        {
            var e = visual.localEulerAngles;
            float current = e.y;
            if (current > 180f) current -= 360f; // [-180,180]�� ����
            float next = Mathf.MoveTowards(current, targetAngleY, rotateSpeed * Time.deltaTime);
            e.y = next;
            visual.localEulerAngles = e;

            if (Mathf.Approximately(next, targetAngleY)) break;
            yield return null;
        }
        rotating = false;
    }
}

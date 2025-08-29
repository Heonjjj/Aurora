using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue; // 변화하는 값
    public float maxValue; // 최대 값
    public float startValue; // 시작 값
    public float passiveValue; // 자동으로 변화 할 값
    public Image uiBar; // UI에서 게이지Bar로 쓸 이미지 타입

    private void Start()
    {
        curValue = startValue; // 시작 시 curValue를 startValue 저장
    }

    private void Update()
    {
        uiBar.fillAmount = GetPercentage(); // UIBar 이미지의 fillAmount를 GetPercentage함수에 의해 조절
    }

    // 증가 함수
    public void Add(float amount)
    {
        // amount 값을 더한 값이 maxValue를 넘으면 maxValue로 설정
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    // 감소 함수
    public void Subtract(float amount)
    {
        // amount 값을 뺀 값이 0보다 낮으면 minValue로 설정
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }

    // 현재 값과 최대 값의 비율로 게이지를 조절하는 함수
    public float GetPercentage()
    {
        return curValue / maxValue;
    }
}
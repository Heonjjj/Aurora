using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue; // ��ȭ�ϴ� ��
    public float maxValue; // �ִ� ��
    public float startValue; // ���� ��
    public float passiveValue; // �ڵ����� ��ȭ �� ��
    public Image uiBar; // UI���� ������Bar�� �� �̹��� Ÿ��

    private void Start()
    {
        curValue = startValue; // ���� �� curValue�� startValue ����
    }

    private void Update()
    {
        uiBar.fillAmount = GetPercentage(); // UIBar �̹����� fillAmount�� GetPercentage�Լ��� ���� ����
    }

    // ���� �Լ�
    public void Add(float amount)
    {
        // amount ���� ���� ���� maxValue�� ������ maxValue�� ����
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    // ���� �Լ�
    public void Subtract(float amount)
    {
        // amount ���� �� ���� 0���� ������ minValue�� ����
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }

    // ���� ���� �ִ� ���� ������ �������� �����ϴ� �Լ�
    public float GetPercentage()
    {
        return curValue / maxValue;
    }
}
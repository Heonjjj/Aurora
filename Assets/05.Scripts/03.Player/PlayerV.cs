using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerV : MonoBehaviour
{
    [Header("Bars")]
    public Image healthImage;
    public Image staminaImage;
    public Image expImage;

    [Header("Stats")]
    public TMP_Text attackText;
    public TMP_Text defenseText;
    public TMP_Text critChanceText;
    public TMP_Text critDamageText;
    public TMP_Text penetrationText;
    public TMP_Text staminaRegenText;
    public TMP_Text levelText;
    public TMP_Text healthText;    

    [Header("Optional Animations")]
    public Animator levelUpAnimator;

    private PlayerVM viewModel;
    private CompositeDisposable disposables = new CompositeDisposable();


    public void Init(PlayerVM vm)
    {
        viewModel = vm;

        // ü�� ��, ���� ��ȭ (��� -> ����)
        viewModel.Health.Subscribe(h =>  
        {
            float fill = (float)h / vm.MaxHealth;
            healthImage.fillAmount = fill;
            healthImage.color = Color.Lerp(Color.red, Color.green, fill);
        }).AddTo(disposables);

        //���´ϸ� ��
        viewModel.Stamina.Subscribe(s => 
        staminaImage.fillAmount = (float)s / vm.MaxStamina).AddTo(disposables);

        // ����ġ �� (Level Up ���)
        viewModel.Exp.Subscribe(e =>
        {
            int expToNext = vm.Level.Value * 100;
            expImage.fillAmount = Mathf.Clamp01((float)e / expToNext);
        }).AddTo(disposables);

        // �⺻ ����
        BindText(viewModel.Attack, attackText);
        BindText(viewModel.Defense, defenseText);
        BindText(viewModel.CriticalChance, critChanceText, "{0:F1}% ", 100f);
        BindText(viewModel.CriticalDamage, critDamageText, "{0:F1}x");
        BindText(viewModel.Penetration, penetrationText);
        BindText(viewModel.StaminaRegen, staminaRegenText, "{0:F1}/sec");
        BindText(viewModel.Health, healthText);

        // ������ ���ϸ��̼�
        viewModel.Level.Subscribe(l =>
        {
            levelText.text = $"Lv {l}";
            levelUpAnimator?.SetTrigger("LevelUp");
        }).AddTo(disposables);

        // ������ �̺�Ʈ �߰� ����
        viewModel.OnLevelUp.Subscribe(l =>
        {
            // ����, ����Ʈ �� ó�� ����
            Debug.Log($"Level Up! New Level: {l}");
        }).AddTo(disposables);
    }

    // Helper: ReactiveProperty�� TMP_Text�� ���ε�
    private void BindText<T>(IReadOnlyReactiveProperty<T> rp, TMP_Text text, string format = "{0}", float multiplier = 1f)
    {
        rp.Subscribe(v =>
        {
            if (v is int)  // int�� �Ҽ��� ����
            {
                text.text = string.Format("{0:0}", (int)(Convert.ToSingle(v) * multiplier));
            }
            else  // float�̸� format ����
            {
                float value = Convert.ToSingle(v) * multiplier;
                text.text = string.Format(format, value);
            }
        }).AddTo(disposables);
    }


    void OnDestroy() => disposables.Dispose();
}
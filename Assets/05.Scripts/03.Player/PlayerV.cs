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

        // 체력 바, 색상 변화 (녹색 -> 빨강)
        viewModel.Health.Subscribe(h =>  
        {
            float fill = (float)h / vm.MaxHealth;
            healthImage.fillAmount = fill;
            healthImage.color = Color.Lerp(Color.red, Color.green, fill);
        }).AddTo(disposables);

        //스태니마 바
        viewModel.Stamina.Subscribe(s => 
        staminaImage.fillAmount = (float)s / vm.MaxStamina).AddTo(disposables);

        // 경험치 바 (Level Up 대비)
        viewModel.Exp.Subscribe(e =>
        {
            int expToNext = vm.Level.Value * 100;
            expImage.fillAmount = Mathf.Clamp01((float)e / expToNext);
        }).AddTo(disposables);

        // 기본 스탯
        BindText(viewModel.Attack, attackText);
        BindText(viewModel.Defense, defenseText);
        BindText(viewModel.CriticalChance, critChanceText, "{0:F1}% ", 100f);
        BindText(viewModel.CriticalDamage, critDamageText, "{0:F1}x");
        BindText(viewModel.Penetration, penetrationText);
        BindText(viewModel.StaminaRegen, staminaRegenText, "{0:F1}/sec");
        BindText(viewModel.Health, healthText);

        // 레벨업 에니메이션
        viewModel.Level.Subscribe(l =>
        {
            levelText.text = $"Lv {l}";
            levelUpAnimator?.SetTrigger("LevelUp");
        }).AddTo(disposables);

        // 레벨업 이벤트 추가 구독
        viewModel.OnLevelUp.Subscribe(l =>
        {
            // 사운드, 이펙트 등 처리 가능
            Debug.Log($"Level Up! New Level: {l}");
        }).AddTo(disposables);
    }

    // Helper: ReactiveProperty를 TMP_Text에 바인딩
    private void BindText<T>(IReadOnlyReactiveProperty<T> rp, TMP_Text text, string format = "{0}", float multiplier = 1f)
    {
        rp.Subscribe(v =>
        {
            if (v is int)  // int면 소수점 없이
            {
                text.text = string.Format("{0:0}", (int)(Convert.ToSingle(v) * multiplier));
            }
            else  // float이면 format 적용
            {
                float value = Convert.ToSingle(v) * multiplier;
                text.text = string.Format(format, value);
            }
        }).AddTo(disposables);
    }


    void OnDestroy() => disposables.Dispose();
}
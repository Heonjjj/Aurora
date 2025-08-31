using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerVM : IDisposable
{
    private CompositeDisposable disposables = new CompositeDisposable();
    private readonly PlayerM model;
    public InventoryVM InventoryVM { get; }

    public ReadOnlyReactiveProperty<int> Health { get; } //클래스안에 수정가능한 Model필드값이 존재
    public ReadOnlyReactiveProperty<int> Stamina { get; }
    public ReadOnlyReactiveProperty<int> Attack { get; }
    public ReadOnlyReactiveProperty<int> Defense { get; }
    public ReadOnlyReactiveProperty<int> Level { get; }
    public ReadOnlyReactiveProperty<int> Exp { get; }
    public ReadOnlyReactiveProperty<float> CriticalChance { get; }
    public ReadOnlyReactiveProperty<float> CriticalDamage { get; }
    public ReadOnlyReactiveProperty<int> Penetration { get; }
    public ReadOnlyReactiveProperty<float> StaminaRegen { get; }
    public int MaxHealth => model.MaxHealth; //M쪽이 단순값이라 그냥 참조
    public int MaxStamina => model.MaxStamina;

    public IObservable<int> OnLevelUp { get; }

    public PlayerVM(PlayerM model) //동기화작업, 프로퍼티감싸기
    {
        this.model = model ?? throw new ArgumentNullException(nameof(model));

        // 모델의 ReactiveProperty를 그대로 구독하여 ReadOnlyReactiveProperty 생성
        Health = model.Health.ToReadOnlyReactiveProperty().AddTo(disposables);
        Stamina = model.Stamina.ToReadOnlyReactiveProperty().AddTo(disposables);
        Attack = model.Attack.ToReadOnlyReactiveProperty().AddTo(disposables);
        Defense = model.Defense.ToReadOnlyReactiveProperty().AddTo(disposables);
        Level = model.Level.ToReadOnlyReactiveProperty().AddTo(disposables);
        Exp = model.Exp.ToReadOnlyReactiveProperty().AddTo(disposables);

        // 모델의 일반 값들을 Observable로 감싸서 ReadOnlyReactiveProperty로 변환
        // 옵저버 = 단일 이벤트 스트림
        CriticalChance = Observable.Return(model.CriticalChance).ToReadOnlyReactiveProperty().AddTo(disposables);
        CriticalDamage = Observable.Return(model.CriticalDamage).ToReadOnlyReactiveProperty().AddTo(disposables);
        Penetration = Observable.Return(model.Penetration).ToReadOnlyReactiveProperty().AddTo(disposables);
        StaminaRegen = Observable.Return(model.StaminaRegenRate).ToReadOnlyReactiveProperty().AddTo(disposables);

        var levelUpSubject = new Subject<int>();
        model.OnLevelUp += level => levelUpSubject.OnNext(level);
        OnLevelUp = levelUpSubject.AsObservable();

        StartStaminaRecovery();
    }

    public void TakeDamage(int amount) => model.TakeDamage(amount);

    public void Heal(int amount) => model.Heal(amount);

    public void ConsumeStamina(int amount) => model.UseStamina(amount);

    private void StartStaminaRecovery()
    {
        Observable.EveryUpdate() // 매프레임 호출
                    .Where(_ => model.Stamina.Value < model.MaxStamina) //이검사는 호출최소화
                    .Subscribe(_ => model.RecoverStamina())
                    .AddTo(disposables);
    }

    public void Dispose()
    {
        disposables.Dispose();
        InventoryVM?.Dispose();
    }
}
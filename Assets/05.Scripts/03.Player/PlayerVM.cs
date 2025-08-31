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

    public ReadOnlyReactiveProperty<int> Health { get; } //Ŭ�����ȿ� ���������� Model�ʵ尪�� ����
    public ReadOnlyReactiveProperty<int> Stamina { get; }
    public ReadOnlyReactiveProperty<int> Attack { get; }
    public ReadOnlyReactiveProperty<int> Defense { get; }
    public ReadOnlyReactiveProperty<int> Level { get; }
    public ReadOnlyReactiveProperty<int> Exp { get; }
    public ReadOnlyReactiveProperty<float> CriticalChance { get; }
    public ReadOnlyReactiveProperty<float> CriticalDamage { get; }
    public ReadOnlyReactiveProperty<int> Penetration { get; }
    public ReadOnlyReactiveProperty<float> StaminaRegen { get; }
    public int MaxHealth => model.MaxHealth; //M���� �ܼ����̶� �׳� ����
    public int MaxStamina => model.MaxStamina;

    public IObservable<int> OnLevelUp { get; }

    public PlayerVM(PlayerM model) //����ȭ�۾�, ������Ƽ���α�
    {
        this.model = model ?? throw new ArgumentNullException(nameof(model));

        // ���� ReactiveProperty�� �״�� �����Ͽ� ReadOnlyReactiveProperty ����
        Health = model.Health.ToReadOnlyReactiveProperty().AddTo(disposables);
        Stamina = model.Stamina.ToReadOnlyReactiveProperty().AddTo(disposables);
        Attack = model.Attack.ToReadOnlyReactiveProperty().AddTo(disposables);
        Defense = model.Defense.ToReadOnlyReactiveProperty().AddTo(disposables);
        Level = model.Level.ToReadOnlyReactiveProperty().AddTo(disposables);
        Exp = model.Exp.ToReadOnlyReactiveProperty().AddTo(disposables);

        // ���� �Ϲ� ������ Observable�� ���μ� ReadOnlyReactiveProperty�� ��ȯ
        // ������ = ���� �̺�Ʈ ��Ʈ��
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
        Observable.EveryUpdate() // �������� ȣ��
                    .Where(_ => model.Stamina.Value < model.MaxStamina) //�̰˻�� ȣ���ּ�ȭ
                    .Subscribe(_ => model.RecoverStamina())
                    .AddTo(disposables);
    }

    public void Dispose()
    {
        disposables.Dispose();
        InventoryVM?.Dispose();
    }
}
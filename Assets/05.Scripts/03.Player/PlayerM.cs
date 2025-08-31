using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Random = System.Random;
using Time = UnityEngine.Time;

public interface IPlayer //�����ϴ��̵��� ���α��� ���󵵵�, �׽�Ʈ�Ǵ� ���͵� ���밡��
{
    void Heal(int amount);
    void GainExp(int amount);
    void AddStats(int atk, int def, int hp, int stamina);
    void RemoveStats(int atk, int def, int hp, int stamina);
}

public sealed class PlayerM : IPlayer
{
    public ReactiveProperty<int> Health { get; } //���Ѱ���ü�� �̺�Ʈ��Ȱ��
    public ReactiveProperty<int> Stamina { get; }
    public ReactiveProperty<int> Attack { get; }
    public ReactiveProperty<int> Defense { get; }
    public ReactiveProperty<int> Level { get; }
    public ReactiveProperty<int> Exp { get; }
    public int MaxHealth { get; private set; } //�ܼ� ��(�ڵ�������Ƽ, ��ŷ�ʵ� ��������)
    public int MaxStamina { get; private set; }
    public int ShieldBreak {  get; private set; }
    public float CriticalChance { get; private set; } // 0 ~ 1 (30% = 0.3f)
    public float CriticalDamage { get; private set; } // �⺻ 1.5�� = 1.5f
    public int AbilityATK { get; private set; }
    public int AbilityDEF { get; private set; }
    public int Penetration { get; private set; } // ��� ���� ��ġ 
    public float StaminaRegenRate { get; private set; } // �ʴ� ȸ����

    public event Action<int> OnLevelUp;

    //�Ű������ι޾ƾ� ���ָ��� �ٸ� �ʱⰪ�� �ټ�����
    public PlayerM(
        int maxHealth = 100, int maxStamina = 100, int attack = 10, int defense = 5, int shieldBreak = 10,
        float critChance = 0.1f, float critDamage = 1.5f, int abilityATK = 10, int abilityDEF = 10,
        int penetration = 0, float staminaRegen = 5f)
    {
        Health = new ReactiveProperty<int>(maxHealth);
        Stamina = new ReactiveProperty<int>(maxStamina);
        Attack = new ReactiveProperty<int>(attack);
        Defense = new ReactiveProperty<int>(defense);
        Level = new ReactiveProperty<int>(1);
        Exp = new ReactiveProperty<int>(0);
        MaxHealth = maxHealth;
        MaxStamina = maxStamina;
        ShieldBreak = shieldBreak;
        CriticalChance = critChance; 
        CriticalDamage = critDamage; 
        AbilityATK = abilityATK;
        AbilityDEF = abilityDEF;
        Penetration = penetration; 
        StaminaRegenRate = staminaRegen; 

        Exp.Subscribe(_ => CheckLevelUp());
    }
    private void CheckLevelUp()
    {
        int expForNextLevel = Level.Value * 100; // ���� ����
        while (Exp.Value >= expForNextLevel)
        {
            Exp.Value -= expForNextLevel;
            Level.Value++;
            MaxHealth += 10;
            MaxStamina += 20;
            Attack.Value += 2;
            Defense.Value += 1;
            CriticalChance += 0.01f;  // ���������� 1% ġȮ ����
            CriticalDamage += 0.05f;  // ġ��Ÿ ���� 5% ����
            Penetration += 1;         // ����� +1
            StaminaRegenRate += 0.5f;     // ��� ȸ�� +0.5/sec
            Health.Value = MaxHealth;   // ������ �� ü�� ȸ��
            Stamina.Value = MaxStamina; // ������ �� ���¹̳� ȸ��
            expForNextLevel = Level.Value * 100;
            OnLevelUp?.Invoke(Level.Value);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        Health.Value = Math.Min(MaxHealth, Health.Value + amount);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        Health.Value = Math.Max(0, Health.Value - amount);
    }

    public void UseStamina(int amount)
    {
        if (amount <= 0) return;
        Stamina.Value = Math.Max(0, Stamina.Value - amount);
    }

    public bool CanUseStamina(int amount) => Stamina.Value >= amount;



    private float staminaAccum;
    public void RecoverStamina()
    {
        staminaAccum += StaminaRegenRate * Time.deltaTime;
        int recover = (int)Math.Floor(staminaAccum);
        if (recover > 0)
        {
            Stamina.Value = Math.Min(MaxStamina, Stamina.Value + recover);
            staminaAccum -= recover;
        }
    }

    // ���� ��� (ġ��Ÿ, ����� �ݿ�)
    private Random rng = new Random();
    public struct DamageResult
    {
        public int Damage;
        public bool IsCritical;
    }

    public DamageResult CalculateDamage(int targetDefense)
    {
        bool isCrit = rng.NextDouble() < CriticalChance;
        float damage = Attack.Value - Mathf.Max(0, targetDefense - Penetration);
        if (damage < 1) damage = 1; //�ּҵ�����
        if (isCrit) damage *= CriticalDamage;

        return new DamageResult
        {
            Damage = (int)Math.Round(damage),
            IsCritical = isCrit
        };
    }
    // ���ݹ�����
    public void ReceiveAttack(PlayerM attacker)
    {
        var result = attacker.CalculateDamage(Defense.Value);
        TakeDamage(result.Damage);
        // �ʿ� �� UI������ IsCritical ��� ����
    }

    // --- ����ġ ȹ�� ---
    public void GainExp(int amount)
    {
        if (amount <= 0) return;
        Exp.Value += amount;
    }

    public void AddStats(int atk, int def, int hp, int stamina)
    {
        Attack.Value += atk;
        Defense.Value += def;
        MaxHealth += hp;
        MaxStamina += stamina;
    }
    public void RemoveStats(int atk, int def, int hp, int stamina)
    {
        Attack.Value -= atk;
        Defense.Value -= def;
        MaxHealth -= hp;
        MaxStamina -= stamina;
        Health.Value = Math.Min(Health.Value, MaxHealth);
        Stamina.Value = Math.Min(Stamina.Value, MaxStamina);
    }
}
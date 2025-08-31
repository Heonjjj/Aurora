using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using static UnityEditor.Progress;


// ������ ���/���� ��Ģ
public interface IActionRule
{
    bool CanUse(IPlayer player, ItemData item);
    void OnUse(IPlayer player, ItemData item);

    bool CanEquip(IPlayer player, ItemData item);
    void OnEquip(IPlayer player, ItemData item);

    bool CanUnEquip(IPlayer player, ItemData item);
    void OnUnEquip(IPlayer player, ItemData item);
}

// ������ �̵�/�ŷ� ��Ģ
public interface ITransferRule
{
    bool CanTransfer(InventoryM from, InventoryM to, int index, int amount);
    void OnTransfer(InventoryM from, InventoryM to, int index, int amount);
}

//����Ʈ�� ���⿡������, �����Ұ� > �ŷ�,����
// ���� ��Ģ (ȿ�� ����) / �̱���
public class ShopActionRule : IActionRule
{
    public bool CanUse(IPlayer player, ItemData item) => false;
    public void OnUse(IPlayer player, ItemData item) { }

    public bool CanEquip(IPlayer player, ItemData item) => false;
    public void OnEquip(IPlayer player, ItemData item) { }

    public bool CanUnEquip(IPlayer player, ItemData item) => false;
    public void OnUnEquip(IPlayer player, ItemData item) { }
}

// �÷��̾� ��Ģ (ȿ���� ���)
public class InvenActionRule : IActionRule
{
    public bool CanUse(IPlayer player, ItemData item) => item.itemType == ItemType.Consumable;

    public void OnUse(IPlayer player, ItemData item)
    {
        if (item is ConsumableData c)
        {
            player.Heal(c.healAmount);
            player.GainExp(c.expAmount);
            Debug.Log($"Used {item.itemName}, healed {c.healAmount} HP!");
        }
    }

    public bool CanEquip(IPlayer player, ItemData item) => item.itemType == ItemType.Equipment;

    public void OnEquip(IPlayer player, ItemData item)
    {
        if (item is EquipmentData eq)
            player.AddStats(eq.attackBonus, eq.defenseBonus, eq.maxHealthBonus, eq.maxStaminaBonus);
    }

    public bool CanUnEquip(IPlayer player, ItemData item) => item.itemType == ItemType.Equipment;

    public void OnUnEquip(IPlayer player, ItemData item)
    {
        if (item is EquipmentData eq)
            player.RemoveStats(eq.attackBonus, eq.defenseBonus, eq.maxHealthBonus, eq.maxStaminaBonus);
    }
}

public class EventMediator : IDisposable
{
    private CompositeDisposable _disposables = new CompositeDisposable();
    private readonly IInventory _inventory;
    private readonly IPlayer _player;       // <- PlayerM ��� �������̽�
    private readonly IActionRule _actionRule;

    public EventMediator(IInventory inventory, IPlayer player, IActionRule actionRule)
    {
        _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _actionRule = actionRule ?? throw new ArgumentNullException(nameof(actionRule));

        ItemObject.OnItemPickedGlobal
                  .Subscribe(item => _inventory.AddItem(item, 1))
                  .AddTo(_disposables);
    }

    public bool UseItem(int index) => _inventory.TryUseItem(index, _player, _actionRule);
    public bool EquipItem(int index) => _inventory.TryEquipSlot(index, _player, _actionRule);
    public bool UnEquipItem(int index) => _inventory.TryUnEquipSlot(index, _player, _actionRule);

    public void Dispose() => _disposables.Dispose();
}
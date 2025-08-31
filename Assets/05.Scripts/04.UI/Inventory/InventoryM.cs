using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using static UnityEditor.Progress;

public sealed class ItemSlotM //����������� ����, ���â���� ��Ȱ�밡��
{
    // ���ο����� ���� ������ ReactiveProperty
    [JsonIgnore] private ReactiveProperty<ItemData> _item;
    [JsonIgnore] private ReactiveProperty<int> _quantity;
    [JsonIgnore] private ReactiveProperty<bool> _equipped;

    // �ܺο� ������ ReadOnlyReactiveProperty
    public IReadOnlyReactiveProperty<ItemData> Item => _item;
    public IReadOnlyReactiveProperty<int> Quantity => _quantity;
    public IReadOnlyReactiveProperty<bool> Equipped => _equipped;
    public IReadOnlyReactiveProperty<bool> IsEmpty { get; }

    public ItemSlotM(ItemData item = null, int quantity = 0, bool equipped = false)
    {
        _item = new ReactiveProperty<ItemData>(item);
        _quantity = new ReactiveProperty<int>(quantity);
        _equipped = new ReactiveProperty<bool>(equipped);

        IsEmpty = _item.Select(i => i == null).ToReadOnlyReactiveProperty();
        //Reactive LINQ, Null���� ����ϴ� Observable => Item�� Null�̸� true ��ȯ
    }

    // ���� ���������� �� ���� ����
    public void SetItem(ItemData item, int quantity = 1, bool equipped = false)
    {
        _item.Value = item;
        _quantity.Value = quantity;
        _equipped.Value = equipped;
    }

    public void Clear()
    {
        _item.Value = null;
        _quantity.Value = 0;
        _equipped.Value = false;
    }

    public void ChangeQuantity(int amount)
    {
        _quantity.Value = Mathf.Max(0, _quantity.Value + amount);
        if (_quantity.Value == 0) Clear();
    }

    public void Equip() => _equipped.Value = true;
    public void UnEquip() => _equipped.Value = false;
}

public interface IInventory
{
    IReadOnlyList<ItemSlotM> Slots { get; }
    bool TryUseItem(int index, IPlayer player, IActionRule actionRule);
    bool TryEquipSlot(int index, IPlayer player, IActionRule actionRule);
    bool TryUnEquipSlot(int index, IPlayer player, IActionRule actionRule);
    void AddItem(ItemData item, int amount);
    void RemoveItem(int index, int amount);
}

public sealed class InventoryM : IInventory
{
    private List<ItemSlotM> _slots;
    public IReadOnlyList<ItemSlotM> Slots => _slots;
    public int MaxSlots { get; }

    public InventoryM(int maxSlots)
    {
        MaxSlots = maxSlots;
        _slots = new List<ItemSlotM>(maxSlots);
        for (int i = 0; i < maxSlots; i++)
            _slots.Add(new ItemSlotM());
    }

    public void AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return;

        // ���� ������ ���� ã��
        if (item.stackable)
        {
            var slot = _slots.FirstOrDefault(s => s.Item.Value == item && s.Quantity.Value < item.maxStack);
            if (slot != null)
            {
                slot.ChangeQuantity(Mathf.Min(amount, item.maxStack - slot.Quantity.Value));
                return;
            }
        }

        // �� ���� ã�Ƽ� �� ������ �ֱ�
        var emptySlot = _slots.FirstOrDefault(s => s.IsEmpty.Value);
        if (emptySlot != null)
        {
            emptySlot.SetItem(item, Mathf.Min(amount, item.maxStack));
        }
    }

    public void RemoveItem(int index, int amount)
    {
        if (index < 0 || index >= _slots.Count || amount <= 0) return;
        _slots[index].ChangeQuantity(-amount);
    }


    // ���� ��û ó�� (Equip/UnEquip)
    public bool TryEquipSlot(int index, IPlayer player, IActionRule rule)
    {
        if (index < 0 || index >= _slots.Count) return false;

        var slot = _slots[index];
        var item = slot.Item.Value;
        if (item == null) return false;

        if (!rule.CanEquip(player, item)) return false;

        // ���� ���� ���� ����
        if (item is EquipmentData eq)
        {
            foreach (var s in _slots)
            {
                if (s.Equipped.Value && s.Item.Value is EquipmentData otherEq &&
                    otherEq.equipSlot == eq.equipSlot)
                {
                    s.UnEquip();
                    rule.OnUnEquip(player, otherEq);
                }
            }
        }

        slot.Equip();
        rule.OnEquip(player, item);
        return true;
    }

    public bool TryUnEquipSlot(int index, IPlayer player, IActionRule rule)
    {
        if (index < 0 || index >= _slots.Count) return false;

        var slot = _slots[index];
        var item = slot.Item.Value;
        if (item == null) return false;

        if (!rule.CanUnEquip(player, item)) return false;

        slot.UnEquip();
        rule.OnUnEquip(player, item);
        return true;
    }

    public bool TryUseItem(int index, IPlayer player, IActionRule rule)
    {
        if (index < 0 || index >= _slots.Count) return false;

        var slot = _slots[index];
        var item = slot.Item.Value;
        if (item == null) return false;

        if (!rule.CanUse(player, item)) return false;

        rule.OnUse(player, item);
        slot.ChangeQuantity(-1); // ��� �� ���� ����
        return true;
    }
}
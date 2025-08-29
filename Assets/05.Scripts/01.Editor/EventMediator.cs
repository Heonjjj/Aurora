using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMediator
{
    private readonly InventoryM inventory;

    public EventMediator(InventoryM inventory)
    {
        this.inventory = inventory;

        inventory.OnUseRequested += HandleUse;
        inventory.OnEquipRequested += HandleEquip;
        inventory.OnUnEquipRequested += HandleUnEquip;
    }

    private void HandleUse(int index, PlayerM player)
    {
        var slot = inventory.Slots[index];
        if (slot.Item.Value is IUsable usable)
        {
            usable.Use(player);       // ConsumableData.OnUse ȣ��
            slot.ChangeQuantity(-1);  // ��� �� ���� ����
        }
    }

    private void HandleEquip(int index, PlayerM player)
    {
        var slot = inventory.Slots[index];
        if (slot.Item.Value is IEquipable equipable)
        {
            // ���� ����Ÿ�� ���� ����
            foreach (var s in inventory.Slots)
            {
                if (s.Equipped.Value && s.Item.Value is IEquipable eq && eq.SlotType == equipable.SlotType)
                {
                    s.UnEquip();
                    eq.OnUnequip(player);
                }
            }

            slot.Equip();
            equipable.OnEquip(player);
        }
    }

    private void HandleUnEquip(int index, PlayerM player)
    {
        var slot = inventory.Slots[index];
        if (slot.Item.Value is IEquipable equipable)
        {
            slot.UnEquip();
            equipable.OnUnequip(player);
        }
    }
}

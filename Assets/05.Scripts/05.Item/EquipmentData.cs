using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Item/Equipment")]
public class EquipmentData : ItemData, IEquipable
{
    [Header("Stats")]
    public EquipableType equipSlot;
    public int attackBonus;
    public int defenseBonus;
    public int healthBonus;
    public int staminaBonus;
    public float critChanceBonus;
    public float critDamageBonus;
    public float staminaRegenBonus;

    public EquipableType SlotType => equipSlot;

    public void OnEquip(PlayerM player) =>
            player.AddStats(attackBonus, defenseBonus, healthBonus, staminaBonus);

    public void OnUnequip(PlayerM player) =>
        player.RemoveStats(attackBonus, defenseBonus, healthBonus, staminaBonus);
}

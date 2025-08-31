using UnityEngine;

public enum EquipableType
{
    Weapon,
    Necklace,
    Ring,
    Helmet,
    Armor,
    Pants,
}

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Item/Equipment")]
public class EquipmentData : ItemData
{
    [Header("Equipment Stats")]
    public EquipableType equipSlot;
    public int attackBonus;
    public int defenseBonus;
    public int maxHealthBonus;
    public int maxStaminaBonus;
    public int shieldBreakBonus;
    public float critChanceBonus;
    public float critDamageBonus;
    public int abilityATKBounus;
    public int abilityDEFBounus;
    public int penetrationBounus;
    public float staminaRegenBonus;

    private void OnEnable() => itemType = ItemType.Equipment; // 생성 시 자동 설정
}
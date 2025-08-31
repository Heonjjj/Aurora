using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Item/Consumable")]
public class ConsumableData : ItemData
{
    [Header("Consumable Stats")]
    public int healAmount;
    public int expAmount;
    public float duration; // ���� ���ӽð�

    private void OnEnable() => itemType = ItemType.Consumable;
}
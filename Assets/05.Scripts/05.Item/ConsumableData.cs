using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Item/Consumable")]
public class ConsumableData : ItemData, IUsable
{
    [Header("Heal")]
    public int healAmount;
    public float duration; // ȿ�� ���ӽð�

    [Header("Buff")]
    public int expAmount;

    public void Use(PlayerM player)
    {
        player.Heal(healAmount);
        player.GainExp(expAmount);
        Debug.Log($"Used {name}, healed {healAmount} HP!");
    }
    public override void OnUse(PlayerM player) => Use(player);
}

using UnityEngine;

public enum EquipableType
{
    Helmet,
    Armor,
    Pants,
    Weapon,
    Necklace,
    Ring
}
public interface IUsable
{
    void Use(PlayerM player); // 사용 시 호출
}

public interface IEquipable
{
    EquipableType SlotType { get; }
    void OnEquip(PlayerM player);
    void OnUnequip(PlayerM player);
}

public abstract class ItemData : ScriptableObject
{
    [Header("Info")]
    public string name;
    public string description;
    public Sprite icon;
    public GameObject itemPrefab;
    public bool stackable;
    public int maxStack = 99;

    public virtual void OnUse(PlayerM player) { } // 기본 구현
}

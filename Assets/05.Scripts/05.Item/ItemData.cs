using UnityEngine;

public enum ItemType
{
    None,
    Consumable,
    Equipment
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Generic")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public string description;
    public Sprite icon;
    public bool stackable;
    public int maxStack = 99;
    //public GameObject itemPrefab; //item����� ���� ����


    [Header("Type")]
    public ItemType itemType = ItemType.None;
}
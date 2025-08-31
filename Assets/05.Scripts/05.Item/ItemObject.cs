using System;
using TMPro;
using UnityEngine;
using UniRx;

public interface IInteractable //Ȯ�尡�� ��,����,NPC ��
{
    public string GetInteractPrompt();
    public void OnInteract();
}
public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public static Subject<ItemData> OnItemPickedGlobal = new Subject<ItemData>(); // ���� �̺�Ʈ

    public string GetInteractPrompt()
            => data == null ? "" : $"{data.itemName}\n{data.description}";

    public void OnInteract()
    {
        if (data == null) return;
        OnItemPickedGlobal.OnNext(data); //Ȯ�尡�ɼ� �����, ����, ����Ʈ ��
        Destroy(gameObject);
    }
}
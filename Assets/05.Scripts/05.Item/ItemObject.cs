using System;
using TMPro;
using UnityEngine;
using UniRx;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}
public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public static Subject<ItemData> OnItemPickedGlobal = new Subject<ItemData>(); // ���� �̺�Ʈ

    public string GetInteractPrompt()
            => data == null ? "" : $"{data.name}\n{data.description}";

    public void OnInteract()
    {
        if (data == null) return;
        OnItemPickedGlobal.OnNext(data); //Ȯ�尡�ɼ� �����, ����, ����Ʈ ��
        Destroy(gameObject);
    }
}
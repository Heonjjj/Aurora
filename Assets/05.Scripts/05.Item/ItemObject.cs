using System;
using TMPro;
using UnityEngine;
using UniRx;

public interface IInteractable //확장가능 문,상자,NPC 등
{
    public string GetInteractPrompt();
    public void OnInteract();
}
public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public static Subject<ItemData> OnItemPickedGlobal = new Subject<ItemData>(); // 전역 이벤트

    public string GetInteractPrompt()
            => data == null ? "" : $"{data.itemName}\n{data.description}";

    public void OnInteract()
    {
        if (data == null) return;
        OnItemPickedGlobal.OnNext(data); //확장가능성 열어둠, 사운드, 퀘스트 등
        Destroy(gameObject);
    }
}
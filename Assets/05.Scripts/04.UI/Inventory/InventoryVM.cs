using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class ItemSlotVM : IDisposable
{
    private CompositeDisposable disposables = new CompositeDisposable();
    public ItemSlotM Slot { get; }
    public ReadOnlyReactiveProperty<string> LabelText { get; }
    public ReadOnlyReactiveProperty<Sprite> Icon { get; }

    public ItemSlotVM(ItemSlotM slot)
    {
        Slot = slot;

        LabelText = Observable.CombineLatest(
            Slot.Item, Slot.Quantity, Slot.Equipped,
            (item, qty, eq) =>
            {
                if (item == null) return "";   // 빈 슬롯이면 빈 문자열

                // ItemData 안의 displayName 사용
                return $"{item.name} x{qty}" + (eq ? " [E]" : "");
            })
            .ToReadOnlyReactiveProperty()
            .AddTo(disposables);


        // 아이콘도 ItemData에서 바로 가져오기
        Icon = Slot.Item
           .Select(item => item?.icon)
           .ToReadOnlyReactiveProperty()
           .AddTo(disposables);   
    }   

    public void Dispose() => disposables.Dispose();
}

public class InventoryVM : IDisposable
{
    private CompositeDisposable disposables = new CompositeDisposable();
    public List<ItemSlotVM> Slots { get; }
    public InventoryM Model { get; }  // InventoryM 직접 참조
    public ReactiveProperty<int?> SelectedIndex { get; }

    public InventoryVM(InventoryM model)
    {
        Model = model;
        Slots = model.Slots.Select(s => new ItemSlotVM(s)).ToList();
        SelectedIndex = new ReactiveProperty<int?>(null).AddTo(disposables);

        // 전역 아이템 픽업 이벤트 구독
        ItemObject.OnItemPickedGlobal
            .Subscribe(item => model.AddItem(item, 1))
            .AddTo(disposables);
    }
    public void Dispose()
    {
        foreach (var s in Slots) s.Dispose();
        SelectedIndex.Dispose();
        disposables.Dispose();
    }
}
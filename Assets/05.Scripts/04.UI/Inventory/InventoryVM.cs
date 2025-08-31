using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class ItemSlotVM : IDisposable
{
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
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
                if (item == null) return "";
                return $"{item.itemName} x{qty}" + (eq ? " [E]" : "");
            }).ToReadOnlyReactiveProperty().AddTo(_disposables);

        Icon = Slot.Item.Select(i => i?.icon).ToReadOnlyReactiveProperty().AddTo(_disposables);
    }

    public void Dispose() => _disposables.Dispose();
}

public class InventoryVM : IDisposable
{
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    public List<ItemSlotVM> Slots { get; }
    public InventoryM Model { get; }
    public ReactiveProperty<int?> SelectedIndex { get; }

    public InventoryVM(InventoryM model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        if (Model.Slots == null) throw new InvalidOperationException("InventoryVM: Slots is null");

        Slots = model.Slots.Select(s => new ItemSlotVM(s)).ToList();
        SelectedIndex = new ReactiveProperty<int?>(null).AddTo(_disposables);
    }

    public void Dispose()
    {
        foreach (var slotVM in Slots) slotVM.Dispose();
        SelectedIndex.Dispose();
        _disposables.Dispose();
    }
}
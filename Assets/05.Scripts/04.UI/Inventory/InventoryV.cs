using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class InventoryV : MonoBehaviour
{
    public Transform slotPanel;
    public GameObject slotPrefab;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    private InventoryVM viewModel;
    private EventMediator manager;
    private ItemSlotV[] slotViews;
    private CompositeDisposable disposables = new CompositeDisposable();

    private IPlayer player; // 현재 인벤토리 주인

    public void Init(InventoryVM vm, EventMediator mgr, IPlayer player)
    {
        viewModel = vm;
        manager = mgr;
        this.player = player;

        slotViews = slotPanel.GetComponentsInChildren<ItemSlotV>();
        int count = Mathf.Min(slotViews.Length, viewModel.Slots.Count);

        for (int i = 0; i < count; i++)
        {
            int idx = i; // closure 방지
            slotViews[i].Init(viewModel.Slots[i], idx, OnSelectItem);
        }

        if (count > 0) OnSelectItem(0);

        // 버튼 클릭 바인딩
        useButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (viewModel.SelectedIndex.Value.HasValue)
                manager.UseItem(viewModel.SelectedIndex.Value.Value);
        });

        equipButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (viewModel.SelectedIndex.Value.HasValue)
                manager.EquipItem(viewModel.SelectedIndex.Value.Value);
        });

        unequipButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (viewModel.SelectedIndex.Value.HasValue)
                manager.UnEquipItem(viewModel.SelectedIndex.Value.Value);
        });

        dropButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (viewModel.SelectedIndex.Value.HasValue)
                viewModel.Model.RemoveItem(viewModel.SelectedIndex.Value.Value, 1);
        });
    }

    private void OnSelectItem(int index)
    {
        viewModel.SelectedIndex.Value = index;
        UpdateSelectedUI(index);

        for (int i = 0; i < slotViews.Length; i++)
            slotViews[i].SetSelected(i == index);
    }

    private void UpdateSelectedUI(int index)
    {
        var slot = viewModel.Slots[index].Slot;

        if (slot.IsEmpty.Value)
        {
            nameText.text = "";
            descriptionText.text = "";
            useButton.SetActive(false);
            equipButton.SetActive(false);
            unequipButton.SetActive(false);
            dropButton.SetActive(false);
            slotViews[index].SetImageActive(false); // 빈 슬롯 이미지 숨기기
            return;
        }

        var item = slot.Item.Value;
        nameText.text = item.itemName;
        descriptionText.text = $"Quantity: {slot.Quantity.Value}";

        equipButton.SetActive(!slot.Equipped.Value);
        unequipButton.SetActive(slot.Equipped.Value);
        dropButton.SetActive(true);

        slotViews[index].SetImageActive(true);
        slotViews[index].icon.sprite = item.icon;
    }


    private void OnDestroy()
    {
        useButton.GetComponent<Button>().onClick.RemoveAllListeners();
        equipButton.GetComponent<Button>().onClick.RemoveAllListeners();
        unequipButton.GetComponent<Button>().onClick.RemoveAllListeners();
        dropButton.GetComponent<Button>().onClick.RemoveAllListeners();

        disposables.Dispose();
        viewModel?.Dispose();
    }
}
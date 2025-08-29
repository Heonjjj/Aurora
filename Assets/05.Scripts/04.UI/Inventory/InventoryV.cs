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
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    public GameObject statusPanel;
    public GameObject invenPanel;
    public GameObject skillPanel;
    public Toggle statusToggle;
    public Toggle invenToggle;
    public Toggle skillToggle;

    private InventoryVM viewModel;
    private ItemSlotV[] slots;
    private CompositeDisposable disposables = new CompositeDisposable();

    private PlayerM player; // ���� �κ��丮 ����

    public void Init(InventoryVM vm, PlayerM player)
    {
        viewModel = vm;
        this.player = player;

        slots = slotPanel.GetComponentsInChildren<ItemSlotV>();

        for (int i = 0; i < slots.Length && i < viewModel.Slots.Count; i++)
        {
            int idx = i; // closure ���� ����
            slots[i].Init(viewModel.Slots[i], idx, OnSelectItem);
        }

        if (slots.Length > 0)
            OnSelectItem(0);

        // ��ư Ŭ�� ����
        useButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (viewModel.SelectedIndex.Value.HasValue)
                viewModel.Model.RequestUse(viewModel.SelectedIndex.Value.Value, player);
        });

        equipButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (viewModel.SelectedIndex.Value.HasValue)
                viewModel.Model.RequestEquip(viewModel.SelectedIndex.Value.Value, player);
        });

        unequipButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (viewModel.SelectedIndex.Value.HasValue)
                viewModel.Model.RequestUnEquip(viewModel.SelectedIndex.Value.Value, player);
        });

        dropButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (viewModel.SelectedIndex.Value.HasValue)
                viewModel.Model.RemoveItem(viewModel.SelectedIndex.Value.Value, 1);
        });

        // ó�� �����ϸ� ����â�� �ѱ�
        statusToggle.isOn = true;
    }

    private void OnSelectItem(int index)
    {
        viewModel.SelectedIndex.Value = index;
        UpdateSelectedUI(index);

        // ���� ���� ó��
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSelected(i == index); // ���� ���Ը� �˰�
        }
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
            slots[index].SetImageActive(false); // �� ���� �̹��� �����
            return;
        }

        var item = slot.Item.Value;
        nameText.text = item.name;
        descriptionText.text = $"Quantity: {slot.Quantity.Value}";

        equipButton.SetActive(!slot.Equipped.Value);
        unequipButton.SetActive(slot.Equipped.Value);
        dropButton.SetActive(true);

        slots[index].SetImageActive(true);
        slots[index].icon.sprite = item.icon;
    }


    private void OnDestroy() => disposables.Dispose();
}
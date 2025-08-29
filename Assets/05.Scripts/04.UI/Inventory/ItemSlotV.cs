using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotV : MonoBehaviour, IPointerEnterHandler
{
    public Image icon;
    public Image backgroundImage;
    public TMP_Text label;

    public int index;
    Color originalBackgroundColor;
    Color originalIconColor;

    private CompositeDisposable disposables = new CompositeDisposable();

    public void Init(ItemSlotVM vm, int idx, Action<int> onClick)
    {
        if (backgroundImage != null) originalBackgroundColor = backgroundImage.color;
        if (icon != null) originalIconColor = icon.color;

        index = idx; //Ŭ���� ���ư�� ���ȴ��� �ĺ���, �κ��� ���Թ�ȣ ���

        vm.LabelText.Subscribe(text => label.text = text).AddTo(disposables);
        vm.Icon.Subscribe(sprite =>
        {
            icon.sprite = sprite;
            icon.enabled = sprite != null;
        }).AddTo(disposables);

        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(() => onClick?.Invoke(index));
    }

    public void OnPointerEnter(PointerEventData eventData) //���콺�ö󰡸� �̺�Ʈ�ߵ�
    {
        //throw new NotImplementedException();
    }

    public void SetImageActive(bool active) => icon.enabled = active;

    public void SetSelected(bool selected)
    {
        if (icon != null)
        {
            icon.color = selected
                ? new Color(0f, 0f, 0f, icon.color.a)   // ���� �� RGB 0, ���� ����
                : originalIconColor;                   // ���� �� ���� ����
        }

        if (backgroundImage != null)
        {
            var c = backgroundImage.color;
            backgroundImage.color = selected
                ? new Color(c.r, c.g, c.b, 1f)        // ���� �� ���� 1
                : originalBackgroundColor;            // ���� �� ���� ����
        }
    }
    private void OnDestroy() => disposables.Dispose();
}
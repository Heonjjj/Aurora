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

        index = idx; //클릭시 어떤버튼이 눌렸는지 식별용, 인벤내 슬롯번호 기록

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

    public void OnPointerEnter(PointerEventData eventData) //마우스올라가면 이벤트발동
    {
        //throw new NotImplementedException();
    }

    public void SetImageActive(bool active) => icon.enabled = active;

    public void SetSelected(bool selected)
    {
        if (icon != null)
        {
            icon.color = selected
                ? new Color(0f, 0f, 0f, icon.color.a)   // 선택 시 RGB 0, 알파 유지
                : originalIconColor;                   // 비선택 시 원래 색상
        }

        if (backgroundImage != null)
        {
            var c = backgroundImage.color;
            backgroundImage.color = selected
                ? new Color(c.r, c.g, c.b, 1f)        // 선택 시 알파 1
                : originalBackgroundColor;            // 비선택 시 원래 색상
        }
    }
    private void OnDestroy() => disposables.Dispose();
}
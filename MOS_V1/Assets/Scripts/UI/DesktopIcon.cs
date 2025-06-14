using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TMPro;

public class DesktopIcon : MonoBehaviour, IPointerClickHandler
{
    public string targetUIName; // Ҫ�򿪵Ľ�����
    public Image iconImage;
    public TextMeshProUGUI iconText;

    public Action<DesktopIcon> OnOpen; // ���¼�����Desktop����

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;
    public static bool UseDoubleClick = true; // ���л���/˫��

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UseDoubleClick)
        {
            if (Time.time - lastClickTime < doubleClickThreshold)
            {
                OnOpen?.Invoke(this);
            }
            lastClickTime = Time.time;
        }
        else
        {
            OnOpen?.Invoke(this);
        }
    }

    public void SetData(string uiName, Sprite icon, string text)
    {
        targetUIName = uiName;
        if (iconImage) iconImage.sprite = icon;
        if (iconText) iconText.text = text;
    }
}
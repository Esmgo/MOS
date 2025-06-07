using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class UIButton : MonoBehaviour, IPointerClickHandler
{
    // 按钮点击事件（C# event）
    public event Action<BaseEventData> OnClick;

    protected virtual void Reset()
    {
        // 确保只用于UI物体（带有CanvasRenderer组件）
        if (GetComponent<CanvasRenderer>() == null)
        {
            Debug.LogWarning($"{nameof(UIButton)} 只能挂载在UI物体上！");
        }
    }

    // 注册点击事件
    public void RegisterClick(Action<BaseEventData> callback)
    {
        OnClick += callback;
    }

    // IPointerClickHandler接口实现
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(eventData);
    }
}

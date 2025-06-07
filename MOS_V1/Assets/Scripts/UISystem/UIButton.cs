using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class UIButton : MonoBehaviour, IPointerClickHandler
{
    // ��ť����¼���C# event��
    public event Action<BaseEventData> OnClick;

    protected virtual void Reset()
    {
        // ȷ��ֻ����UI���壨����CanvasRenderer�����
        if (GetComponent<CanvasRenderer>() == null)
        {
            Debug.LogWarning($"{nameof(UIButton)} ֻ�ܹ�����UI�����ϣ�");
        }
    }

    // ע�����¼�
    public void RegisterClick(Action<BaseEventData> callback)
    {
        OnClick += callback;
    }

    // IPointerClickHandler�ӿ�ʵ��
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(eventData);
    }
}

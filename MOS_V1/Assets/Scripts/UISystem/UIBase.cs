using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class UIBase : MonoBehaviour
{
    // UI事件数据
    public class UIEventData
    {
        public GameObject target;
        public EventTriggerType eventType;
        public UnityAction<BaseEventData> callback;
    }

    private List<UIEventData> registeredEvents = new List<UIEventData>();

    // 初始化
    public virtual void Initialize()
    {
        // 可以在这里做一些初始化工作
    }

    // 打开UI
    public virtual void OnOpen()
    {
        gameObject.SetActive(true);
    }

    // 关闭UI
    public virtual void OnClose()
    {
        gameObject.SetActive(false);
    }

    // 暂停UI（当其他UI打开时）
    public virtual void OnPause()
    {
        // 可以在这里做一些暂停逻辑，比如停止动画等
    }

    // 恢复UI（当其他UI关闭时）
    public virtual void OnResume()
    {
        // 可以在这里做一些恢复逻辑
    }

    // 隐藏UI
    public virtual void OnHide()
    {
        gameObject.SetActive(false);
    }

    // 显示UI
    public virtual void OnShow()
    {
        gameObject.SetActive(true);
    }

    // 销毁UI
    public virtual void OnDestroy()
    {
        // 清除所有注册的事件
        ClearAllEvents();
    }

    // 注册UI事件
    protected void RegisterEvent(GameObject target, EventTriggerType eventType, UnityAction<BaseEventData> callback)
    {
        if (target == null || callback == null) return;

        // 添加事件触发器
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        // 创建事件项
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(callback);

        // 添加到触发器
        trigger.triggers.Add(entry);

        // 保存事件数据以便清理
        registeredEvents.Add(new UIEventData
        {
            target = target,
            eventType = eventType,
            callback = callback
        });
    }

    // 清除所有注册的事件
    private void ClearAllEvents()
    {
        foreach (var eventData in registeredEvents)
        {
            if (eventData.target != null)
            {
                var trigger = eventData.target.GetComponent<EventTrigger>();
                if (trigger != null)
                {
                    // 找到对应的事件项并移除
                    for (int i = trigger.triggers.Count - 1; i >= 0; i--)
                    {
                        if (trigger.triggers[i].eventID == eventData.eventType)
                        {
                            trigger.triggers.RemoveAt(i);
                            break;
                        }
                    }

                    // 如果没有事件项了，移除EventTrigger组件
                    if (trigger.triggers.Count == 0)
                    {
                        Destroy(trigger);
                    }
                }
            }
        }

        registeredEvents.Clear();
    }
}
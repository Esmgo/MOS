using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class UIBase : MonoBehaviour
{
    // UI�¼�����
    public class UIEventData
    {
        public GameObject target;
        public EventTriggerType eventType;
        public UnityAction<BaseEventData> callback;
    }

    private List<UIEventData> registeredEvents = new List<UIEventData>();

    // ��ʼ��
    public virtual void Initialize()
    {
        // ������������һЩ��ʼ������
    }

    // ��UI
    public virtual void OnOpen()
    {
        gameObject.SetActive(true);
    }

    // �ر�UI
    public virtual void OnClose()
    {
        gameObject.SetActive(false);
    }

    // ��ͣUI��������UI��ʱ��
    public virtual void OnPause()
    {
        // ������������һЩ��ͣ�߼�������ֹͣ������
    }

    // �ָ�UI��������UI�ر�ʱ��
    public virtual void OnResume()
    {
        // ������������һЩ�ָ��߼�
    }

    // ����UI
    public virtual void OnHide()
    {
        gameObject.SetActive(false);
    }

    // ��ʾUI
    public virtual void OnShow()
    {
        gameObject.SetActive(true);
    }

    // ����UI
    public virtual void OnDestroy()
    {
        // �������ע����¼�
        ClearAllEvents();
    }

    // ע��UI�¼�
    protected void RegisterEvent(GameObject target, EventTriggerType eventType, UnityAction<BaseEventData> callback)
    {
        if (target == null || callback == null) return;

        // ����¼�������
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        // �����¼���
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(callback);

        // ��ӵ�������
        trigger.triggers.Add(entry);

        // �����¼������Ա�����
        registeredEvents.Add(new UIEventData
        {
            target = target,
            eventType = eventType,
            callback = callback
        });
    }

    // �������ע����¼�
    private void ClearAllEvents()
    {
        foreach (var eventData in registeredEvents)
        {
            if (eventData.target != null)
            {
                var trigger = eventData.target.GetComponent<EventTrigger>();
                if (trigger != null)
                {
                    // �ҵ���Ӧ���¼���Ƴ�
                    for (int i = trigger.triggers.Count - 1; i >= 0; i--)
                    {
                        if (trigger.triggers[i].eventID == eventData.eventType)
                        {
                            trigger.triggers.RemoveAt(i);
                            break;
                        }
                    }

                    // ���û���¼����ˣ��Ƴ�EventTrigger���
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
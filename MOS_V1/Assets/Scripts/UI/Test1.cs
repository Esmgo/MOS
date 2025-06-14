using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test1 : UIBase 
{
    public override void Initialize()
    {
        // ������Ϊ"Close"��Image
        var closeObj = transform.Find("Close");
        if (closeObj != null)
        {
            // ע�����¼����رձ�UI
            RegisterEvent(closeObj.gameObject, EventTriggerType.PointerClick, (e) => OnClose());
        }
        else
        {
            Debug.LogWarning("δ�ҵ���ΪClose��Image");
        }
    }

    private void Start()
    {
        
    }
}

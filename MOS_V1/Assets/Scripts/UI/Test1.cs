using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test1 : UIBase 
{
    public override void Initialize()
    {
        // 查找名为"Close"的Image
        var closeObj = transform.Find("Close");
        if (closeObj != null)
        {
            // 注册点击事件，关闭本UI
            RegisterEvent(closeObj.gameObject, EventTriggerType.PointerClick, (e) => OnClose());
        }
        else
        {
            Debug.LogWarning("未找到名为Close的Image");
        }
    }

    private void Start()
    {
        
    }
}

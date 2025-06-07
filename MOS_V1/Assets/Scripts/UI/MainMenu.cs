using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class MainMenu : UIBase
{
    public override void Initialize()
    {
        // 初始化主菜单UI
        Debug.Log("MainMenu Initialized");
        transform.Find("Button").GetComponent<UIButton>().RegisterClick(OnButtonClick);
    }

    //测试
    public void OnButtonClick(BaseEventData eventData)
    {
        Debug.Log("Button Clicked in MainMenu");
        // 可以在这里添加按钮点击后的逻辑
    }
}

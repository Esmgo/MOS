using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class MainMenu : UIBase
{
    public override void Initialize()
    {
        // ��ʼ�����˵�UI
        Debug.Log("MainMenu Initialized");
        transform.Find("Button").GetComponent<UIButton>().RegisterClick(OnButtonClick);
    }

    //����
    public void OnButtonClick(BaseEventData eventData)
    {
        Debug.Log("Button Clicked in MainMenu");
        // ������������Ӱ�ť�������߼�
    }
}

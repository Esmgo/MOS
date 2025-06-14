using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Desktop : UIBase
{
    public GameObject iconPrefab; // ����ͼ��Ԥ����
    public RectTransform iconParent; // ͼ�길����
    public int columnCount = 5; // ÿ��ͼ����
    public Vector2 iconSpacing = new Vector2(120, 120); // ͼ����

    [Header("�߽����")]
    public float marginLeft = 20f;
    public float marginRight = 20f;
    public float marginTop = 20f;
    public float marginBottom = 20f;

    private List<DesktopIcon> icons = new List<DesktopIcon>();

    public override void Initialize()
    {
        Debug.Log("Desktop Initialized");
        // ʾ������Ӽ�����ݷ�ʽ
        AddIcon("Test1", null, "���˵�");
        AddIcon("Test1", null, "����");
        ArrangeIcons();
    }

    public void AddIcon(string uiName, Sprite icon, string text)
    {
        var go = Instantiate(iconPrefab, iconParent);
        var desktopIcon = go.GetComponent<DesktopIcon>();
        desktopIcon.SetData(uiName, icon, text);
        desktopIcon.OnOpen = OnIconOpen;
        icons.Add(desktopIcon);
    }

    // �Զ�����ͼ��
    public void ArrangeIcons()
    {
        if (iconParent == null) return;

        float parentWidth = iconParent.rect.width;
        float parentHeight = iconParent.rect.height;

        // ����������
        float usableWidth = parentWidth - marginLeft - marginRight;
        float usableHeight = parentHeight - marginTop - marginBottom;

        for (int i = 0; i < icons.Count; i++)
        {
            int row = i / columnCount;
            int col = i % columnCount;
            var rt = icons[i].GetComponent<RectTransform>();
            // �����Ͻǣ����߾ࣩ��ʼ����
            rt.anchoredPosition = new Vector2(
                -parentWidth / 2f + marginLeft + col * iconSpacing.x,
                parentHeight / 2f - marginTop - row * iconSpacing.y
            );
        }
    }

    // �л���/˫��
    public void SetUseDoubleClick(bool useDoubleClick)
    {
        DesktopIcon.UseDoubleClick = useDoubleClick;
    }

    // �򿪶�Ӧ����
    private void OnIconOpen(DesktopIcon icon)
    {
        //Debug.Log($"�򿪽���: {icon.targetUIName}");
        // ����������UI�������򿪽���
        UIManager.Instance.OpenUI(icon.targetUIName);
    }

    //����
    public void OnButtonClick(BaseEventData eventData)
    {
        Debug.Log("Button Clicked in MainMenu");
        // ������������Ӱ�ť�������߼�
    }
}

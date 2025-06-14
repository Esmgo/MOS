using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Desktop : UIBase
{
    public GameObject iconPrefab; // 桌面图标预制体
    public RectTransform iconParent; // 图标父物体
    public int columnCount = 5; // 每行图标数
    public Vector2 iconSpacing = new Vector2(120, 120); // 图标间距

    [Header("边界距离")]
    public float marginLeft = 20f;
    public float marginRight = 20f;
    public float marginTop = 20f;
    public float marginBottom = 20f;

    private List<DesktopIcon> icons = new List<DesktopIcon>();

    public override void Initialize()
    {
        Debug.Log("Desktop Initialized");
        // 示例：添加几个快捷方式
        AddIcon("Test1", null, "主菜单");
        AddIcon("Test1", null, "设置");
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

    // 自动排列图标
    public void ArrangeIcons()
    {
        if (iconParent == null) return;

        float parentWidth = iconParent.rect.width;
        float parentHeight = iconParent.rect.height;

        // 可用区域宽高
        float usableWidth = parentWidth - marginLeft - marginRight;
        float usableHeight = parentHeight - marginTop - marginBottom;

        for (int i = 0; i < icons.Count; i++)
        {
            int row = i / columnCount;
            int col = i % columnCount;
            var rt = icons[i].GetComponent<RectTransform>();
            // 从左上角（含边距）开始排列
            rt.anchoredPosition = new Vector2(
                -parentWidth / 2f + marginLeft + col * iconSpacing.x,
                parentHeight / 2f - marginTop - row * iconSpacing.y
            );
        }
    }

    // 切换单/双击
    public void SetUseDoubleClick(bool useDoubleClick)
    {
        DesktopIcon.UseDoubleClick = useDoubleClick;
    }

    // 打开对应界面
    private void OnIconOpen(DesktopIcon icon)
    {
        //Debug.Log($"打开界面: {icon.targetUIName}");
        // 这里调用你的UI管理器打开界面
        UIManager.Instance.OpenUI(icon.targetUIName);
    }

    //测试
    public void OnButtonClick(BaseEventData eventData)
    {
        Debug.Log("Button Clicked in MainMenu");
        // 可以在这里添加按钮点击后的逻辑
    }
}

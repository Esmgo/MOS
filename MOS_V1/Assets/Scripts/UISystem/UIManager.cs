using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    // UI根节点
    private Transform uiRoot;

    // 所有已加载的UI界面
    private Dictionary<string, UIBase> loadedUIs = new Dictionary<string, UIBase>();

    // 正在加载的UI界面
    private Dictionary<string, AsyncOperationHandle<GameObject>> loadingUIs = new Dictionary<string, AsyncOperationHandle<GameObject>>();

    // UI堆栈（用于管理界面层级）
    private Stack<UIBase> uiStack = new Stack<UIBase>();

    // 初始化UI管理器
    public void Initialize()
    {
        uiRoot = GameObject.Find("UIRoot").transform;
    }

    /// <summary>
    /// 异步加载UI
    /// </summary>
    /// <param name="uiName">UI的aa地址</param>
    /// <param name="onComplete">加载完成回调(可选)</param>
    public void LoadUIAsync(string uiName, UnityAction<UIBase> onComplete = null)
    {
        // 如果已经加载
        if (loadedUIs.ContainsKey(uiName))
        {
            onComplete?.Invoke(loadedUIs[uiName]);
            return;
        }

        // 如果正在加载
        if (loadingUIs.ContainsKey(uiName))
        {
            // 可以在这里添加回调到现有加载操作
            return;
        }

        // 开始加载
        var handle = Addressables.LoadAssetAsync<GameObject>(uiName);
        loadingUIs.Add(uiName, handle);

        handle.Completed += (operation) =>
        {
            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject uiObj = Instantiate(operation.Result, uiRoot);
                UIBase uiBase = uiObj.GetComponent<UIBase>();

                if (uiBase != null)
                {
                    uiBase.Initialize();
                    loadedUIs.Add(uiName, uiBase);
                    onComplete?.Invoke(uiBase);
                }
                else
                {
                    Debug.LogError($"UI {uiName} 没有挂载UIBase脚本");
                }
            }
            else
            {
                Debug.LogError($"加载UI {uiName} 失败: {operation.OperationException}");
            }

            loadingUIs.Remove(uiName);
        };
    }

    // 打开UI
    public void OpenUI(string uiName, UnityAction<UIBase> onComplete = null)
    {
        if (loadedUIs.TryGetValue(uiName, out UIBase ui))
        {
            OpenUI(ui);
            onComplete?.Invoke(ui);
        }
        else
        {
            LoadUIAsync(uiName, (loadedUI) =>
            {
                OpenUI(loadedUI);
                onComplete?.Invoke(loadedUI);
            });
        }
    }

    private void OpenUI(UIBase ui)
    {
        if (uiStack.Count > 0)
        {
            UIBase topUI = uiStack.Peek();
            if (topUI != ui)
            {
                topUI.OnPause();
            }
        }

        ui.OnOpen();
        uiStack.Push(ui);
    }

    // 关闭当前UI
    public void CloseCurrentUI()
    {
        if (uiStack.Count == 0) return;

        UIBase topUI = uiStack.Pop();
        topUI.OnClose();

        if (uiStack.Count > 0)
        {
            UIBase newTopUI = uiStack.Peek();
            newTopUI.OnResume();
        }
    }

    // 隐藏UI
    public void HideUI(string uiName)
    {
        if (loadedUIs.TryGetValue(uiName, out UIBase ui))
        {
            ui.OnHide();
        }
    }

    // 显示UI
    public void ShowUI(string uiName)
    {
        if (loadedUIs.TryGetValue(uiName, out UIBase ui))
        {
            ui.OnShow();
        }
    }

    // 获取UI
    public UIBase GetUI(string uiName)
    {
        loadedUIs.TryGetValue(uiName, out UIBase ui);
        return ui;
    }

    // 卸载UI
    public void UnloadUI(string uiName)
    {
        if (loadedUIs.TryGetValue(uiName, out UIBase ui))
        {
            if (uiStack.Contains(ui))
            {
                Debug.LogWarning($"不能卸载正在使用的UI: {uiName}");
                return;
            }

            ui.OnDestroy();
            Destroy(ui.gameObject);
            loadedUIs.Remove(uiName);
            Addressables.Release(ui.gameObject);
        }
    }
}


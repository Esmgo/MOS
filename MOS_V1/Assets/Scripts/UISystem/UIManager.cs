using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    // UI���ڵ�
    private Transform uiRoot;

    // �����Ѽ��ص�UI����
    private Dictionary<string, UIBase> loadedUIs = new Dictionary<string, UIBase>();

    // ���ڼ��ص�UI����
    private Dictionary<string, AsyncOperationHandle<GameObject>> loadingUIs = new Dictionary<string, AsyncOperationHandle<GameObject>>();

    // UI��ջ�����ڹ������㼶��
    private Stack<UIBase> uiStack = new Stack<UIBase>();

    // ��ʼ��UI������
    public void Initialize()
    {
        uiRoot = GameObject.Find("UIRoot").transform;
    }

    /// <summary>
    /// �첽����UI
    /// </summary>
    /// <param name="uiName">UI��aa��ַ</param>
    /// <param name="onComplete">������ɻص�(��ѡ)</param>
    public void LoadUIAsync(string uiName, UnityAction<UIBase> onComplete = null)
    {
        // ����Ѿ�����
        if (loadedUIs.ContainsKey(uiName))
        {
            onComplete?.Invoke(loadedUIs[uiName]);
            return;
        }

        // ������ڼ���
        if (loadingUIs.ContainsKey(uiName))
        {
            // ������������ӻص������м��ز���
            return;
        }

        // ��ʼ����
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
                    Debug.LogError($"UI {uiName} û�й���UIBase�ű�");
                }
            }
            else
            {
                Debug.LogError($"����UI {uiName} ʧ��: {operation.OperationException}");
            }

            loadingUIs.Remove(uiName);
        };
    }

    // ��UI
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

    // �رյ�ǰUI
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

    // ����UI
    public void HideUI(string uiName)
    {
        if (loadedUIs.TryGetValue(uiName, out UIBase ui))
        {
            ui.OnHide();
        }
    }

    // ��ʾUI
    public void ShowUI(string uiName)
    {
        if (loadedUIs.TryGetValue(uiName, out UIBase ui))
        {
            ui.OnShow();
        }
    }

    // ��ȡUI
    public UIBase GetUI(string uiName)
    {
        loadedUIs.TryGetValue(uiName, out UIBase ui);
        return ui;
    }

    // ж��UI
    public void UnloadUI(string uiName)
    {
        if (loadedUIs.TryGetValue(uiName, out UIBase ui))
        {
            if (uiStack.Contains(ui))
            {
                Debug.LogWarning($"����ж������ʹ�õ�UI: {uiName}");
                return;
            }

            ui.OnDestroy();
            Destroy(ui.gameObject);
            loadedUIs.Remove(uiName);
            Addressables.Release(ui.gameObject);
        }
    }
}


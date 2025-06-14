using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApplication : MonoBehaviour
{
    private void Awake()
    {
        DOTween.Init();
        UIManager.Instance.Initialize(); // 初始化UI管理器
    }

    private void Start()
    {
        UIManager.Instance.LoadUIAsync("Desktop", (ui) =>
        {
            ui.OnOpen(); // 打开主菜单UI
        });
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApplication : MonoBehaviour
{
    private void Awake()
    {
        DOTween.Init();
        UIManager.Instance.Initialize(); // ��ʼ��UI������
    }

    private void Start()
    {
        UIManager.Instance.LoadUIAsync("Desktop", (ui) =>
        {
            ui.OnOpen(); // �����˵�UI
        });
    }
}

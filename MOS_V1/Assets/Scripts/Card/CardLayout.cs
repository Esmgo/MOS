using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLayout : MonoBehaviour
{
    [Header("排列参数")]
    public float cardMoveDuration = 0.3f; // 动画时间
    public float cardSpacing = 30f; // 默认卡牌间距

    private List<CardMove> cards = new List<CardMove>();
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        RefreshCards();
        ArrangeCards();
    }

    public void RefreshCards()
    {
        int no = 0;
        cards.Clear();
        foreach (Transform child in transform)
        {
            var card = child.GetComponent<CardMove>();
            if (card != null)
            {
                cards.Add(card);
                card.SetOriginalSiblingIndex(no++); // 设置原始的兄弟索引
            }
        }
    }

    public void ArrangeCards()
    {
        int count = cards.Count;
        if (count == 0) return;

        // 获取父物体宽度
        float parentWidth = rectTransform != null ? rectTransform.rect.width : 1000f;

        // 获取单张卡牌宽度（假设所有卡牌宽度一致）
        float cardWidth = cards[0].CardTransform.rect.width * cards[0].CardTransform.lossyScale.x;

        // 计算理论总宽度
        float totalWidth = cardWidth * count + cardSpacing * (count - 1);

        // 允许重叠：当卡牌总宽度大于父物体宽度时，压缩间距为负数
        float spacing = cardSpacing;
        if (totalWidth > parentWidth && count > 1)
        {
            spacing = (parentWidth - cardWidth * count) / (count - 1);
            // 允许spacing为负，实现重叠
        }

        // 起始X坐标，使卡牌整体居中
        float startX = (rectTransform != null ? rectTransform.position.x : transform.position.x) - (cardWidth * count + spacing * (count - 1)) / 2f + cardWidth / 2f;

        Vector3 basePos = rectTransform != null ? rectTransform.position : transform.position;

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * (cardWidth + spacing);
            Vector3 pos = new Vector3(x, basePos.y, basePos.z);

            // 设置该位置为卡牌的初始位置
            cards[i].SetOriginalPosition(pos);

            // 移动到目标位置，旋转归零
            cards[i].CardTransform.DORotate(Vector3.zero, cardMoveDuration).SetEase(Ease.OutQuad);
        }
    }
}

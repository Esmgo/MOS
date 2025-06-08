using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLayout : MonoBehaviour
{
    [Header("���в���")]
    public float cardMoveDuration = 0.3f; // ����ʱ��
    public float cardSpacing = 30f; // Ĭ�Ͽ��Ƽ��

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
                card.SetOriginalSiblingIndex(no++); // ����ԭʼ���ֵ�����
            }
        }
    }

    public void ArrangeCards()
    {
        int count = cards.Count;
        if (count == 0) return;

        // ��ȡ��������
        float parentWidth = rectTransform != null ? rectTransform.rect.width : 1000f;

        // ��ȡ���ſ��ƿ�ȣ��������п��ƿ��һ�£�
        float cardWidth = cards[0].CardTransform.rect.width * cards[0].CardTransform.lossyScale.x;

        // ���������ܿ��
        float totalWidth = cardWidth * count + cardSpacing * (count - 1);

        // �����ص����������ܿ�ȴ��ڸ�������ʱ��ѹ�����Ϊ����
        float spacing = cardSpacing;
        if (totalWidth > parentWidth && count > 1)
        {
            spacing = (parentWidth - cardWidth * count) / (count - 1);
            // ����spacingΪ����ʵ���ص�
        }

        // ��ʼX���꣬ʹ�����������
        float startX = (rectTransform != null ? rectTransform.position.x : transform.position.x) - (cardWidth * count + spacing * (count - 1)) / 2f + cardWidth / 2f;

        Vector3 basePos = rectTransform != null ? rectTransform.position : transform.position;

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * (cardWidth + spacing);
            Vector3 pos = new Vector3(x, basePos.y, basePos.z);

            // ���ø�λ��Ϊ���Ƶĳ�ʼλ��
            cards[i].SetOriginalPosition(pos);

            // �ƶ���Ŀ��λ�ã���ת����
            cards[i].CardTransform.DORotate(Vector3.zero, cardMoveDuration).SetEase(Ease.OutQuad);
        }
    }
}

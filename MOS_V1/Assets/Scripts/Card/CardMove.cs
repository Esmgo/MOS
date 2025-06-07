using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMove : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [TitleGroup("��������"), PropertyOrder(0)]
    [Required] public RectTransform CardTransform;
    [Required] public Image CardImage;

    [TitleGroup("�϶�����"), PropertyOrder(1)]
    [Tooltip("�Ƿ�ƽ���ƶ�")]
    public bool SmoothMovement = true;
    [ShowIf("SmoothMovement"), Range(1, 50)]
    public float FollowSpeed = 25f;

    [TitleGroup("��ת����"), PropertyOrder(2)]
    [Range(0.1f, 2f)]
    public float RotationSensitivity = 0.6f;
    [Range(5, 45)]
    public float MaxRotationAngle = 25f;
    [Range(0.01f, 0.5f)]
    public float RotationDuration = 0.15f;

    [TitleGroup("��������"), PropertyOrder(3)]
    [Range(1, 1.2f)]
    public float HoverScale = 1.05f;
    [Range(0.1f, 1f)]
    public float ScaleDuration = 0.2f;

    [TitleGroup("���ض���"), PropertyOrder(4)]
    [Range(0.1f, 1f)]
    public float ReturnDuration = 0.4f;
    [ValueDropdown("GetEaseTypes")]
    public Ease ReturnEase = Ease.OutBack;

    [TitleGroup("��ӰЧ��"), PropertyOrder(5), Optional]
    public Shadow CardShadow;
    [ShowIf("@CardShadow != null"), Range(0, 30)]
    public float MaxShadowDistance = 15f;
    [ShowIf("@CardShadow != null")]
    public Vector2 MaxShadowOffset = new Vector2(10, -10);

    // ˽�б���
    private Vector3 originalScale;
    private Vector2 lastPosition;
    private Tween rotateTween;
    private Tween returnTween;
    private Tween scaleTween;
    private bool isDragging;

    // Odin�ṩ�Ļ������������б�
    private Ease[] GetEaseTypes => new Ease[]
    {
        Ease.Linear,
        Ease.InBack,
        Ease.OutBack,
        Ease.InOutBack,
        Ease.InBounce,
        Ease.OutBounce,
        Ease.InElastic,
        Ease.OutElastic
    };

    [Button("�Զ���ȡ����"), PropertyOrder(-1)]
    private void AssignReferences()
    {
        CardTransform = GetComponent<RectTransform>();
        CardImage = GetComponent<Image>();
        CardShadow = GetComponent<Shadow>();
        originalScale = CardTransform.localScale;
    }

    private void Awake()
    {
        if (CardTransform == null) CardTransform = GetComponent<RectTransform>();
        if (CardImage == null) CardImage = GetComponent<Image>();
        if (CardShadow == null) CardShadow = GetComponent<Shadow>();

        originalScale = CardTransform.localScale;
    }

    private void Update()
    {
        // �����ƶ�����
        Vector2 currentPosition = CardTransform.position;
        Vector2 moveDirection = currentPosition - lastPosition;
        float moveSpeed = moveDirection.magnitude / Time.deltaTime;

        // ���㲢Ӧ����ת
        ApplyCardRotation(moveDirection, moveSpeed);

        // ������ӰЧ��
        UpdateShadowEffect(moveDirection, moveSpeed);

        lastPosition = currentPosition;
    }

    [TitleGroup("״̬"), ShowInInspector, ReadOnly]
    private string DragStatus => isDragging ? "�����϶�" : "����";

    public void OnBeginDrag(PointerEventData eventData)
    {
        returnTween?.Kill();
        scaleTween?.Kill();

        isDragging = true;
        lastPosition = CardTransform.position; 

        // ��ͣ����
        scaleTween = CardTransform.DOScale(originalScale * HoverScale, ScaleDuration)
            .SetEase(Ease.OutBack);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ����λ��
        if (SmoothMovement)
        {
            transform.DOMove(eventData.position, FollowSpeed * Time.deltaTime)
                .SetEase(Ease.Linear);
            //CardTransform.position = Vector2.Lerp(
            //    CardTransform.position,
            //    eventData.position,
            //    FollowSpeed * Time.deltaTime);
        }
        else
        {
            CardTransform.position = eventData.position;
        }

        
    }

    [FoldoutGroup("����", false), Button("������ת")]
    private void TestRotation(float angle)
    {
        CardTransform.DORotate(new Vector3(0, 0, angle), 0.5f)
            .SetEase(Ease.OutBack);
    }

    private void ApplyCardRotation(Vector2 moveDirection, float moveSpeed)
    {
        float targetRotationZ = -moveDirection.x * RotationSensitivity * Mathf.Clamp01(moveSpeed / 3000f);
        targetRotationZ = Mathf.Clamp(targetRotationZ, -MaxRotationAngle, MaxRotationAngle);
        Debug.Log(moveSpeed + "  " + targetRotationZ);
        rotateTween?.Kill();
        rotateTween = CardTransform.DORotate(new Vector3(0, 0, targetRotationZ), RotationDuration)
            .SetEase(Ease.OutQuad);
    }

    private void UpdateShadowEffect(Vector2 moveDirection, float moveSpeed)
    {
        if (CardShadow == null) return;

        float speedFactor = Mathf.Clamp01(moveSpeed / 300f);
        Vector2 directionNormalized = moveDirection.normalized;

        Vector2 shadowOffset = new Vector2(
            directionNormalized.x * MaxShadowOffset.x * speedFactor,
            directionNormalized.y * MaxShadowOffset.y * speedFactor);

        CardShadow.effectDistance = Vector2.Lerp(
            CardShadow.effectDistance,
            shadowOffset,
            10f * Time.deltaTime);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // ���ض���
        returnTween = CardTransform.DORotate(Vector3.zero, ReturnDuration)
            .SetEase(ReturnEase)
            .OnComplete(() => returnTween = null);

        // ���ŷ���
        scaleTween = CardTransform.DOScale(originalScale, ScaleDuration)
            .SetEase(Ease.InOutQuad);

        // ��Ӱ����
        if (CardShadow != null)
        {
            DOTween.To(() => CardShadow.effectDistance,
                x => CardShadow.effectDistance = x,
                Vector2.zero,
                ReturnDuration);
        }
    }

    private void OnDisable()
    {
        rotateTween?.Kill();
        returnTween?.Kill();
        scaleTween?.Kill();
    }
}

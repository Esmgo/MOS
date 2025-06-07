using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMove : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [TitleGroup("基本设置"), PropertyOrder(0)]
    [Required] public RectTransform CardTransform;
    [Required] public Image CardImage;

    [TitleGroup("拖动设置"), PropertyOrder(1)]
    [Tooltip("是否平滑移动")]
    public bool SmoothMovement = true;
    [ShowIf("SmoothMovement"), Range(1, 50)]
    public float FollowSpeed = 25f;

    [TitleGroup("旋转设置"), PropertyOrder(2)]
    [Range(0.1f, 2f)]
    public float RotationSensitivity = 0.6f;
    [Range(5, 45)]
    public float MaxRotationAngle = 25f;
    [Range(0.01f, 0.5f)]
    public float RotationDuration = 0.15f;

    [TitleGroup("缩放设置"), PropertyOrder(3)]
    [Range(1, 1.2f)]
    public float HoverScale = 1.05f;
    [Range(0.1f, 1f)]
    public float ScaleDuration = 0.2f;

    [TitleGroup("返回动画"), PropertyOrder(4)]
    [Range(0.1f, 1f)]
    public float ReturnDuration = 0.4f;
    [ValueDropdown("GetEaseTypes")]
    public Ease ReturnEase = Ease.OutBack;

    [TitleGroup("阴影效果"), PropertyOrder(5), Optional]
    public Shadow CardShadow;
    [ShowIf("@CardShadow != null"), Range(0, 30)]
    public float MaxShadowDistance = 15f;
    [ShowIf("@CardShadow != null")]
    public Vector2 MaxShadowOffset = new Vector2(10, -10);

    // 私有变量
    private Vector3 originalScale;
    private Vector2 lastPosition;
    private Tween rotateTween;
    private Tween returnTween;
    private Tween scaleTween;
    private bool isDragging;

    // Odin提供的缓动类型下拉列表
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

    [Button("自动获取引用"), PropertyOrder(-1)]
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
        // 计算移动参数
        Vector2 currentPosition = CardTransform.position;
        Vector2 moveDirection = currentPosition - lastPosition;
        float moveSpeed = moveDirection.magnitude / Time.deltaTime;

        // 计算并应用旋转
        ApplyCardRotation(moveDirection, moveSpeed);

        // 更新阴影效果
        UpdateShadowEffect(moveDirection, moveSpeed);

        lastPosition = currentPosition;
    }

    [TitleGroup("状态"), ShowInInspector, ReadOnly]
    private string DragStatus => isDragging ? "正在拖动" : "闲置";

    public void OnBeginDrag(PointerEventData eventData)
    {
        returnTween?.Kill();
        scaleTween?.Kill();

        isDragging = true;
        lastPosition = CardTransform.position; 

        // 悬停动画
        scaleTween = CardTransform.DOScale(originalScale * HoverScale, ScaleDuration)
            .SetEase(Ease.OutBack);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 更新位置
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

    [FoldoutGroup("方法", false), Button("测试旋转")]
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

        // 返回动画
        returnTween = CardTransform.DORotate(Vector3.zero, ReturnDuration)
            .SetEase(ReturnEase)
            .OnComplete(() => returnTween = null);

        // 缩放返回
        scaleTween = CardTransform.DOScale(originalScale, ScaleDuration)
            .SetEase(Ease.InOutQuad);

        // 阴影返回
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

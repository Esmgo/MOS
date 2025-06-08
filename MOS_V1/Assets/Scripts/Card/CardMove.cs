using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMove : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [TitleGroup("基本设置"), PropertyOrder(0)]
    [Required] public RectTransform CardTransform;

    [TitleGroup("拖动设置"), PropertyOrder(1)]
    [LabelText("跟随时间"),Range(0, 5)]
    public float FollowSpeed = 0.2f;

    [TitleGroup("旋转设置"), PropertyOrder(2)]
    [Range(0.1f, 2f)]
    public float RotationSensitivity = 1.5f;
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
    public float ReturnDuration = 0.2f;
    [ValueDropdown("GetEaseTypes")]
    public Ease ReturnEase = Ease.OutQuad;

    // 私有变量
    private Vector3 originalPosition;
    private Vector2 lastPosition;
    private Tween rotateTween;
    private Tween moveTween;
    private Tween scaleTween;
    private bool isDragging;
    private float targetRotationZ;
    private int originalSiblingIndex;

    // Odin提供的缓动类型下拉列表
    private Ease[] GetEaseTypes => new Ease[]
    {
        Ease.Linear,
        Ease.InBack,
        Ease.OutBack,
        Ease.InOutBack,
        Ease.InQuad,
        Ease.OutQuad,
        Ease.InBounce,
        Ease.OutBounce,
        Ease.InElastic,
        Ease.OutElastic
    };



    private void Update()
    {
        ApplyCardRotation();
    }

    [TitleGroup("状态"), ShowInInspector, ReadOnly]
    private string DragStatus => isDragging ? "正在拖动" : "闲置";

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.SetSiblingIndex(originalSiblingIndex); // 恢复原始兄弟索引
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        moveTween?.Kill();
        scaleTween?.Kill();

        isDragging = true;
        lastPosition = CardTransform.position; 

    }

    public void OnDrag(PointerEventData eventData)
    {
        // 更新位置
        MoveToPos(eventData.position, FollowSpeed, Ease.OutQuad);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        // 返回原位置
        MoveToPos(originalPosition, ReturnDuration, ReturnEase);
    }

    /// <summary>
    /// 移动到位置
    /// </summary>
    /// <param name="aimPOs">位置</param>
    /// <param name="time">时间</param>
    /// <param name="ease">动画曲线</param>
    public void MoveToPos(Vector3 aimPOs, float time, Ease ease = Ease.Flash)
    {
        moveTween?.Kill();
        moveTween = transform.DOMove(aimPOs, time)
            .SetEase(ease)
            .OnComplete(() => moveTween = null);
    }

    public void SetOriginalPosition(Vector3 originalPosition)
    {
        this.originalPosition = originalPosition;
        MoveToPos(originalPosition, FollowSpeed, Ease.OutQuad);
    }

    public void SetOriginalSiblingIndex(int siblingIndex)
    {
        originalSiblingIndex = siblingIndex;
        transform.SetSiblingIndex(originalSiblingIndex);
    }

    private void ApplyCardRotation()
    {
        // 计算移动参数
        Vector2 currentPosition = CardTransform.position;
        Vector2 moveDirection = currentPosition - lastPosition;
        float moveSpeed = moveDirection.magnitude / Time.deltaTime;

        // 计算并应用旋转
        lastPosition = currentPosition;
        targetRotationZ = -moveDirection.x * RotationSensitivity * Mathf.Clamp01(moveSpeed / 500f);
        targetRotationZ = Mathf.Clamp(targetRotationZ, -MaxRotationAngle, MaxRotationAngle);
        rotateTween?.Kill();
        rotateTween = CardTransform.DORotate(new Vector3(0, 0, targetRotationZ), RotationDuration)
            .SetEase(Ease.OutQuad);
    }

    private void OnDisable()
    {
        rotateTween?.Kill();
        moveTween?.Kill();
        scaleTween?.Kill();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.SetAsLastSibling(); // 确保卡牌在最上层
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}

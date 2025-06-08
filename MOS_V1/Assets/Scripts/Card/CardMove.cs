using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMove : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [TitleGroup("��������"), PropertyOrder(0)]
    [Required] public RectTransform CardTransform;

    [TitleGroup("�϶�����"), PropertyOrder(1)]
    [LabelText("����ʱ��"),Range(0, 5)]
    public float FollowSpeed = 0.2f;

    [TitleGroup("��ת����"), PropertyOrder(2)]
    [Range(0.1f, 2f)]
    public float RotationSensitivity = 1.5f;
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
    public float ReturnDuration = 0.2f;
    [ValueDropdown("GetEaseTypes")]
    public Ease ReturnEase = Ease.OutQuad;

    // ˽�б���
    private Vector3 originalPosition;
    private Vector2 lastPosition;
    private Tween rotateTween;
    private Tween moveTween;
    private Tween scaleTween;
    private bool isDragging;
    private float targetRotationZ;
    private int originalSiblingIndex;

    // Odin�ṩ�Ļ������������б�
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

    [TitleGroup("״̬"), ShowInInspector, ReadOnly]
    private string DragStatus => isDragging ? "�����϶�" : "����";

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.SetSiblingIndex(originalSiblingIndex); // �ָ�ԭʼ�ֵ�����
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
        // ����λ��
        MoveToPos(eventData.position, FollowSpeed, Ease.OutQuad);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        // ����ԭλ��
        MoveToPos(originalPosition, ReturnDuration, ReturnEase);
    }

    /// <summary>
    /// �ƶ���λ��
    /// </summary>
    /// <param name="aimPOs">λ��</param>
    /// <param name="time">ʱ��</param>
    /// <param name="ease">��������</param>
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
        // �����ƶ�����
        Vector2 currentPosition = CardTransform.position;
        Vector2 moveDirection = currentPosition - lastPosition;
        float moveSpeed = moveDirection.magnitude / Time.deltaTime;

        // ���㲢Ӧ����ת
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
        transform.SetAsLastSibling(); // ȷ�����������ϲ�
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}

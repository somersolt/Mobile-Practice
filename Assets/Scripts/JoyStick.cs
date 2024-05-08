using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public enum Axis
    {
        Horizontal,
        Vertical,
    }

    public RectTransform stick;
    private float radius;

    private Vector2 direction = Vector2.zero;

    private RectTransform rectTr;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();  
        rectTr = GetComponent<RectTransform>();
        radius = rectTr.sizeDelta.x * 0.5f;
    }
    private void Update()
    {
        Debug.Log(direction);
    }

    public float GetAxis(Axis axis)
    {
        switch (axis) 
        {
                case Axis.Horizontal:
                return direction.x;

                case Axis.Vertical:
                return direction.y;

        }

        return 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTr, eventData.position, canvas.worldCamera, out var pos))
        {
            pos = Vector3.ClampMagnitude(pos, radius);
            stick.anchoredPosition = pos;

            direction = pos.normalized;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        stick.anchoredPosition = Vector2.zero;
    }
}

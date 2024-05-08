using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultiTouchMgr : Singleton<MultiTouchMgr>
{
    public bool Tap { get; private set; }
    public bool LongTap { get; private set; }
    public bool DoubleTap { get; private set; }
    public Vector3 Swipe { get; private set; }
    public float Zoom { get; private set; }

    private int primaryFingerId = int.MinValue;
    private int secondFingerId = int.MinValue;

    private float timeTap = 0.25f;
    private float timeLongTap = 0.5f;
    private float timeDoubleTap = 0.25f;

    private bool isFirstTap = false;

    private float primaryStartTime = 0f;

    private float firstTapTime = 0f;

    private Vector2 touchStartPos;
    private Vector2 touchMovePos;

    private float zoomDistance;
    private float swipeStartTime = 0;
    private float swipeTime = 0.25f;

    public float minSwipeDistanceInch = 0.25f; // Inch
    private float minSwipeDistancePixels;

    private void Start()
    {
        minSwipeDistancePixels = Screen.dpi * minSwipeDistanceInch;
    }
    private void Update()
    {
        Tap = false;
        DoubleTap = false;
        LongTap = false;
        Swipe = Vector3.zero;
        Zoom = 0f;
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (primaryFingerId == int.MinValue)
                    {
                        primaryFingerId = touch.fingerId;
                        primaryStartTime = Time.time;

                        touchStartPos = Input.touches[i].position;
                        swipeStartTime = Time.time;
                        break;
                    }
                    if (primaryFingerId != int.MinValue && secondFingerId == int.MinValue)
                    {
                        secondFingerId = touch.fingerId;
                        var touchSecondStartPos = Input.touches[i].position;
                        zoomDistance = Vector2.Distance(touchSecondStartPos, touchStartPos);
                    }
                    break;
                case TouchPhase.Moved:
                    if (primaryFingerId != int.MinValue && secondFingerId != int.MinValue)
                    {
                        if (primaryFingerId == touch.fingerId)
                        {
                            touchMovePos = Input.touches[i].position;
                        }
                        if (secondFingerId == touch.fingerId)
                        {
                            var touchSecondMovePos = Input.touches[i].position;
                            var secondZoomDistance = Vector2.Distance(touchSecondMovePos, touchMovePos);

                            Zoom = secondZoomDistance - zoomDistance;
                            Debug.Log(zoomDistance + " / " + secondZoomDistance.ToString());
                        }
                    }
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (primaryFingerId == touch.fingerId)
                    {
                        primaryFingerId = int.MinValue;
                        var duration = Time.time - primaryStartTime;

                        var delta = Input.touches[i].position - touchStartPos;
                        var distance = delta.magnitude;

                        if (distance > minSwipeDistancePixels && Time.time - swipeStartTime < swipeTime)
                        {
                            var dir1 = delta.x > 0 ? Vector3.right : Vector3.left;
                            var dir2 = delta.y > 0 ? Vector3.up : Vector3.down;
                            Swipe = Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? dir1 : dir2;
                            swipeStartTime = Time.time;
                        }

                        if (duration < timeTap)
                        {
                            Tap = true;


                            if (isFirstTap && Time.time - firstTapTime > timeDoubleTap)
                            {
                                isFirstTap = false;
                            }


                            if (!isFirstTap)
                            {
                                isFirstTap = true;
                                firstTapTime = Time.time;
                            }
                            else
                            {
                                DoubleTap = Time.time - firstTapTime < timeDoubleTap;
                                isFirstTap = false;
                                firstTapTime = 0f;
                            }
                        }

                        if (duration > timeLongTap)
                        {
                            LongTap = true;
                        }

                    }
                    if (secondFingerId == touch.fingerId)
                    {
                        secondFingerId = int.MinValue;
                    }
                    break;

            }
        }
    }
}

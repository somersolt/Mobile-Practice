using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTouchMgr : Singleton<MultiTouchMgr>
{
    public bool Tap { get; private set; }
    public bool LongTap { get; private set; }
    public bool DoubleTap { get; private set; }

    private int primaryFingerId = int.MinValue;

    private float timeTap = 0.25f;
    private float timeLongTap = 0.5f;
    private float timeDoubleTap = 0.25f;

    private bool isFirstTap = false;

    private float primaryStartTime = 0f;

    private float firstTapTime = 0f;
    private void Update()
    {
        Tap = false;
        DoubleTap = false;
        LongTap = false;

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
                    }
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (primaryFingerId == touch.fingerId)
                    {
                        primaryFingerId = int.MinValue;
                        var duration = Time.time - primaryStartTime;

                        if (duration < timeTap)
                        {
                            Tap = true;


                            if(isFirstTap && Time.time - firstTapTime > timeDoubleTap)
                            {
                                isFirstTap = false;
                            }


                            if(!isFirstTap)
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
                    break;
            }
        }
    }
}

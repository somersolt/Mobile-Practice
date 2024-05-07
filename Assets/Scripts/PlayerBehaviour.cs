using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;

/// <summary> 
/// Responsible for moving the player automatically and 
/// reciving input. 
/// </summary> 
[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    /// <summary> 
    /// A reference to the Rigidbody component 
    /// </summary> 
    private Rigidbody rb;

    [Tooltip("How fast the ball moves left/right")]
    public float dodgeSpeed = 5;

    [Tooltip("How fast the ball moves forwards automatically")]
    [Range(0, 10)]
    public float rollSpeed = 5;

    public float minSwipeDistanceInch = 0.25f; // Inch
    private float minSwipeDistancePixels;

    private int fingerId = int.MinValue;
    private Vector2 touchStartPos;

    private float swipeStartTime = 0;
    private float swipeTime = 0.25f;
    private float swipeTimer = 0f;


    public float swipeMove = 2f;

    private Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        // Get access to our Rigidbody component 
        rb = GetComponent<Rigidbody>();
        minSwipeDistancePixels = Screen.dpi * minSwipeDistanceInch;
    }

    /// <summary>
    /// FixedUpdate is called at a fixed framerate and is a prime place to put
    /// Anything based on time.
    /// </summary>
    /// 
    private void FixedUpdate()
    {
        var pos = rb.position;
        pos += dir * swipeMove;
        rb.MovePosition(pos);

        dir = Vector3.zero;

        rb.AddForce(0, 0, rollSpeed);
    }
    void Update()
    {
        if (MultiTouchMgr.Instance.Tap)
        {
            Debug.Log("Tap");
        }
        if (MultiTouchMgr.Instance.DoubleTap)
        {
            Debug.Log("DoubleTap");
        }
        if (MultiTouchMgr.Instance.LongTap)
        {
            Debug.Log("LongTap");
        }

#if !UNITY_EDITOR
        Debug.Log(Input.mousePosition);
#endif

        var horizontalSpeed = 0f;

#if UNITY_ANDROID
        if (Input.touchCount > 0 )
        {
            //horizontalSpeed = (Input.touches[0].position.x < Screen.width * 0.5f) ? -1f : 1f;
        }
#endif

        var h = 0f;
        if (Input.touchCount > 0)
        {
            var pos = Camera.main.ScreenToViewportPoint(Input.touches[0].position);
            h = pos.x;
        }

        horizontalSpeed = ( h < 0.5f ? -1 : 1) * dodgeSpeed;
        //if (Input.GetMouseButton(0))
        //{
        //   horizontalSpeed = (Input.mousePosition.x < Screen.width * 0.5f)  ? -1f : 1f;
        //}

        for (int i = 0; i < Input.touchCount; ++i)
        {
            switch (Input.touches[i].phase)
            {
                case TouchPhase.Began:
                    if (fingerId == int.MinValue)
                    {
                        fingerId = Input.touches[i].fingerId;
                        touchStartPos = Input.touches[i].position;
                        swipeStartTime = Time.time;
                        Debug.Log(1);
                    }
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (fingerId == Input.touches[i].fingerId)
                    {

                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (fingerId == Input.touches[i].fingerId)
                    {
                        var delta = Input.touches[i].position - touchStartPos;
                        var distance = delta.magnitude;

                        if (distance > minSwipeDistancePixels && Time.time - swipeStartTime < swipeTime)
                        {
                            dir = delta.x > 0 ? Vector3.right : Vector3.left;
                        }
                        else
                        {
                            //Debug.Log("Swipe X");
                        }
                        fingerId = int.MinValue;
                        Debug.Log(2);
                    }
                    break;

            }

        }


        //Logger.Log(horizontalSpeed.ToString());
        rb.AddForce(horizontalSpeed, 0, rollSpeed);
    }
}
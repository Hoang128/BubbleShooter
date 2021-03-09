using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr : MonoBehaviour
{
    [SerializeField] float fireRate = 1f;

    bool canTouch = true;
    Vector2 touchPoint;
    RaycastHit2D touchHit;
    GameObject bubbleListMgr;

    //Debug
    GameObject currentBubble;

    // Start is called before the first frame update
    void Awake()
    {
        bubbleListMgr = GameObject.Find("BubbleMgr");
        currentBubble = bubbleListMgr.GetComponent<BubbleListMgr>().BubbleTypeList[0];
    }

    // Update is called once per frame
    void Update()
    {
        TouchHandle();
    }

    void TouchHandle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canTouch)
            {
                touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchHit = Physics2D.Raycast(touchPoint, Vector2.zero);

                if (touchHit)
                {
                    if (touchHit.collider.gameObject.tag == "Bubble")
                    {
                        Instantiate(currentBubble, touchPoint, transform.rotation);
                    }
                }

                canTouch = false;
                StartCoroutine(enableTouchAfter(fireRate));
            }
        }
    }

    IEnumerator enableTouchAfter(float seconds)
    {
        yield return new WaitForSeconds(fireRate);

        canTouch = true;
    }
}

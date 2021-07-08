using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] GameObject[] bulletTypes;
    [SerializeField] GameObject aimObject;
    [SerializeField] GameObject bubbleMapMgrObj;
    [SerializeField] float aimObjectSpace;
    [SerializeField] float maxAngle;
    [SerializeField] float space;

    int aimLength = 30;
    Stack<Ray2D> aimLineList;
    Vector3 lastMouseCoordinate = Vector3.zero;
    Vector2 finalShotPos;
    Collider2D bubbleCol = null;
    List<GameObject> aimObjectList;
    GameObject bulletMgr;
    
    BubbleMapMgr bubbleMapMgr;
    
    // Start is called before the first frame update
    void Start()
    {
        bulletMgr = GameObject.Find("BulletMgr");
        aimLineList = new Stack<Ray2D>();
        finalShotPos = transform.position;
        aimObjectList = new List<GameObject>();
        bubbleMapMgr = bubbleMapMgrObj.GetComponent<BubbleMapMgr>();
        
        for (int i = 0; i< aimLength; i++)
        {
            GameObject aimObj = Instantiate(aimObject, transform.position, transform.rotation);
            aimObj.transform.parent = gameObject.transform;
            aimObjectList.Add(aimObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bubbleMapMgr.State == BubbleMapMgr.MapState.IDLE)
        {
            if (Input.GetMouseButton(0))
            {
                if (bulletMgr.GetComponent<BulletMgr>().BulletShot != null)
                {
                    if (!bulletMgr.GetComponent<BulletMgr>().BulletShot.GetComponent<BubbleBullet>().Moving)
                    {
                        Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;

                        if (mouseDelta != new Vector3(0, 0, 0))
                        {
                            UpdateAimLineList();
                            DisplayAimLine(aimLineList);
                        }
                        lastMouseCoordinate = Input.mousePosition;
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (bulletMgr.GetComponent<BulletMgr>().BulletShot != null)
                {
                    if (!bulletMgr.GetComponent<BulletMgr>().BulletShot.GetComponent<BubbleBullet>().Moving)
                    {
                        Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;

                        if (mouseDelta != new Vector3(0, 0, 0))
                        {
                            UpdateAimLineList();
                            DisplayAimLine(aimLineList);
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (aimLineList.Count != 0)
                {
                    bulletMgr.GetComponent<BulletMgr>().LaunchBubble(aimLineList, finalShotPos, bubbleCol);

                    aimLineList.Clear();

                    finalShotPos = transform.position;
                    bubbleCol = null;

                    HideAimLine();
                }
            }
        }
    }

    void UpdateAimLineList()
    {
        aimLineList.Clear();
        Vector2 touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
        Ray2D aimRay = new Ray2D(transform.position, touchPoint - pos2D);

        float aimAngle = Vector2.Angle(Vector2.up, aimRay.direction);

        if (aimAngle > maxAngle)
            return;

        bool aiming = true;
        aimLineList.Push(aimRay);
        
        if (transform.position.x <= touchPoint.x)
            transform.eulerAngles = new Vector3(0, 0, -aimAngle);
        else
            transform.eulerAngles = new Vector3(0, 0, aimAngle);

        while (aiming)
        {
            RaycastHit2D aimHit = Physics2D.Raycast(aimLineList.Peek().origin, aimLineList.Peek().direction);
            if (aimHit)
            {
                if (aimHit.collider.tag == "Wall")
                {
                    Ray2D nextAimRay = new Ray2D(aimHit.point - aimLineList.Peek().direction.normalized * space, Vector2.Reflect(aimLineList.Peek().direction, aimHit.normal));

                    aimLineList.Push(nextAimRay);
                }
                else if (aimHit.collider.tag == "Bubble")
                {
                    finalShotPos = aimHit.point;
                    bubbleCol = aimHit.collider;
                    aiming = false;
                }
            }
            else
                aiming = false;
        }
    }

    void DisplayAimLine(Stack<Ray2D> aimLineList)
    {
        HideAimLine();

        List<Ray2D> aimRayList = new List<Ray2D>(aimLineList);

        int currentAimCount = 0;      
        int aimObjIndex = 0;

        for (int rayCount = aimRayList.Count - 1; rayCount >= 0;)
        {
            if (aimObjIndex < aimObjectList.Count)
            {
                Vector2 startPoint = aimRayList[rayCount].origin;
                Vector2 endPoint = new Vector2(0, 0);
                if (rayCount > 0)
                    endPoint = aimRayList[rayCount - 1].origin;
                else
                    endPoint = finalShotPos;

                Vector2 aimObjPos = startPoint + aimObjectSpace * aimRayList[rayCount].direction.normalized * currentAimCount;
                if ((endPoint - aimObjPos).magnitude < (aimObjectSpace/2))
                {
                    rayCount--;
                    currentAimCount = 0;
                }
                else
                {
                    aimObjectList[aimObjIndex].transform.position = aimObjPos;
                    currentAimCount++;
                    aimObjIndex++;
                }
            }
            else
            {
                break;
            }
        }
    }

    void HideAimLine()
    {
        foreach (GameObject aimObj in aimObjectList)
        {
            aimObj.transform.position = transform.position;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (Ray2D aimRay in aimLineList)
            Gizmos.DrawLine(aimRay.origin, aimRay.origin + aimRay.direction * 100f);
        Gizmos.DrawLine(finalShotPos + Vector2.up, finalShotPos + Vector2.down);
        Gizmos.DrawLine(finalShotPos + Vector2.right, finalShotPos + Vector2.left);
    }
#endif
}

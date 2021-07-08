using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum BulletState { IN_QUEUE, READY_SHOT, MOVING};

public class BubbleBullet : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Bubble.BubbleType bulletType;
    Vector3 posInQueue;
    Vector3 posReady;

    BulletState state = BulletState.IN_QUEUE;
    BulletMgr bulletMgr;
    Stack<Ray2D> moveRayList;
    bool moving = false;
    bool haveMovePos = true;
    bool canCreateBubble = true;
    Vector2 moveDir;
    Vector2 movePos;
    Vector2 finalPos;
    GameObject bubbleMapMgr;
    Collider2D bubbleCol;
    GameObject shooter;

    public Stack<Ray2D> MoveRayList { get => moveRayList; set => moveRayList = value; }
    public bool CanCreateBubble { get => canCreateBubble; set => canCreateBubble = value; }
    public Vector2 FinalPos { get => finalPos; set => finalPos = value; }
    public Collider2D BubbleCol { get => bubbleCol; set => bubbleCol = value; }
    public bool Moving { get => moving; set => moving = value; }
    public Bubble.BubbleType BulletType { get => bulletType; set => bulletType = value; }
    public BulletState State { get => state; set => state = value; }
    public BulletMgr BulletMgr { get => bulletMgr; set => bulletMgr = value; }
    public Vector3 PosReady { get => posReady; set => posReady = value; }
    public Vector3 PosInQueue { get => posInQueue; set => posInQueue = value; }

    // Start is called before the first frame update
    private void Awake()
    {
        moveRayList = new Stack<Ray2D>();
        bubbleMapMgr = GameObject.Find("BubbleMgr");
        shooter = GameObject.Find("Shooter");
    }
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case BulletState.IN_QUEUE:
                {
                    
                    if (transform.position != posInQueue)
                    {
                        if (Vector3.Distance(transform.position, posInQueue) > moveSpeed * Time.deltaTime)
                            transform.Translate((posInQueue - transform.position).normalized * moveSpeed * Time.deltaTime);
                        else
                            transform.position = posInQueue;
                    }
                }
                break;

            case BulletState.READY_SHOT:
                {
                    if (transform.position != posReady)
                    {
                        if (Vector3.Distance(transform.position, posReady) > moveSpeed * Time.deltaTime)
                            transform.Translate((posReady - transform.position).normalized * moveSpeed * Time.deltaTime);
                        else
                            transform.position = posReady;
                    }
                }
                break;

            case BulletState.MOVING:
                {
                    MoveBubbleBullet();
                }
                break;
        }
    }

    private void MoveBubbleBullet()
    {
        if (!moving)
        {
            if (moveRayList.Count > 0)
            {
                moveDir = moveRayList.Peek().direction;
                moveRayList.Pop();
                if (moveRayList.Count == 0)
                {
                    if (finalPos != new Vector2(shooter.transform.position.x, shooter.transform.position.y))
                        movePos = finalPos;
                    else
                        haveMovePos = false;
                }
                else
                    movePos = moveRayList.Peek().origin;
                moving = true;
            }
        }
        else
        {
            if (haveMovePos)
            {
                if ((new Vector2(transform.position.x, transform.position.y) - movePos).magnitude > moveSpeed * (Time.deltaTime))
                    transform.Translate(moveDir * moveSpeed * Time.deltaTime);
                else
                {
                    transform.position = movePos;
                    if (finalPos != new Vector2(transform.position.x, transform.position.y))
                    {
                        moveDir = moveRayList.Peek().direction;
                        moveRayList.Pop();
                        if (moveRayList.Count == 0)
                        {
                            if (finalPos != new Vector2(shooter.transform.position.x, shooter.transform.position.y))
                                movePos = finalPos;
                            else
                                haveMovePos = false;
                        }
                        else
                            movePos = moveRayList.Peek().origin;
                    }
                    else
                        bubbleCol.GetComponent<Bubble>().HandleColWithBullet(transform, finalPos);
                }
            }
            else
                transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, "moving = " + moving);
    }
#endif
}

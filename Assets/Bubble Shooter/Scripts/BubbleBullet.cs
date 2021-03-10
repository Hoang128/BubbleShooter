using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBullet : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    Stack<Ray2D> moveRayList;
    bool moving = false;
    bool haveMovePos = true;
    bool canCreateBubble = true;
    Vector2 moveDir;
    Vector2 movePos;
    

    public Stack<Ray2D> MoveRayList { get => moveRayList; set => moveRayList = value; }
    public bool CanCreateBubble { get => canCreateBubble; set => canCreateBubble = value; }

    // Start is called before the first frame update
    void Awake()
    {
        moveRayList = new Stack<Ray2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving)
        {
            if (moveRayList.Count > 0)
            {
                moveDir = moveRayList.Peek().direction;
                moveRayList.Pop();
                if (moveRayList.Count == 0)
                {
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
                {
                    transform.Translate(moveDir * moveSpeed *Time.deltaTime);
                }
                else
                {
                    transform.position = movePos;
                    moveDir = moveRayList.Peek().direction;
                    moveRayList.Pop();
                    if (moveRayList.Count == 0)
                    {
                        haveMovePos = false;
                    }
                    else
                        movePos = moveRayList.Peek().origin;
                }
            }
            else
            {
                transform.Translate(moveDir * moveSpeed * Time.deltaTime);
            }
        }
    }
}

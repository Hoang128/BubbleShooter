﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Bubble : MonoBehaviour
{
    enum BubbleType { RED, GREEN, BLUE };

    struct AroundList
    {
        public List<GameObject> bubblesAround;
        public List<Vector2Int> bubbleCoors;
    }

    AroundList aroundList;
    Vector2Int coor;
    GameObject bubbleListMgr;
    [SerializeField] BubbleType type;

    public GameObject BubbleListMgr { get => bubbleListMgr; set => bubbleListMgr = value; }
    private BubbleType Type { get => type; set => type = value; }
    public Vector2Int Coor { get => coor; set => coor = value; }
    public Vector2Int Coor1 { get => coor; set => coor = value; }

    // Start is called before the first frame update
    void Awake()
    {
        coor = new Vector2Int(-1, -1);
        aroundList.bubbleCoors = new List<Vector2Int>();
        aroundList.bubblesAround = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetNearByBubble()
    {
        if (aroundList.bubblesAround.Count > 0)
            aroundList.bubblesAround.Clear();
        if (aroundList.bubbleCoors.Count > 0)
            aroundList.bubbleCoors.Clear();
        if (coor.x < bubbleListMgr.GetComponent<BubbleListMgr>().Range.x - 1)
        {
            aroundList.bubblesAround.Add(bubbleListMgr.GetComponent<BubbleListMgr>().BubbleList[coor.y][coor.x + 1]);
            aroundList.bubbleCoors.Add(new Vector2Int(coor.x + 1, coor.y));
        }
        if (coor.x > 0)
        {
            aroundList.bubblesAround.Add(bubbleListMgr.GetComponent<BubbleListMgr>().BubbleList[coor.y][coor.x - 1]);
            aroundList.bubbleCoors.Add(new Vector2Int(coor.x - 1, coor.y));
        }

        if (coor.y > 0)
        {
            aroundList.bubblesAround.Add(bubbleListMgr.GetComponent<BubbleListMgr>().BubbleList[coor.y - 1][coor.x]);
            aroundList.bubbleCoors.Add(new Vector2Int(coor.x, coor.y - 1));
            if (coor.y % 2 == 0)
            {
                if (coor.x > 0)
                {
                    aroundList.bubblesAround.Add(bubbleListMgr.GetComponent<BubbleListMgr>().BubbleList[coor.y - 1][coor.x - 1]);
                    aroundList.bubbleCoors.Add(new Vector2Int(coor.x - 1, coor.y - 1));
                }
            }
            else
            {
                if (coor.x < bubbleListMgr.GetComponent<BubbleListMgr>().Range.x - 1)
                {
                    aroundList.bubblesAround.Add(bubbleListMgr.GetComponent<BubbleListMgr>().BubbleList[coor.y - 1][coor.x + 1]);
                    aroundList.bubbleCoors.Add(new Vector2Int(coor.x + 1, coor.y - 1));
                }
            }
        }
        if (coor.y < bubbleListMgr.GetComponent<BubbleListMgr>().Range.y - 1)
        {
            aroundList.bubblesAround.Add(bubbleListMgr.GetComponent<BubbleListMgr>().BubbleList[coor.y + 1][coor.x]);
            aroundList.bubbleCoors.Add(new Vector2Int(coor.x, coor.y + 1));
            if (coor.y % 2 == 0)
            {
                if (coor.x > 0)
                {
                    aroundList.bubblesAround.Add(bubbleListMgr.GetComponent<BubbleListMgr>().BubbleList[coor.y + 1][coor.x - 1]);
                    aroundList.bubbleCoors.Add(new Vector2Int(coor.x - 1, coor.y + 1));
                }
            }
            else
            {
                if (coor.x < bubbleListMgr.GetComponent<BubbleListMgr>().Range.x - 1)
                {
                    aroundList.bubblesAround.Add(bubbleListMgr.GetComponent<BubbleListMgr>().BubbleList[coor.y + 1][coor.x + 1]);
                    aroundList.bubbleCoors.Add(new Vector2Int(coor.x + 1, coor.y + 1));
                }
            }
        }
    }

    Vector2Int FindNearestBlankCellToPosition(Vector2 position)
    {
        float minDistance = 1000f;
        Vector2Int nearestCoor = new Vector2Int(-1, -1);
        Vector2 startPos = bubbleListMgr.GetComponent<BubbleListMgr>().StartPos;
        Vector2 space = bubbleListMgr.GetComponent<BubbleListMgr>().BubbleSpace;
        for(int i = 0; i < aroundList.bubblesAround.Count; i++)
        {
            if (aroundList.bubblesAround[i] == null)
            {
                Vector2 blankNearPos = new Vector2(0, 0);
                blankNearPos.x = startPos.x + (space.x * aroundList.bubbleCoors[i].x);
                blankNearPos.y = startPos.y + (space.y * aroundList.bubbleCoors[i].y);
                float distance = Vector2.Distance(position, blankNearPos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCoor = aroundList.bubbleCoors[i];
                    Debug.Log("Found new nearest coor = " + nearestCoor.x + ", " + nearestCoor.y + " & distance = " + distance);
                }
            }
        }

        if (coor.y == (bubbleListMgr.GetComponent<BubbleListMgr>().Range.y - 1))
        {
            Vector2 blankNearPos = new Vector2(startPos.x + space.x * coor.x, startPos.y - space.y * (coor.y + 1));
            float distance = Vector2.Distance(position, blankNearPos);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCoor = new Vector2Int(coor.x, coor.y + 1);
                Debug.Log("Found new nearest coor = " + nearestCoor.x + ", " + nearestCoor.y + " & distance = " + distance);
            }

            if (coor.y % 2 == 0)
            {
                if (coor.x > 0)
                {
                    blankNearPos = new Vector2(startPos.x + space.x * (coor.x - 1), startPos.y - space.y * (coor.y + 1));
                    distance = Vector2.Distance(position, blankNearPos);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestCoor = new Vector2Int(coor.x - 1, coor.y + 1);
                        Debug.Log("Found new nearest coor = " + nearestCoor.x + ", " + nearestCoor.y + " & distance = " + distance);
                    }
                }
            }
            else
            {
                if (coor.x < bubbleListMgr.GetComponent<BubbleListMgr>().Range.x - 1)
                {
                    blankNearPos = new Vector2(startPos.x + space.x * (coor.x + 1), startPos.y - space.y * (coor.y + 1));
                    distance = Vector2.Distance(position, blankNearPos);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestCoor = new Vector2Int(coor.x + 1, coor.y + 1);
                        Debug.Log("Found new nearest coor = " + nearestCoor.x + ", " + nearestCoor.y + " & distance = " + distance);
                    }
                }
            }
        }

        return nearestCoor;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (coor != (new Vector2Int(-1, -1)))
        {
            if (col.GetComponent<BubbleBullet>().CanCreateBubble)
            {
                Vector2Int addCoor = FindNearestBlankCellToPosition(col.transform.position);
                if (addCoor != new Vector2Int(-1, -1))
                {
                    bubbleListMgr.GetComponent<BubbleListMgr>().AddBubbleToList(addCoor, col.gameObject);
                }
                col.GetComponent<BubbleBullet>().CanCreateBubble = false;
            }
        }
    }

    public void OnDrawGizmos()
    {
        float yPos = 0;
        Handles.Label(transform.position + new Vector3(0f, yPos, 0f), coor.x + ", " + coor.y);
        yPos -= 0.1f;
        for (int i = 0; i < aroundList.bubblesAround.Count; i++)
        {
            string type = "0";
            Vector2 nearCoor = aroundList.bubbleCoors[i];
            if (aroundList.bubblesAround[i] != null)
            {
                switch (aroundList.bubblesAround[i].GetComponent<Bubble>().Type)
                {
                    case BubbleType.RED: type = "r"; break;
                    case BubbleType.GREEN: type = "g"; break;
                    case BubbleType.BLUE: type = "b"; break;
                }
            }

            Handles.Label(transform.position + new Vector3(0f, yPos, 0f), "nb = " + nearCoor.x + ", " + nearCoor.y + ", " + type);
            yPos -= 0.1f;
        }
    }
}

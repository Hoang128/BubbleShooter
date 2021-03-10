using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleListMgr : MonoBehaviour
{
    [SerializeField] GameObject[] bubbleTypeList;
    [SerializeField] Vector2 bubbleSpace;
    List<List<GameObject>> bubbleList;
    Vector2 range = new Vector2(0, 0);
    Vector2 startPos = new Vector2(-4, 4);

    public List<List<GameObject>> BubbleList { get => bubbleList; set => bubbleList = value; }
    public Vector2 Range { get => range; set => range = value; }
    public Vector2 BubbleSpace { get => bubbleSpace; set => bubbleSpace = value; }
    public Vector2 StartPos { get => startPos; set => startPos = value; }
    public GameObject[] BubbleTypeList { get => bubbleTypeList; set => bubbleTypeList = value; }

    // Start is called before the first frame update
    void Start()
    {
        bubbleList = new List<List<GameObject>>();

        List<List<string>> bubbleDataList = new List<List<string>>();

        bubbleDataList = LoadBubbleList(2);

        //LoadBubbleList();
        DisplayBubbleList(bubbleDataList);
        FindAllNearByEachBubble();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<List<string>> LoadBubbleList(int level)
    {
        XmlHandler xmlHandler = new XmlHandler();
        List<List<string>> bubbleDataList = new List<List<string>>();
        return xmlHandler.ReadLevel(level);
    }

    void FindAllNearByEachBubble()
    {
        foreach(List<GameObject> rowList in bubbleList)
        {
            foreach (GameObject bubble in rowList)
            {
                if (bubble != null)
                {
                    bubble.GetComponent<Bubble>().GetNearByBubble();
                }
            }
        }
    }

    void UpdateBubbleList()
    {

    }

    public void AddBubbleToList(Vector2Int coor, GameObject obj)
    {
        if (coor.y > (range.y - 1))
        {
            List<GameObject> bubbleRow = new List<GameObject>();
            for (int i = 0; i < range.x; i++)
            {
                bubbleRow.Add(null);
            }
            range.y++;
            bubbleList.Insert(bubbleList.Count, bubbleRow);
        }
        bubbleList[coor.y][coor.x] = obj;
        obj.GetComponent<Bubble>().BubbleListMgr = gameObject;
        obj.GetComponent<Bubble>().Coor = coor;
        Vector2 movePos = new Vector2();
        if (coor.y % 2 == 0)
            movePos.x = startPos.x + coor.x * bubbleSpace.x;
        else
            movePos.x = startPos.x + coor.x * bubbleSpace.x + bubbleSpace.x/2;
        movePos.y = startPos.y - coor.y * bubbleSpace.y;
        obj.transform.position = movePos;

        FindAllNearByEachBubble();
    }

    void DisplayBubbleList(List<List<string>> bubbleDataList)
    {
        Vector2 createPos = startPos;
        Vector2Int coor = new Vector2Int(0, 0);
        foreach (List<string> rowBubbleData in bubbleDataList)
        {
            List<GameObject> rowList = new List<GameObject>();
            foreach (string bubbleData in rowBubbleData)
            {
                if (bubbleData != "0")
                {
                    GameObject bubbleType = null;
                    switch (bubbleData)
                    {
                        case "r":
                            {
                                bubbleType = bubbleTypeList[0];
                            }   
                            break;
                        case "g":
                            {
                                bubbleType = bubbleTypeList[1];
                            }
                            break;
                        case "b":
                            {
                                bubbleType = bubbleTypeList[2];
                            }
                            break;
                    }

                    GameObject bubble = GameObject.Instantiate(bubbleType, createPos, transform.rotation);
                    bubble.GetComponent<Bubble>().BubbleListMgr = gameObject;
                    bubble.GetComponent<Bubble>().Coor = coor;
                    rowList.Add(bubble);
                }
                else
                    rowList.Add(null);

                coor.x++;
                createPos.x += bubbleSpace.x;
            }
            bubbleList.Add(rowList);

            if (coor.y % 2 != 0)
                createPos.x = startPos.x;
            else
                createPos.x = startPos.x + bubbleSpace.x / 2;
            createPos.y -= bubbleSpace.y;
            if (range.x == 0)
                range.x = coor.x;
            coor.x = 0;
            coor.y++;
        }
        if (range.y == 0)
            range.y = coor.y;
        Debug.Log("range y = " + range.y);
    }

    List<GameObject> FindClusterByColor()
    {
        List<GameObject> bubbleList = new List<GameObject>();
        return bubbleList;
    }
}

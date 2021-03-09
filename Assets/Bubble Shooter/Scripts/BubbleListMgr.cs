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

    void AddBubbleToList()
    {

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
    }

    List<GameObject> FindClusterByColor()
    {
        List<GameObject> bubbleList = new List<GameObject>();
        return bubbleList;
    }
}

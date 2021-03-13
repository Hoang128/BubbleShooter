using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMapMgr : MonoBehaviour
{
    enum MapState {IDLE, MOVE, UPDATE};
    MapState mapState = MapState.IDLE;
    public class BubbleData
    {
        Vector2Int gridPos;
        int typeNum;
        int typeKey;
        

        public Vector2Int GridPos { get => gridPos; set => gridPos = value; }
        public int TypeNum { get => typeNum; set => typeNum = value; }
        public int TypeKey { get => typeKey; set => typeKey = value; }
    }
    [SerializeField] GameObject[] bubbleTypeList;
    [SerializeField] Vector2 bubbleSpace;
    [SerializeField] GameObject shooter;
    [SerializeField] float vSpace;
    [SerializeField] float yPosLimit;
    [SerializeField] float mapMoveSpd = 4f;
    List<List<GameObject>> bubbleList;
    Vector2Int range = new Vector2Int(12, 0);
    float currentCeilPosY = 0;
    bool isFirstRowOdd = true;

    public List<List<GameObject>> BubbleList { get => bubbleList; set => bubbleList = value; }
    public Vector2Int Range { get => range; set => range = value; }
    public Vector2 BubbleSpace { get => bubbleSpace; set => bubbleSpace = value; }
    public GameObject[] BubbleTypeList { get => bubbleTypeList; set => bubbleTypeList = value; }

    // Start is called before the first frame update
    void Start()
    {
        bubbleList = new List<List<GameObject>>();

        //Load bubble list (follow the prototype)
        List<BubbleData> bubbleXmlList = new List<BubbleData>();
        bubbleXmlList = LoadBubbleXmlList(1, 4);
        DisplayBubbleList(bubbleXmlList);
        FindAllNearByEachBubble();
        UpdateCurrentCeilPosY();

        transform.position = new Vector2(transform.position.x, currentCeilPosY);
    }

    // Update is called once per frame
    void Update()
    {
        if (mapState == MapState.MOVE)
        {
            if (Mathf.Abs(transform.position.y - currentCeilPosY) > mapMoveSpd * (Time.deltaTime))
            {
                float newPosY = transform.position.y - Mathf.Sign(transform.position.y - currentCeilPosY) * mapMoveSpd * Time.deltaTime;
                transform.position = new Vector2(transform.position.x, newPosY);
            }
            else
                mapState = MapState.IDLE;
        }
    }

    List<BubbleData> LoadBubbleXmlList(int level, int levelSub)
    {
        XmlHandler xmlHandler = new XmlHandler();
        List<BubbleData> bubbleList = xmlHandler.LoadBubbleLevel(level, levelSub);

        //find range y
        int minY = 1000;
        int maxY = 0;
        foreach (BubbleData bubbleData in bubbleList)
        {
            if (bubbleData.GridPos.y > maxY)
                maxY = bubbleData.GridPos.y;
            if (bubbleData.GridPos.y < minY)
                minY = bubbleData.GridPos.y;
        }

        range.y = maxY - minY + 1;

        foreach(BubbleData bubbleData in bubbleList)
        {
            int newPosY = (int)(bubbleData.GridPos.y + (((float)(maxY + minY)) / 2 - bubbleData.GridPos.y) * 2 - minY);
            bubbleData.GridPos = new Vector2Int(bubbleData.GridPos.x, newPosY);
        }

        if (maxY % 2 == 0)
            isFirstRowOdd = false;
        else
            isFirstRowOdd = true;

        return bubbleList;
    }

    public void UpdateCurrentCeilPosY()
    {
        float distance = vSpace + ((range.y - 1) * bubbleSpace.y);
        if (distance > (yPosLimit - shooter.transform.position.y))
            currentCeilPosY = Mathf.Clamp(shooter.transform.position.y + vSpace + ((range.y - 1) * bubbleSpace.y), yPosLimit, Mathf.Infinity);
        else
            currentCeilPosY = yPosLimit;
    }

    public void UpdateBubbleList(Vector2Int coor, GameObject obj)
    {
        mapState = MapState.UPDATE;

        AddBubbleToList(coor, obj);

        List<GameObject> sameColorCluster = new List<GameObject>(FindBubbleClusterSameColor(coor));

        List<GameObject> rearOfsameColorCluster = new List<GameObject>(FindRearBubblesOfSameColorCluster(sameColorCluster));

        if (sameColorCluster.Count >= 3)
            DestroyClusterOfBubble(sameColorCluster);

        DestroyAllIsolatedBubbleCluster(rearOfsameColorCluster);

        for(int i = bubbleList.Count - 1; i >= 0; i--)
        {
            bool stopDeleteRow = false;
            foreach(GameObject bubble in bubbleList[i])
            {
                if (bubble != null)
                {
                    stopDeleteRow = true;
                    break;
                }
            }
            if (stopDeleteRow)
                break;
            else
                bubbleList.Remove(bubbleList[i]);
        }

        range.y = bubbleList.Count;

        UpdateCurrentCeilPosY();

        mapState = MapState.MOVE;


    }

    void DisplayBubbleList(List<BubbleData> bubbleDataList)
    {
        for(int i = 0; i < range.y; i++)
        {
            bubbleList.Add(new List<GameObject>());
        }

        bool isOddRow = isFirstRowOdd;

        foreach(List<GameObject> bubbleRow in BubbleList)
        {
            if (isOddRow)
            {
                for (int i = 0; i < range.x - 1; i++)
                {
                    bubbleRow.Add(null);
                }
                isOddRow = false;
            }
            else
            {
                for (int i = 0; i < range.x; i++)
                {
                    bubbleRow.Add(null);
                }
                isOddRow = true;
            }
        }

        isOddRow = isFirstRowOdd;
        foreach(BubbleData bubbleData in bubbleDataList)
        {
            Vector2 createPos = new Vector2();

            float spacing = 0f;
            if (isFirstRowOdd)
            {
                if (bubbleData.GridPos.y % 2 == 0)
                    spacing = bubbleSpace.x / 2;
            }
            else
            {
                if (bubbleData.GridPos.y % 2 == 1)
                    spacing = bubbleSpace.x / 2;
            }

            GameObject bubbleInstance = bubbleTypeList[0];
            if (bubbleData.TypeNum < bubbleTypeList.Length)
                bubbleInstance = bubbleTypeList[bubbleData.TypeNum];

            createPos.x = transform.position.x + bubbleData.GridPos.x * bubbleSpace.x + spacing;
            createPos.y = transform.position.y - bubbleData.GridPos.y * bubbleSpace.y;
            bubbleList[bubbleData.GridPos.y][bubbleData.GridPos.x] = Instantiate(bubbleInstance, createPos, transform.rotation);
            bubbleList[bubbleData.GridPos.y][bubbleData.GridPos.x].GetComponent<Bubble>().BubbleMapMgr = gameObject;
            bubbleList[bubbleData.GridPos.y][bubbleData.GridPos.x].GetComponent<Bubble>().Coor = bubbleData.GridPos;
            bubbleList[bubbleData.GridPos.y][bubbleData.GridPos.x].GetComponent<Bubble>().transform.parent = transform;
        }
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


    public void AddBubbleToList(Vector2Int coor, GameObject obj)
    {
        Destroy(obj);
        int currentRangeX = range.x;
        if (bubbleList[coor.y - 1].Count == currentRangeX)
            currentRangeX = range.x - 1;
        if (coor.y > (range.y - 1))
        {
            List<GameObject> bubbleRow = new List<GameObject>();
            
            for (int i = 0; i < currentRangeX; i++)
            {
                bubbleRow.Add(null);
            }
            range.y++;
            bubbleList.Insert(bubbleList.Count, bubbleRow);
        }
        Vector2 movePos = new Vector2();
        if (currentRangeX == range.x)
            movePos.x = transform.position.x + coor.x * bubbleSpace.x;
        else
            movePos.x = transform.position.x + coor.x * bubbleSpace.x + bubbleSpace.x / 2;
        movePos.y = transform.position.y - coor.y * bubbleSpace.y;
        
        GameObject newBubble = Instantiate(bubbleTypeList[0], movePos, transform.rotation);
        bubbleList[coor.y][coor.x] = newBubble;
        newBubble.GetComponent<Bubble>().BubbleMapMgr = gameObject;
        newBubble.GetComponent<Bubble>().Coor = coor;
        newBubble.transform.parent = transform;

        FindAllNearByEachBubble();
    }

    void DestroyClusterOfBubble(List<GameObject> bubbleCluster)
    {
        foreach (GameObject bubble in bubbleCluster)
        {
            Vector2Int bubbleCoor = bubble.GetComponent<Bubble>().Coor;
            GameObject.Destroy(bubbleList[bubbleCoor.y][bubbleCoor.x]);
            bubbleList[bubbleCoor.y][bubbleCoor.x] = null;
        }

        FindAllNearByEachBubble();
    }

    void DestroyAllIsolatedBubbleCluster(List<GameObject> inputBubbleList)
    {
        
        while(inputBubbleList.Count > 0)
        {
            GameObject markBubble = inputBubbleList[0];
            inputBubbleList.Remove(inputBubbleList[0]);
            List<GameObject> bubbleCluster = new List<GameObject>(FindBubbleCluster(markBubble.GetComponent<Bubble>().Coor));

            if (inputBubbleList.Count > 0)
            {
                List<GameObject> removeList = new List<GameObject>();
                foreach (GameObject bubble in bubbleCluster)
                {
                    foreach (GameObject inputBubble in inputBubbleList)
                    {
                        if (bubble.GetComponent<Bubble>().Coor == inputBubble.GetComponent<Bubble>().Coor)
                            removeList.Add(inputBubble);
                    }
                }

                foreach(GameObject removeItem in removeList)
                {
                    inputBubbleList.Remove(removeItem);
                }
            }

            if (IsClusterIsolatedByCoor(bubbleCluster))
                DestroyClusterOfBubble(bubbleCluster);
        }
    }

    List<GameObject> FindBubbleCluster(Vector2Int position)
    {
        List<GameObject> bubbleCluster = new List<GameObject>();

        GameObject rootBubble = bubbleList[position.y][position.x];

        Stack<GameObject> bubbleStack = new Stack<GameObject>();

        bubbleCluster.Add(rootBubble);
        bubbleStack.Push(rootBubble);

        while (bubbleStack.Count > 0)
        {
            GameObject bubbleFinder = bubbleStack.Pop();
            List<GameObject> nearbyBubbleFinder;
            nearbyBubbleFinder = new List<GameObject>(bubbleFinder.GetComponent<Bubble>().AroundBubbleList.bubblesAround);

            foreach (GameObject bubble in nearbyBubbleFinder)
            {
                if (bubble != null)
                {
                    if (bubbleCluster.Count > 0)
                    {
                        bool duplicated = false;
                        foreach (GameObject bubbleInCluster in bubbleCluster)
                        {
                            if (bubble.GetComponent<Bubble>().Coor == bubbleInCluster.GetComponent<Bubble>().Coor)
                            {
                                duplicated = true;
                                break;
                            }
                        }
                        if (!duplicated)
                        {
                            bubbleCluster.Add(bubble);
                            bubbleStack.Push(bubble);
                        }
                    }
                    else
                    {
                        bubbleCluster.Add(bubble);
                        bubbleStack.Push(bubble);
                    }
                }
            }
        }

        return bubbleCluster;
    }

    List<GameObject> FindBubbleClusterSameColor(Vector2Int position)
    {
        Bubble.BubbleType rootType = bubbleList[position.y][position.x].GetComponent<Bubble>().TypeBubble;

        List<GameObject> bubbleCluster = new List<GameObject>();

        GameObject rootBubble = bubbleList[position.y][position.x];

        Stack<GameObject> bubbleStack = new Stack<GameObject>();

        bubbleCluster.Add(rootBubble);
        bubbleStack.Push(rootBubble);

        while(bubbleStack.Count > 0)
        {
            GameObject bubbleFinder = bubbleStack.Pop();
            List<GameObject> nearbyBubbleFinder;
            nearbyBubbleFinder = new List<GameObject>(bubbleFinder.GetComponent<Bubble>().AroundBubbleList.bubblesAround);

            foreach (GameObject bubble in nearbyBubbleFinder)
            {
                if (bubble != null)
                {
                    if (bubbleCluster.Count > 0)
                    {
                        bool duplicated = false;
                        foreach (GameObject bubbleInCluster in bubbleCluster)
                        {
                            if (bubble.GetComponent<Bubble>().Coor == bubbleInCluster.GetComponent<Bubble>().Coor)
                            {
                                duplicated = true;
                                break;
                            }
                        }
                        if (!duplicated)
                        {
                            if (bubble.GetComponent<Bubble>().TypeBubble == rootType)
                            {
                                bubbleCluster.Add(bubble);
                                bubbleStack.Push(bubble);
                            }
                        }
                    }
                    else
                    {
                        if (bubble.GetComponent<Bubble>().TypeBubble == rootType)
                        {
                            bubbleCluster.Add(bubble);
                            bubbleStack.Push(bubble);
                        }
                    }
                }
            }
        }
        
        return bubbleCluster;
    }

    bool IsClusterIsolatedByCoor(List<GameObject> bubbleCluster)
    {
        foreach(GameObject bubble in bubbleCluster)
        {
            if (bubble.GetComponent<Bubble>().Coor.y == 0)
                return false;
        }
        return true;
    }

    List<GameObject> FindRearBubblesOfSameColorCluster(List<GameObject> sameColBubbleCluster)
    {
        List<GameObject> rearBubblesList = new List<GameObject>();
        Bubble.BubbleType rootType = sameColBubbleCluster[0].GetComponent<Bubble>().TypeBubble;

        foreach (GameObject bubbleFinder in sameColBubbleCluster)
        {
            List<GameObject> nearbyBubbleFinder;
            nearbyBubbleFinder = new List<GameObject>(bubbleFinder.GetComponent<Bubble>().AroundBubbleList.bubblesAround);
            foreach(GameObject bubble in nearbyBubbleFinder)
            {
                if (bubble != null)
                {
                    if (rootType != bubble.GetComponent<Bubble>().TypeBubble)
                    {
                        bool duplicated = false;
                        foreach (GameObject bubbleTaken in rearBubblesList)
                        {
                            if (bubble.GetComponent<Bubble>().Coor == bubbleTaken.GetComponent<Bubble>().Coor)
                            {
                                duplicated = true;
                                break;
                            }
                        }
                        if (!duplicated)
                            rearBubblesList.Add(bubble);
                    }
                }
            }
        }
        Debug.Log("Done!");

        return rearBubblesList;
    }
}

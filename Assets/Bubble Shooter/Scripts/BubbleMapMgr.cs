using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BubbleMapMgr : MonoBehaviour
{
    public enum MapState {IDLE, MOVE, UPDATE};
    MapState state = MapState.IDLE;
    public class BubbleData
    {
        Vector2Int gridPos;
        int typeNum;
        int typeKey;
        int randomType;

        public Vector2Int GridPos { get => gridPos; set => gridPos = value; }
        public int TypeNum { get => typeNum; set => typeNum = value; }
        public int TypeKey { get => typeKey; set => typeKey = value; }
        public int RandomType { get => randomType; set => randomType = value; }
    }
    [SerializeField] GameObject[] bubbleTypeList;
    [SerializeField] GameObject keyObject;
    [SerializeField] Vector2 bubbleSpace;
    [SerializeField] GameObject shooter;
    [SerializeField] GameObject objGameplayMgr;
    [SerializeField] float vSpace;
    [SerializeField] float yPosLimit;
    [SerializeField] float mapMoveSpd = 4f;
    List<List<GameObject>> bubbleList;
    Vector2Int range = new Vector2Int(12, 0);
    float currentCeilPosY = 0;
    bool isFirstRowOdd = true;
    BulletMgr bulletMgr;
    int[] rColorEnable;
    int[] rColorSet;
    GameplayMgr gameplayMgr;

    public List<List<GameObject>> BubbleList { get => bubbleList; set => bubbleList = value; }
    public Vector2Int Range { get => range; set => range = value; }
    public Vector2 BubbleSpace { get => bubbleSpace; set => bubbleSpace = value; }
    public GameObject[] BubbleTypeList { get => bubbleTypeList; set => bubbleTypeList = value; }
    public GameObject KeyObject { get => keyObject; set => keyObject = value; }
    public MapState State { get => state; set => state = value; }
    public GameplayMgr GameplayMgr { get => gameplayMgr; set => gameplayMgr = value; }

    // Start is called before the first frame update
    void Start()
    {
        bubbleList = new List<List<GameObject>>();
        rColorEnable = new int[6];
        for (int i = 0; i < 6; i++)
        {
            rColorEnable[i] = 0;
        }

        gameplayMgr = objGameplayMgr.GetComponent<GameplayMgr>();

        //Load bubble list (follow file xml)
        bulletMgr = GameObject.Find("BulletMgr").GetComponent<BulletMgr>();
        rColorEnable = LoadRandomColorList(1, 2);
        List<BubbleData> bubbleXmlList = new List<BubbleData>();
        bubbleXmlList = LoadBubbleXmlList(1, 2);
        DisplayBubbleList(bubbleXmlList);
        FindAllNearByEachBubble();
        UpdateCurrentCeilPosY();

        transform.position = new Vector2(transform.position.x, currentCeilPosY);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == MapState.MOVE)
        {
            if (Mathf.Abs(transform.position.y - currentCeilPosY) > mapMoveSpd * (Time.deltaTime))
            {
                float newPosY = transform.position.y - Mathf.Sign(transform.position.y - currentCeilPosY) * mapMoveSpd * Time.deltaTime;
                transform.position = new Vector2(transform.position.x, newPosY);
            }
            else
                state = MapState.IDLE;
        }
    }

    int[] LoadRandomColorList(int level, int levelSub)
    {
        XmlHandler xmlHandler = new XmlHandler();
        int[] rColorEnable = xmlHandler.LoadLevelRandomColor(level, levelSub);
        return rColorEnable;
    }

    void SetRandomColor()
    {
        if (rColorSet.Length > 0)
        {
            List<int> colorList = new List<int>();

            for (int i = 0; i < rColorEnable.Length; i++)
            {
                if (rColorEnable[i] == 1)
                    colorList.Add(i);
            }

            if (colorList.Count > 0)
            {
                colorList = colorList.OrderBy(x => Random.value).ToList();
                for (int i = 0; i < rColorSet.Length; i++)
                {
                    if (colorList.Count >= i)
                        rColorSet[i] = colorList[i];
                    else
                        rColorSet[i] = colorList[0];
                }
            }
        }
    }

    List<BubbleData> LoadBubbleXmlList(int level, int levelSub)
    {
        XmlHandler xmlHandler = new XmlHandler();
        List<BubbleData> bubbleList = xmlHandler.LoadBubbleLevelMap(level, levelSub);

        //find max color set
        int maxColorSet = 0;
        uint bird = 0;

        //find range y
        int minY = 1000;
        int maxY = 0;
        foreach (BubbleData bubbleData in bubbleList)
        {
            if (bubbleData.GridPos.y > maxY)
                maxY = bubbleData.GridPos.y;
            if (bubbleData.GridPos.y < minY)
                minY = bubbleData.GridPos.y;
            if (bubbleData.RandomType > maxColorSet)
                maxColorSet = bubbleData.RandomType;
            if (bubbleData.TypeKey > 0)
                bird++;
        }

        rColorSet = new int[maxColorSet + 1];
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

        gameplayMgr.BirdInTrap = bird;
        gameplayMgr.BirdIndexer.GetMaxBird(bird);
        gameplayMgr.BirdIndexer.UpdateBirdIndexerText(bird);

        return bubbleList;
    }

    public void UpdateCurrentCeilPosY()
    {
        float distance = vSpace + ((range.y - 1) * bubbleSpace.y);
        if (distance > (yPosLimit - shooter.transform.position.y))
            currentCeilPosY = Mathf.Clamp(shooter.transform.position.y + vSpace + ((range.y - 1) * bubbleSpace.y), yPosLimit, Mathf.Infinity);
        else
            currentCeilPosY = yPosLimit;
        FindAllNearByEachBubble();
    }

    public void UpdateBubbleList(Vector2Int coor, GameObject obj)
    {
        state = MapState.UPDATE;

        AddBubbleToList(coor, obj);

        List<GameObject> sameColorCluster = new List<GameObject>(FindBubbleClusterSameColor(coor));

        List<GameObject> rearOfsameColorCluster = new List<GameObject>(FindRearBubblesOfSameColorCluster(sameColorCluster));

        if (sameColorCluster.Count >= 3)
        {
            DestroyClusterOfBubble(sameColorCluster, false);

            foreach (GameObject bubble in rearOfsameColorCluster)
            {
                bubble.GetComponent<Bubble>().GetNearByBubble();
            }

            DestroyAllIsolatedBubbleCluster(rearOfsameColorCluster);
        }

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

        gameplayMgr.ScoreIndexer.UpdateScoreIndexerText(gameplayMgr.Score);
        gameplayMgr.UpdateGameplayState();

        if (gameplayMgr.State == GameplayMgr.GameplayState.WIN)
            Debug.Log("PLAYER WIN!");
        else if (gameplayMgr.State == GameplayMgr.GameplayState.LOSE)
            Debug.Log("PLAYER LOSE!");
        else
            state = MapState.MOVE;
    }

    void DisplayBubbleList(List<BubbleData> bubbleDataList)
    {
        SetRandomColor();

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
            else if (bubbleData.TypeNum == 27)
            {
                bubbleInstance = bubbleTypeList[rColorSet[bubbleData.RandomType]];
            }

            createPos.x = transform.position.x + bubbleData.GridPos.x * bubbleSpace.x + spacing;
            createPos.y = transform.position.y - bubbleData.GridPos.y * bubbleSpace.y;
            bubbleList[bubbleData.GridPos.y][bubbleData.GridPos.x] = Instantiate(bubbleInstance, createPos, transform.rotation);
            bubbleList[bubbleData.GridPos.y][bubbleData.GridPos.x].GetComponent<Bubble>().BubbleMapMgr = this;
            bubbleList[bubbleData.GridPos.y][bubbleData.GridPos.x].GetComponent<Bubble>().Coor = bubbleData.GridPos;
            bubbleList[bubbleData.GridPos.y][bubbleData.GridPos.x].GetComponent<Bubble>().transform.parent = transform;
            bubbleList[bubbleData.GridPos.y][bubbleData.GridPos.x].GetComponent<Bubble>().KeyType = bubbleData.TypeKey;
        }

        bulletMgr.InitBulletPool();
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
        GameObject newBubbleType = bubbleTypeList[(int)obj.GetComponent<BubbleBullet>().BulletType];
        

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
        
        GameObject newBubble = Instantiate(newBubbleType, movePos, transform.rotation);
        bubbleList[coor.y][coor.x] = newBubble;
        newBubble.GetComponent<Bubble>().BubbleMapMgr = this;
        newBubble.GetComponent<Bubble>().Coor = coor;
        newBubble.transform.parent = transform;

        newBubble.GetComponent<Bubble>().GetNearByBubble();
        List<GameObject> newBubbleNearByList = newBubble.GetComponent<Bubble>().AroundBubbleList.bubblesAround;
        foreach (GameObject bubble in newBubbleNearByList)
        {
            if (bubble != null)
            {
                bubble.GetComponent<Bubble>().GetNearByBubble();
            }
        }
    }

    void DestroyClusterOfBubble(List<GameObject> bubbleCluster, bool isIsolated)
    {
        foreach (GameObject bubble in bubbleCluster)
        {
            Vector2Int bubbleCoor = bubble.GetComponent<Bubble>().Coor;
            Bubble.BubbleState state;
            if (isIsolated)
                state = Bubble.BubbleState.FALL;
            else
                state = Bubble.BubbleState.EXPLODE;
            bubbleList[bubbleCoor.y][bubbleCoor.x].GetComponent<Bubble>().ChangeBubbleState(state);
            bubbleList[bubbleCoor.y][bubbleCoor.x].GetComponent<Bubble>().FreeBird();
            gameplayMgr.BirdIndexer.UpdateBirdIndexerText(gameplayMgr.BirdInTrap);
            GameObject.Destroy(bubbleList[bubbleCoor.y][bubbleCoor.x]);
            bubbleList[bubbleCoor.y][bubbleCoor.x] = null;
        }
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
                DestroyClusterOfBubble(bubbleCluster, true);
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
    }
#endif
}

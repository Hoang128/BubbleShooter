using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BulletMgr : MonoBehaviour
{
    [SerializeField] GameObject[] bulletTypes;
    [SerializeField] GameObject indexer;

    BubbleMapMgr bubbleMapMgr;
    GameObject bulletShot;
    GameObject bubbleMgr;
    Queue<GameObject> bulletQueue;
    List<int> bubbleColorPool;
    int bulletLeft = 30;

    public GameObject BulletShot { get => bulletShot; set => bulletShot = value; }
    public Queue<GameObject> BulletQueue { get => bulletQueue; set => bulletQueue = value; }
    public int BulletLeft { get => bulletLeft; set => bulletLeft = value; }

    // Start is called before the first frame update
    void Awake()
    {
        bubbleMapMgr = GameObject.Find("BubbleMgr").GetComponent<BubbleMapMgr>();
        bubbleMgr = GameObject.Find("BubbleMgr");
        bubbleColorPool = new List<int>();
        bulletQueue = new Queue<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitBulletPool()
    {
        UpdateBubbleColorPool();
        GameObject bullet1 = GameObject.Instantiate(bulletTypes[bubbleColorPool[0]], transform.position + new Vector3(-4.83f, -2f, -10f), transform.rotation);
        bullet1.GetComponent<BubbleBullet>().BulletMgr = this;
        bullet1.GetComponent<BubbleBullet>().PosInQueue = bullet1.transform.position;
        bullet1.GetComponent<BubbleBullet>().PosReady = transform.position + new Vector3(0f, 0f, -10f);
        bulletQueue.Enqueue(bullet1);

        UpdateBubbleColorPool();
        GameObject bullet2 = GameObject.Instantiate(bulletTypes[bubbleColorPool[0]], transform.position + new Vector3(-4.83f, -2f, -10f), transform.rotation);
        bullet2.GetComponent<BubbleBullet>().BulletMgr = this;
        bullet2.GetComponent<BubbleBullet>().PosInQueue = bullet2.transform.position;
        bullet2.GetComponent<BubbleBullet>().PosReady = transform.position + new Vector3(0f, 0f, -10f);
        bulletQueue.Enqueue(bullet2);

        bulletShot = bulletQueue.Dequeue();
        bulletShot.GetComponent<SpriteRenderer>().sortingOrder = 1;
        bulletShot.GetComponent<BubbleBullet>().State = BulletState.READY_SHOT;

        indexer.GetComponent<BulletNumberIndexer>().UpdateBulletNumber(bulletLeft);
    }

    void UpdateBulletPool()
    {
        if (bulletLeft > 2)
        {
            GameObject bullet = GameObject.Instantiate(bulletTypes[bubbleColorPool[0]], transform.position + new Vector3(-4.83f, -2f, -10f), transform.rotation);
            bullet.GetComponent<BubbleBullet>().BulletMgr = this;
            bullet.GetComponent<BubbleBullet>().PosInQueue = bullet.transform.position;
            bullet.GetComponent<BubbleBullet>().PosReady = transform.position + new Vector3(0f, 0f, -10f);
            bulletQueue.Enqueue(bullet);
        }

        if (bulletQueue.Count > 0)
        {
            bulletShot = bulletQueue.Dequeue();
            bulletShot.GetComponent<SpriteRenderer>().sortingOrder = 1;
            bulletShot.GetComponent<BubbleBullet>().State = BulletState.READY_SHOT;
        }

        else
            bulletShot = null;
    }
    
    public void LaunchBubble(Stack<Ray2D> aimRayStack, Vector2 finalShotPos, Collider2D bubbleCol)
    {
        BubbleBullet bullet = bulletShot.GetComponent<BubbleBullet>();
        bullet.MoveRayList = new Stack<Ray2D>(aimRayStack);
        bullet.FinalPos = finalShotPos;
        bullet.BubbleCol = bubbleCol;
        bullet.State = BulletState.MOVING;
       

        if (bulletLeft > 1)
        {
            
            UpdateBubbleColorPool();
            bubbleColorPool = bubbleColorPool.OrderBy(x => Random.value).ToList();
            UpdateBulletPool();
        }

        if (bulletLeft > 0)
            bulletLeft--;

        indexer.GetComponent<BulletNumberIndexer>().UpdateBulletNumber(bulletLeft);
    }

    public void ChangeBubble()
    {
        if (bulletQueue.Count > 0)
        {
            GameObject objBubbleTemp = bulletShot;
            objBubbleTemp.GetComponent<BubbleBullet>().State = BulletState.IN_QUEUE;
            objBubbleTemp.GetComponent<SpriteRenderer>().sortingOrder = 0;

            bulletShot = bulletQueue.Dequeue();
            bulletShot.GetComponent<BubbleBullet>().State = BulletState.READY_SHOT;
            bulletShot.GetComponent<SpriteRenderer>().sortingOrder = 1;
            bulletQueue.Enqueue(objBubbleTemp);
        }
    }

    void UpdateBubbleColorPool()
    {
        bubbleColorPool.Clear();

        List<List<GameObject>> bubbleMap = bubbleMapMgr.BubbleList;

        foreach(List<GameObject> rowBubble in bubbleMap)
        {
            foreach(GameObject bubble in rowBubble)
            {
                if (bubble != null)
                {
                    Bubble.BubbleType bubbleType = bubble.GetComponent<Bubble>().TypeBubble;
                    if (bubbleColorPool.Count > 0)
                    {
                        foreach (int type in bubbleColorPool)
                        {
                            if (((int)bubbleType) == type)
                            {
                                continue;
                            }
                        }

                        bubbleColorPool.Add((int)bubbleType);
                    }
                    else
                        bubbleColorPool.Add((int)bubbleType);
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

    }
#endif
}

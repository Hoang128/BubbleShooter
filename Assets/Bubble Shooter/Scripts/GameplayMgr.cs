using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameplayMgr : MonoBehaviour
{
    public enum GameplayState { WIN, LOSE, CONTINUE, RESULT}

    GameplayState state = GameplayState.CONTINUE;
    List<uint> starScore;
    uint birdInTrap;
    uint score = 0;
    BulletMgr bulletMgr;
    [SerializeField] GameObject bulletMgrObj;
    [SerializeField] uint scoreFall;
    [SerializeField] uint scoreExplode;
    [SerializeField] uint scoreBird;
    [SerializeField] uint scoreBubbleLeft;
    [SerializeField] GameObject birdIndexerObj;
    [SerializeField] GameObject scoreIndexerObj;
    BirdIndexer birdIndexer;
    ScoreIndexer scoreIndexer;
    

    public List<uint> StarScore { get => starScore; set => starScore = value; }
    public uint BirdInTrap { get => birdInTrap; set => birdInTrap = value; }
    public GameplayState State { get => state; set => state = value; }
    public uint Score { get => score; set => score = value; }
    public uint ScoreFall { get => scoreFall; set => scoreFall = value; }
    public uint ScoreExplode { get => scoreExplode; set => scoreExplode = value; }
    public uint ScoreFall1 { get => scoreFall; set => scoreFall = value; }
    public uint ScoreExplode1 { get => scoreExplode; set => scoreExplode = value; }
    public uint ScoreBird { get => scoreBird; set => scoreBird = value; }
    public BirdIndexer BirdIndexer { get => birdIndexer; set => birdIndexer = value; }
    public ScoreIndexer ScoreIndexer { get => scoreIndexer; set => scoreIndexer = value; }

    // Start is called before the first frame update
    private void Awake()
    {
        bulletMgr = bulletMgrObj.GetComponent<BulletMgr>();
        birdIndexer = birdIndexerObj.GetComponent<BirdIndexer>();
        scoreIndexer = scoreIndexerObj.GetComponent<ScoreIndexer>();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case GameplayState.WIN:
            case GameplayState.LOSE:
                {
                    //pass through counting bubble phase :v
                    state = GameplayState.RESULT;
                }   break;
            case GameplayState.RESULT:
                {
                    
                }   break;
        }
    }

    public void UpdateScore(uint scoreIncrease)
    {

        score += scoreIncrease;
    }

    public void UpdateGameplayState()
    {
        if (birdInTrap == 0)
        {
            state = GameplayState.WIN;
            return;
        }
        else
        {
            if (bulletMgr.BulletLeft == 0)
                state = GameplayState.LOSE;
            else
                state = GameplayState.CONTINUE;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(new Vector3(0f, -19.9f, 0f), "score = " + score);
    }
#endif
}

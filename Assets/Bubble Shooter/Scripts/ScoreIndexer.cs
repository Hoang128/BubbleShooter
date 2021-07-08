using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIndexer : MonoBehaviour
{
    Text scoreNumberIndexer;

    // Start is called before the first frame update
    void Awake()
    {
        scoreNumberIndexer = GetComponent<UnityEngine.UI.Text>();
        scoreNumberIndexer.text = "Score: 0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoreIndexerText(uint newScore)
    {
        scoreNumberIndexer.text = "Score: " + newScore;
    }
}

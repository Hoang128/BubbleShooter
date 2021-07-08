using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdIndexer : MonoBehaviour
{
    Text birdNumberIndexer;
    uint birdMax;

    public uint BirdMax { get => birdMax; set => birdMax = value; }

    // Start is called before the first frame update
    void Awake()
    {
        birdNumberIndexer = GetComponent<UnityEngine.UI.Text>();
        birdNumberIndexer.text = "0/0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetMaxBird(uint maxBirdNumber)
    {
        birdMax = maxBirdNumber;
    }

    public void UpdateBirdIndexerText(uint newBirdNumber)
    {
        birdNumberIndexer.text =  (BirdMax - newBirdNumber) + "/" + BirdMax;
    }
}

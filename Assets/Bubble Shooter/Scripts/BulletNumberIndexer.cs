using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletNumberIndexer : MonoBehaviour
{
    Text bulletNumberIndexer;

    // Start is called before the first frame update
    void Awake()
    {
        bulletNumberIndexer = GetComponent<UnityEngine.UI.Text>();
        bulletNumberIndexer.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBulletNumber(int bulletNumber)
    {
        bulletNumberIndexer.text = "" + bulletNumber;
    }
}

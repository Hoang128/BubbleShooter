using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBubbleButton : MonoBehaviour
{
    [SerializeField] int rotateSpeed;
    int rotate = -1;
    BulletMgr bulletMgr;
    // Start is called before the first frame update
    void Start()
    {
        bulletMgr = GameObject.Find("BulletMgr").GetComponent<BulletMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((rotate >= 0) && (rotate < 180))
        {
            transform.eulerAngles -= new Vector3(0, 0, rotateSpeed);
            rotate += rotateSpeed;
        }
        else if (rotate >= 180)
            rotate = -1;
    }

    public void EnableAutoRotate()
    {
        if (bulletMgr.BulletQueue.Count > 0)
            rotate = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    enum BirdState {IN_TRAP, FLYING, STAND }

    private BirdState state = BirdState.IN_TRAP;
    [SerializeField] Vector2 landPos;
    [SerializeField] float flySpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case BirdState.IN_TRAP:
                {

                }
                break;
            case BirdState.FLYING:
                {
                    if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), landPos) > flySpeed * Time.deltaTime)
                    {
                        transform.Translate((landPos - new Vector2(transform.position.x, transform.position.y)).normalized * flySpeed * Time.deltaTime);
                    }
                    else
                        state = BirdState.STAND;
                }
                break;
            case BirdState.STAND:
                {

                }
                break;
        }
    }

    public void SetBirdStateToFly()
    {
        state = BirdState.FLYING;
        landPos += new Vector2(Random.Range(-2.5f, 2.5f), Random.Range(-1.5f, 1.5f));
    }
}

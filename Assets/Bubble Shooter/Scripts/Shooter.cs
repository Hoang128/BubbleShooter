using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] float maxAngle;
    [SerializeField] float space;

    Stack<Ray2D> aimLineList;
    Vector3 lastMouseCoordinate = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        aimLineList = new Stack<Ray2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;

            if (mouseDelta != new Vector3(0, 0, 0))
            {
                updateAimLineList();
            }
            lastMouseCoordinate = Input.mousePosition;
            Vector2 touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void updateAimLineList()
    {
        aimLineList.Clear();
        Vector2 touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
        Ray2D aimRay = new Ray2D(transform.position, touchPoint - pos2D);

        if (Vector2.Angle(Vector2.up, aimRay.direction) > maxAngle)
            return;

        bool aiming = true;
        aimLineList.Push(aimRay);

        while (aiming)
        {
            RaycastHit2D aimHit = Physics2D.Raycast(aimLineList.Peek().origin, aimLineList.Peek().direction);
            if (aimHit && (aimHit.collider.tag == "Wall"))
            {
                Ray2D nextAimRay = new Ray2D(aimHit.point - aimLineList.Peek().direction.normalized * space, Vector2.Reflect(aimLineList.Peek().direction, aimHit.normal));

                aimLineList.Push(nextAimRay);
            }
            else
                aiming = false;
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Ray2D aimRay in aimLineList)
            Gizmos.DrawRay(aimRay.origin, aimRay.direction);
    }
}

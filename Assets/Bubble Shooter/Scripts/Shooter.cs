﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] GameObject[] bulletTypes;
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
            if (!GameObject.FindGameObjectWithTag("Bullet"))
            {
                Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;

                if (mouseDelta != new Vector3(0, 0, 0))
                {
                    updateAimLineList();
                }
                lastMouseCoordinate = Input.mousePosition;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!GameObject.FindGameObjectWithTag("Bullet"))
            {
                updateAimLineList();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (aimLineList.Count != 0)
            {
                GameObject bullet = Instantiate(bulletTypes[0], transform.position, new Quaternion(0, 0, 0, 0));
                bullet.GetComponent<BubbleBullet>().MoveRayList = new Stack<Ray2D>(aimLineList);
                aimLineList.Clear();
            }
        }
    }

    void updateAimLineList()
    {
        aimLineList.Clear();
        Vector2 touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
        Ray2D aimRay = new Ray2D(transform.position, touchPoint - pos2D);

        float aimAngle = Vector2.Angle(Vector2.up, aimRay.direction);

        if (aimAngle > maxAngle)
            return;

        bool aiming = true;
        aimLineList.Push(aimRay);
        
        if (transform.position.x <= touchPoint.x)
            transform.eulerAngles = new Vector3(0, 0, -aimAngle);
        else
            transform.eulerAngles = new Vector3(0, 0, aimAngle);

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

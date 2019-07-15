using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LockedCircularMovement : MonoBehaviour
{
    #if UNITY_EDITOR
    //float angle = 6f;
    public GameObject interactive;
    public float circlePosition;
    private float prevCirclePosition;

    public float heightPos;
    private float prevHeight;

    //private Vector3 prevPos;
    //private Vector3 currentPos;
    //private float distance;

    private Vector3 updatePosition;
    private float circleX;
    private float circleZ;
    private float circleAngleRadian;

    //private void Start()
    //{
    //}
    private void Start()
    {
        circlePosition = 0;
        prevCirclePosition = 0;

        heightPos = 0;
        prevHeight = 0;
    }

    void Update()
    {

        if (prevCirclePosition != circlePosition)
        {
            circleAngleRadian = circlePosition * 6f * Mathf.Deg2Rad;
            circleX = 0 + (50 * Mathf.Cos(circleAngleRadian));
            circleZ = 0 + (50 * Mathf.Sin(circleAngleRadian));

            //print("CircleAngleDegree = " + circleAngleDegree.ToString() + "\n");
            //print("Degree x = " + circleX.ToString() + "\n");
            //print("Degree z = " + circleZ.ToString() + "\n");

            updatePosition.x = circleX;
            updatePosition.z = circleZ;
            updatePosition.y = transform.position.y;

            transform.position = updatePosition;

            transform.rotation = Quaternion.identity;
            transform.Rotate(90f, 0f, 0f);
            transform.Rotate(0f, 0f, (circlePosition * 6f), Space.Self);
            prevCirclePosition = circlePosition;
        }

        if (prevHeight != heightPos)
        {
            updatePosition.y = 18 + (heightPos * 6);
            updatePosition.x = transform.position.x;
            updatePosition.z = transform.position.z;

            transform.position = updatePosition;
            prevHeight = heightPos;
        }
    }
    #endif
}

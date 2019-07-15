using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 target;
    private Vector3 target_y_value;

    // Start is called before the first frame update
    void Start()
    {
        target = player.transform.position;
        target_y_value = new Vector3(0, target.y, 0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        target = player.transform.position;
        target_y_value = target;
        target_y_value.x = 0;
        target_y_value.z = 0;
        target_y_value.y = target_y_value.y + 10;
        //transform.position = player.transform.position + offset;
        transform.LookAt(target);
        transform.position += (target_y_value - transform.position) * Time.deltaTime;
    }
}
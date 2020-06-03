using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    /* The LookAt Camera
     * 
     * public GameObject player;

    void LateUpdate()
    {
        transform.LookAt(player.transform);
    }*/


    /* //The Dungeon Camera
    public GameObject target;
    public float damping = 1;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - target.transform.position;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.transform.position + offset;
        Vector3 position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
        transform.position = position;

        transform.LookAt(target.transform.position);
    }*/

    //Follow Camera
    public GameObject target;
    public float damping = 1;
    Vector3 offset;

    void Start()
    {
        offset = target.transform.position - transform.position;
    }

    void LateUpdate()
    {
        float currentAngle = transform.eulerAngles.y;
        float desiredAngle = target.transform.eulerAngles.y;
        float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);

        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        transform.position = target.transform.position - (rotation * offset);

        transform.LookAt(target.transform);
    }
}

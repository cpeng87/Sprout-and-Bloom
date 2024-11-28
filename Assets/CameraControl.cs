using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 50.0f;
    public float zoomSpeed = 10.0f;
    public float minZoomDistance = 5.0f;
    public float maxZoomDistance = 50.0f;

    void Start()
    {
        // Initialize camera position
        this.gameObject.transform.position = new Vector3(target.position.x, target.position.y + 5f, -50);
    }

    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.RotateAround(target.position, Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            Vector3 currPos = GetComponent<Camera>().transform.position;
            this.gameObject.transform.position = new Vector3(currPos.x, currPos.y + (Input.GetAxis("Vertical") / 3.0f), currPos.z);
        }
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            Vector3 direction = transform.position - target.position;
            float distance = direction.magnitude;
            distance = Mathf.Clamp(distance - scrollInput * zoomSpeed, minZoomDistance, maxZoomDistance);

            transform.position = target.position + direction.normalized * distance;
        }
        // transform.LookAt(target);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickCameraRotate : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private float distanceToTarget;

    private Vector3 previousPosition;

    WowCameraScript wowCameraScript;


    private void Awake()
    {

        wowCameraScript = GetComponent<WowCameraScript>();
    }
    void Update()
    {
        distanceToTarget = wowCameraScript.currentDistance;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        //}
        //else
        if (Input.GetMouseButton(0))
        {
            Debug.Log("click");
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically

            cam.transform.position = target.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

            previousPosition = newPosition;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCinemachineFreeLookRotation : MonoBehaviour
{

    public float xDeg = 0.0f;
    public float yDeg = 0.0f;

    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.04f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.04f;
        }
    }
}

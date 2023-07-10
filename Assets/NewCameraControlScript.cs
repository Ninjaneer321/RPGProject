using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class NewCameraControlScript : MonoBehaviour
{
    // Start is called before the first frame update

    CinemachineFreeLook cinemachineFreeLook;

    void Start()
    {
        cinemachineFreeLook = transform.GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            cinemachineFreeLook.m_YAxis.m_InputAxisName = "Mouse Y";
            cinemachineFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
        }
        else
        {
            cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
            cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
        }
    }
}

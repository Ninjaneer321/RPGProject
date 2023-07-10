using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{

    PlayerControls playerControls;
    [SerializeField] float rotationAmount = 1;
    [SerializeField] float rotationSpeed = 5;

    float cameraInput;

    Vector3 currentRotation;
    Vector3 targetRotation;

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<float>();
        }

        playerControls.Enable();
    }

    private void Start()
    {
        currentRotation = transform.eulerAngles;
        targetRotation = transform.eulerAngles;
    }

    private void Update()
    {
        if (cameraInput > 0)
        {
            targetRotation.y = targetRotation.y + rotationAmount;
        }
        else if (cameraInput < 0)
        {
            targetRotation.y = targetRotation.y - rotationAmount;
        }

        currentRotation = Vector3.Lerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = currentRotation;
    }
}

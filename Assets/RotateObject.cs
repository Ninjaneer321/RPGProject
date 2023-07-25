using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateObject : MonoBehaviour
{
    [SerializeField] GameObject playerPreviewCamera = null;
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
        if (!playerPreviewCamera.activeInHierarchy) return;
        if (cameraInput > 0)
        {
            targetRotation.y = targetRotation.y + rotationAmount;
        }
        else if (cameraInput < 0)
        {
            targetRotation.y = targetRotation.y - rotationAmount;
        }

        Debug.Log(cameraInput);

        currentRotation = Vector3.Lerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = currentRotation;
    }

    public void RotateButtonInputA()
    {
        targetRotation.y = targetRotation.y + rotationAmount;

    }

    public void RotateButtonInputD()
    {
        targetRotation.y = targetRotation.y - rotationAmount;
    }

}

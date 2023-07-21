using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.UI;
using RPG.Control;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerLocomotion playerLocomotion;
    [SerializeField] GameObject targetInventoryUI;
    [SerializeField] public AudioSource lootAudioOpen;
    [SerializeField] public AudioSource lootAudioClose;

    PlayerAnimatorController animatorController;

    public Vector2 movementInput;// = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private float moveAmount;// = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float verticalInput;// = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float horizontalInput;// = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public bool jumpInput;// = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public bool autoRun;
    public bool isLooting = false;


    private void Start()
    {
        playerControls.PlayerActions.Autorun.performed += DoAutorun;
    }


    private void DoAutorun(InputAction.CallbackContext obj)
    {

        if (!autoRun)
        {
            if (obj.performed)
            {
                Debug.Log("performed");
                autoRun = true;
                movementInput = new Vector2(0, 1);
            }
        }
        else if (autoRun)
        {
            if (obj.performed)
            {
                Debug.Log("performed with autorun");
                autoRun = false;
                movementInput = new Vector2(0, 0);
            }
        }
    }

    private void Awake()
    {
        animatorController = GetComponent<PlayerAnimatorController>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }


    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleJumpingInput();
        HandleLootingInput();

    }

    private void HandleLootingInput()
    {
        GameObject otherInventory = GameObject.FindWithTag("OtherInventory");

        if (otherInventory != null)
        {
            animatorController.animator.SetBool("isLooting", otherInventory.GetComponent<ShowHideUI>().isOpened);
        }

    }
        //animatorcontroller and link up bool from TargetInventoryUI to a new bool on here that triggers an animation to play.
    
    private void HandleMovementInput()
    {

        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            verticalInput = 1f;
        }

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorController.UpdateAnimatorValues(0, moveAmount);


    }

    private void HandleJumpingInput()
    {

            if (jumpInput)           
            {
                jumpInput = false;
                playerLocomotion.HandleJumping();
            }


    }


}

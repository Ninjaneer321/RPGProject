using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Control
{
    public class PlayerLocomotion : MonoBehaviour
    {
        public Vector3 moveDirection;
        InputManager inputManager;
        public Transform cameraObject;
        Rigidbody playerRigidbody;
        PlayerManager playerManager;

        public AudioSource stepSound1;
        public AudioSource stepSound2;
        public bool stepSoundBool = false;


        //BROUGHT OVER ISINTERACTING FROM PLAYERMANAGER TO TEST
        public bool isInteracting;
        Animator animator;

        PlayerAnimatorController playerAnimatorController;

        public float inAirTimer;
        public float leapingVelocity;
        public float fallingVelocity;
        public LayerMask groundLayer;
        public float maxDistance = 1f;

        public bool isGrounded = true;
        public float rayCastHeightOffset = 0.5f;
        public bool isJumping = false;
        public float spherecastRadius = 0.2f;

        public float movementSpeed = 4;
        public float rotationSpeed = 15;
        public float gravityIntensity = -15;
        public float jumpHeight = 3;

        private void Awake()
        {
            
            playerManager = GetComponent<PlayerManager>();
            inputManager = GetComponent<InputManager>();
            playerRigidbody = GetComponent<Rigidbody>();
            playerAnimatorController = GetComponent<PlayerAnimatorController>();
            cameraObject = Camera.main.transform;

            isGrounded = true;
            animator = GetComponent<Animator>();
        
        }




        public void HandleAllMovement()
        {


            HandleFallingAndLanding();

            if (isInteracting)
            {
                return;
            }
            HandleMovement();
            HandleRotation();

        }

        private void HandleStuckInAir()
        {
            Respawner respawner = GetComponent<Respawner>();

            if (inAirTimer > 13f)
            {
                respawner.Respawn();
                inAirTimer = 0f;
            }
        }
        private void HandleMovement()
        { 
            if (isJumping) return;

            moveDirection = cameraObject.forward * inputManager.verticalInput;
            moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
            //moveDirection = transform.forward * inputManager.verticalInput;
            //moveDirection = moveDirection + transform.right * inputManager.horizontalInput;

            moveDirection.y = 0;
            moveDirection.Normalize();
            moveDirection = moveDirection * movementSpeed;

            Vector3 movementVelocity = moveDirection;
            playerRigidbody.velocity = movementVelocity;

        }

        private void HandleRotation()
        {

            if (isJumping) return;

            Vector3 targetDirection = Vector3.zero;

            targetDirection = cameraObject.forward * inputManager.verticalInput;
            targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;

            //targetDirection = transform.forward * inputManager.verticalInput;
            //targetDirection = targetDirection + transform.right * inputManager.horizontalInput;

            targetDirection.Normalize();
            targetDirection.y = 0;
            if (targetDirection == Vector3.zero)
            {
                targetDirection = transform.forward;
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = playerRotation;


        }


        //ISINTERACTING 
        //ISGROUNDED NEVER CHANGES
        //ISJUMPING


        private void HandleFallingAndLanding()
        {


            HandleStuckInAir();

            RaycastHit hit;
            Vector3 targetPosition;
            Vector3 raycastOrigin = transform.position;
            raycastOrigin.y = raycastOrigin.y + rayCastHeightOffset;
            targetPosition = transform.position;

            if (!isGrounded && !isJumping)
            {
                if (!isInteracting)
                {
                    playerAnimatorController.PlayTargetAnimation("Falling", true);
                }

                inAirTimer = inAirTimer + Time.deltaTime;
                playerRigidbody.AddForce(transform.forward * leapingVelocity);
                playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);

            }

            if (Physics.SphereCast(raycastOrigin, spherecastRadius, -Vector3.up, out hit, maxDistance, groundLayer))
            {
                if (!isGrounded && isInteracting)
                {
                    playerAnimatorController.PlayTargetAnimation("Land", true);
                }

                Vector3 rayCastHitPoint = hit.point;
                targetPosition.y = rayCastHitPoint.y;
                inAirTimer = 0;
                isGrounded = true;
                playerAnimatorController.BooleanUpdateAnimatorValues("isGrounded", true);
                isInteracting = false;

            }
            else
            {
                isGrounded = false;
                playerAnimatorController.BooleanUpdateAnimatorValues("isGrounded", false);
                isInteracting = true;
            }

            if (isGrounded)
            {
                if (isInteracting)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
                }
                else
                {
                    transform.position = targetPosition;
                }
            }
        }



        public void HandleJumping()
        {
            if (isGrounded)
            {
                playerAnimatorController.animator.SetBool("isJumping", true);
                playerAnimatorController.PlayTargetAnimation("Jump", false);

                float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
                Vector3 playerVelocity = moveDirection;
                playerVelocity.y = jumpingVelocity;
                playerRigidbody.velocity = playerVelocity;
            }
        }

        public void LeftFootStep()
        {
            if (isGrounded)
            {
                if (inputManager.movementInput != Vector2.zero)
                {
                    stepSound1.Play();
                }
            }
            //if (isGrounded && stepSoundBool)
            //{
            //    stepSound2.Play();
            //    stepSoundBool = false;
            //}
        }

        public void RightFootStep()
        {

            if (isGrounded)
            {
                if (inputManager.movementInput != Vector2.zero)
                {
                    stepSound2.Play();
                }
            }
        }


    }



}

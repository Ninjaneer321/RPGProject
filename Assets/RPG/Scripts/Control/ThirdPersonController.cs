using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Stats;
using GameDevTV.Saving;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using GameDevTV.Inventories;

namespace RPG.Control
{
    public class ThirdPersonController : MonoBehaviour, ISaveable
    {

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;

        //input fields
        private PlayerInput playerInput;
        private ThirdPersonInputActions playerActionsAsset;
        private InputAction move;
        private InputAction walk;
        private InputAction attack;
        private InputAction sheathe;
        private InputAction loot;

        [SerializeField] AnimatorOverrideController inCombatOverrideController = null;
        [SerializeField] RuntimeAnimatorController defaultOverrideController;

        //movement fields
        private Rigidbody rb;
        [SerializeField]
        private float movementForce = 1f;
        [SerializeField]
        private float jumpForce = 5f;
        [SerializeField]
        private float maxSpeed = 4f;
        [SerializeField]
        public float runSpeed = 4f;
        [SerializeField]
        private float walkSpeed = 2f;


        private Vector3 forceDirection = Vector3.zero;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        public float RotationSmoothTime = 0.12f;
        bool isDraggingUI = false;

        [SerializeField]
        private Camera playerCamera;
        private Animator animator;
        private Health health;
        private Fighter fighter;
        private ActionStore actionStore;

        //[SerializeField] private Health target;
        [SerializeField] public bool isSheathed;
        [SerializeField] public bool isAttacking;
        [SerializeField] private bool isWalking;
        [SerializeField] private bool canLoot;
        [SerializeField] float timeSinceLastAttack = Mathf.Infinity;
        [SerializeField] int numberOfAbilities = 6;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        private float _animationBlend;

        List<GameObject> tabTargetList = new List<GameObject>();

        int defaultMask;
        int enemyHighlightMask;
        private void Start()
        {
            isSheathed = false;
            isAttacking = false;
            isWalking = false;

            playerActionsAsset.Player.Jump.performed += DoJump;
            playerActionsAsset.Player.Target.performed += DoTarget;
            playerActionsAsset.Player.Cancel.performed += DoCancel;

            defaultMask = LayerMask.NameToLayer("Default");
            enemyHighlightMask = LayerMask.NameToLayer("Enemy");
        }

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            playerActionsAsset = new ThirdPersonInputActions();

            move = playerInput.actions["Move"];
            move.ReadValue<Vector3>();
            walk = playerInput.actions["Walk"];
            walk.ReadValue<float>();
            attack = playerInput.actions["Attack"];
            attack.ReadValue<float>();
            sheathe = playerInput.actions["Sheathe"];
            sheathe.ReadValue<float>();
            loot = playerInput.actions["Loot"];
            loot.ReadValue<float>();


            rb = this.GetComponent<Rigidbody>();
            animator = this.GetComponent<Animator>();
            health = this.GetComponent<Health>();
            fighter = this.GetComponent<Fighter>();
            actionStore = GetComponent<ActionStore>();
        }
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (InteractWithUI()) return;
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            UseAbilities();
            CycleTarget();

            if (InteractWithComponent()) return;
            if (InteractWithMovementTerrain()) return;

            SetCursor(CursorType.None);


        }


        public bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }
                SetCursor(CursorType.UI);
                return true;
            }
            if (isDraggingUI)
            {
                return true;
            }
            return false;
        }

        private void UseAbilities()
        {
                for (int i = 0; i < numberOfAbilities; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    {
                        actionStore.Use(i, gameObject);
                    }
                }
        }
        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(new PlayerManager()))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }
        private bool InteractWithMovementTerrain()
        {
            //Are we hovering over terrain or any other non-combat object?

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }
        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }



        private void FixedUpdate()
        {
            Move();
            GroundCheck();
            WalkCheck();
            SheatheCheck();
            AttackCheck();
            canLoot = false;
        }




        private void SheatheCheck()
        {
            if (!isSheathed)
            {
                if (sheathe.triggered)
                {
                    fighter.rightHandTransform.gameObject.SetActive(false);
                    fighter.leftHandTransform.gameObject.SetActive(false);
                    attack.Disable();
                    isAttacking = false;
                    Debug.Log("Sheathed!");
                    isSheathed = true;
                }
            }
            else
            {
                if (sheathe.triggered)
                {
                    fighter.rightHandTransform.gameObject.SetActive(true);
                    fighter.leftHandTransform.gameObject.SetActive(true);
                    attack.Enable();
                    Debug.Log("Unsheathed!");
                    isSheathed = false;
                }
            }

        }

        private void GroundCheck()
        {
            if (!IsGrounded())
            {
                animator.SetBool("Grounded", false);
                animator.SetBool("inAir", true);
            }
            else
            {
                animator.SetBool("Grounded", true);
                animator.SetBool("inAir", false);
            }
        }

        private void OnEnable()
        {
            playerActionsAsset.Enable();
        }
        private void OnDisable()
        {
            playerActionsAsset.Disable();

        }



        private void Move()
        {
            LookAt();
            if (!health.IsDead())
            {

                forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
                forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;

                rb.AddForce(forceDirection, ForceMode.Impulse);
                forceDirection = Vector3.zero;

                if (rb.velocity.y < 0f)
                    rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;



                Vector3 horizontalVelocity = rb.velocity;
                horizontalVelocity.y = 0;
                if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
                    rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;

                Vector3 inputDirection = new Vector3(move.ReadValue<Vector2>().x, 0.0f, move.ReadValue<Vector2>().y).normalized;

                if (move.ReadValue<Vector2>() != Vector2.zero)
                {

                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                    // rotate to face input direction relative to camera position
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }

                _animationBlend = Mathf.Lerp(_animationBlend, maxSpeed, Time.deltaTime * SpeedChangeRate);

                animator.SetFloat("Speed", _animationBlend);


            }
        }


        private void LookAt()
        {

            Vector3 direction = rb.velocity;
            direction.y = 0f;

            if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            {
                this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            else
                rb.angularVelocity = Vector3.zero;
        }

        private void DoJump(InputAction.CallbackContext obj)
        {
            if (IsGrounded())
            {
                animator.SetBool("jump", true);
                forceDirection += Vector3.up * jumpForce;
            }
            else
            {
                return;
            }
        }

        private void DoTarget(InputAction.CallbackContext obj)
        {
            LayerMask layerMask = 1 << 3;
            layerMask = ~layerMask;
            float distance = 100f;
            RaycastHit hit;

            if (obj.performed)
            {
                if (fighter.target != null)
                {
                    //fighter.target.GetComponent<AIController>().Deselected();
                }
                if (Physics.Raycast(GetMouseRay(), out hit, distance, layerMask))
                {
                    if (!hit.collider.GetComponent<CombatTarget>())
                    {
                        return;
                    }
                    //fighter.target = hit.collider.gameObject;
                    //fighter.target.GetComponent<AIController>().Selected();
                }
            }

        }


        private int targetIndex = 0;


        private void CycleTarget()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (fighter.target != null)
                {
                    //fighter.target.GetComponent<AIController>().Deselected();
                }
                tabTargetList.Clear();
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, 30f, Vector3.forward);

                foreach (var hit in hits)
                {
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        tabTargetList.Add(hit.transform.gameObject);
                    }
                }
                if (tabTargetList.Count > 0)
                {
                    //fighter.target = tabTargetList[targetIndex];
                    //tabTargetList[targetIndex].GetComponent<AIController>().Selected();
                    targetIndex++;
                    if (targetIndex >= tabTargetList.Count)
                    {
                        targetIndex = 0;
                    }
                }
            }
        }
        public void Attack()
        {
            if (fighter.GetTarget() == null) return;
            if (timeSinceLastAttack < fighter.currentWeaponConfig.weaponSpeed) return;
            if (health.IsDead()) return;

            if (fighter.GetTarget().GetComponent<CombatTarget>() && InFront())
            {
                if (timeSinceLastAttack >= fighter.currentWeaponConfig.weaponSpeed && InRangeOfWeapon())
                {
                    this.animator.SetTrigger("attack");
                    timeSinceLastAttack = 0;
                }
                else
                {
                    Debug.Log("You are too far away from the enemy!");
                }
            }
        }

        private void AttackCheck()
        {
            if (fighter.GetTarget() == null || !fighter.GetTarget().GetComponent<CombatTarget>().enabled)
            {
                isAttacking = false;
                return;
            }
            if (fighter.GetTarget().GetComponent<Health>().IsDead())
            {
                fighter.target = null;

                if (fighter.currentWeaponConfig.HasProjectile()) return;

                animator.runtimeAnimatorController = defaultOverrideController;
            }
            if (!isAttacking)
            {
                if (attack.triggered)
                {
                    isAttacking = true;
                    sheathe.Disable();

                    if (fighter.currentWeaponConfig.HasProjectile()) return;

                    defaultOverrideController = animator.runtimeAnimatorController;
                    animator.runtimeAnimatorController = inCombatOverrideController;
                }
            }
            else if (isAttacking)
            {
                Attack();
                if (attack.triggered)
                {
                    isAttacking = false;
                    sheathe.Enable();
                    if (fighter.currentWeaponConfig.HasProjectile()) return;
                    animator.runtimeAnimatorController = defaultOverrideController;
                }
            }
        }
        private void DoCancel(InputAction.CallbackContext obj)
        {
            if (fighter.target != null)
            {
                //fighter.target.GetComponent<AIController>().Deselected();
                fighter.Cancel();
            }
            tabTargetList.Clear();
            targetIndex = 0;
            move.Enable();
        }

        public bool InFront()
        {
            if (fighter.GetTarget() != null)
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 toOther = fighter.GetTarget().transform.position - transform.position;

                if (Vector3.Dot(forward, toOther) > 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool InRangeOfWeapon()
        {
            return Vector3.Distance(transform.position, fighter.GetTarget().transform.position) < fighter.currentWeaponConfig.weaponRange;
        }

        private void WalkCheck()
        {
            if (!isWalking)
            {
                if (walk.triggered)
                {
                    isWalking = true;
                    maxSpeed = walkSpeed;

                }
            }
            else if (walk.triggered)
            {
                isWalking = false;
                maxSpeed = runSpeed;

            }

        }
        private bool IsGrounded()
        {
            Ray ray = new Ray(this.transform.position + Vector3.up * .5f, Vector3.down);

            Debug.DrawRay(this.transform.position + Vector3.up * .5f, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, .7f))
            {
                return true;
            }

            else
            {
                animator.SetBool("jump", false);
                return false;
            }
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            transform.position = position.ToVector();
        }

        public static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        private Vector3 GetCameraRight(Camera playerCamera)
        {
            Vector3 right = playerCamera.transform.right;
            right.y = 0;
            return right.normalized;
        }

        private Vector3 GetCameraForward(Camera playerCamera)
        {
            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0;
            return forward.normalized;
        }
        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }




        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<Pickup>())
            {
                canLoot = true;

                //Eventually needs a UI element that shows the Player can loot.
                Debug.Log("Loot item...");
            }

            if (canLoot)
            {
               if (loot.triggered)
                {
                    other.GetComponent<Pickup>().PickupItem();
                }
            }

        }
    }
}



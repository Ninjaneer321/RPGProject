using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using GameDevTV.UI;
using RPG.Abilities;
using RPG.Combat;
using RPG.Stats;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RPG.Control
{
    public class PlayerManager : MonoBehaviour, ISaveable
    {

        InputManager inputManager;
        PlayerLocomotion playerLocomotion;
        Rigidbody rb;
        Animator animator;
        Health health;
        Fighter fighter;
        ActionStore actionStore;
        PlayerControls playerControls;

        PlayerInput playerInput;
        private InputAction attack;
        private InputAction cancel;

        public bool isDraggingUI;
        public bool isCastingAbility;

        [SerializeField] public bool isSheathed;
        [SerializeField] private bool isWalking;
        [SerializeField] private bool canLoot;
        [SerializeField] public bool isAttacking;
        [SerializeField] public float timeSinceLastAttack = 0;
        [SerializeField] int numberOfAbilities = 6;
        public LayerMask targetingLayerMask;

        List<GameObject> tabTargetList = new List<GameObject>();
        private int targetIndex = 0;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;

        private void Start()
        {

            playerControls.PlayerActions.Target.performed += DoTarget;
        }


        //public override void OnNetworkSpawn()
        //{
        //    base.OnNetworkSpawn();

        //    playerControls.PlayerActions.Target.performed += DoTarget;

        //}


        private void Awake()
        {
            playerControls = new PlayerControls();

            playerInput = GetComponent<PlayerInput>();
            inputManager = GetComponent<InputManager>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            rb = this.GetComponent<Rigidbody>();
            animator = this.GetComponent<Animator>();
            health = this.GetComponent<Health>();
            fighter = this.GetComponent<Fighter>();
            actionStore = GetComponent<ActionStore>();


            attack = playerInput.actions["Attack"];
            attack.ReadValue<float>();
            cancel = playerInput.actions["Cancel"];
            cancel.ReadValue<float>();


        }

        public void OnEnable()
        {
            playerControls.Enable();
        }
        public void OnDisable()
        {
            playerControls.Disable();

        }
        private void Update()
        {
            inputManager.HandleAllInputs();
            if (InteractWithUI()) return;
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            DoCancel();
            AttackBehaviour();
            UseAbilities();
            CycleTarget();


            if (InteractWithComponent()) return;
            if (InteractWithMovementTerrain()) return;

            SetCursor(CursorType.None);

        }

        private void FixedUpdate()
        {
            timeSinceLastAttack += Time.deltaTime;
            playerLocomotion.HandleAllMovement();

            animator.SetBool("isGrounded", playerLocomotion.isGrounded);
            //isInteracting = animator.GetBool("isInteracting");
        }

        private void LateUpdate()
        {
            playerLocomotion.isJumping = animator.GetBool("isJumping");


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
            if (isCastingAbility)
            {
                return;
            }

            for (int i = 0; i < numberOfAbilities; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    actionStore.UseItem(i, gameObject);
                    actionStore.UseAbility(i, gameObject);
                }
            }
        }

        public void AttackTestMethod()
        {
            if (timeSinceLastAttack >= fighter.currentWeaponConfig.weaponSpeed && InRangeOfWeapon())
            {
                timeSinceLastAttack = 0;
                animator.SetTrigger("attack");
                Attack();
            }
        }


        public void Attack()
        {

            if (fighter.GetTarget() == null)
            {
                return;
            }

            if (timeSinceLastAttack < fighter.currentWeaponConfig.weaponSpeed) return;
            if (health.IsDead()) return;

            if (fighter.GetTarget().GetComponent<CombatTarget>() && InFront())
            {
                if (timeSinceLastAttack >= fighter.currentWeaponConfig.weaponSpeed && InRangeOfWeapon())
                {            
                    animator.SetTrigger("attack");
                    timeSinceLastAttack = 0;
                }
                else
                {
                    Debug.Log("You are too far away from the enemy! Create a UI Element for this");
                }
            }
        }



        private void DoTarget(InputAction.CallbackContext obj)
        {
            //targetingLayerMask = ~targetingLayerMask;

            RaycastHit[] hits = RaycastAllSorted();

            
            if (obj.performed)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.GetComponent<AIController>())
                    {
                        if (fighter.target != null)
                        {
                            if (hit.collider.gameObject != fighter.target.gameObject)
                            {
                                fighter.target.GetComponent<AIController>().Deselected();
                            }
                        }
                        hit.collider.GetComponent<AIController>().Selected();
                        fighter.target = hit.collider.GetComponent<Health>();

                    }
                }
            }
        }

        public void AttackBehaviour()
        {
            if (fighter.GetTarget() == null || !fighter.GetTarget().GetComponent<CombatTarget>())
            {
                isAttacking = false;
                return;
            }
            if (fighter.GetTarget().GetComponent<Health>().IsDead())
            {
                fighter.target = null;

                if (fighter.currentWeaponConfig.HasProjectile()) return;

            }
            if (!isAttacking)
            {
                if (attack.triggered)
                {
                    fighter.isInCombat = true;
                    isAttacking = true;
                    if (fighter.currentWeaponConfig.HasProjectile()) return;

                }
            }
            else if (isAttacking)
            {
                //AttackLoop
                Attack();

                if (attack.triggered)
                {
                    fighter.isInCombat = false;
                    isAttacking = false;

                    if (fighter.currentWeaponConfig.HasProjectile()) return;
                }
            }
        }

        private bool InRangeOfWeapon()
        {
            return Vector3.Distance(transform.position, fighter.GetTarget().transform.position) < fighter.currentWeaponConfig.weaponRange;
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
        private void CycleTarget()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {

                tabTargetList.Clear();
                if (fighter.target != null && fighter.target.tag == "Enemy")
                {
                    fighter.target.GetComponent<AIController>().Deselected();
                }


                RaycastHit[] hits = Physics.SphereCastAll(transform.position, 40f, Vector3.up);

                foreach (var hit in hits)
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        tabTargetList.Add(hit.transform.gameObject);
                    }
                }
                if (tabTargetList.Count > 0)
                {
                    fighter.target = tabTargetList[targetIndex].GetComponent<Health>();
                    tabTargetList[targetIndex].GetComponent<AIController>().Selected();
                    targetIndex++;
                    if (targetIndex >= tabTargetList.Count)
                    {
                        targetIndex = 0;
                    }
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
                    if (raycastable.HandleRaycast(this))
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

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        RaycastHit[] RaycastAllSorted()
        {

            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay(), 100f); //this 100f needs to be changed in a way that 
                                                                         // i can target npcs and enemies but not interact until close enough
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }


        private void DoCancel()
        {
            if (cancel.triggered)
            {

                if (fighter.target == null)
                {
                    Debug.Log("Show Pause Menu Here");
                    tabTargetList.Clear();
                    targetIndex = 0;
                    return;
                }
                if (fighter.target != null)
                {
                    if (fighter.target.tag == "Enemy")
                    {
                        fighter.target.GetComponent<AIController>().Deselected();
                    }
                    fighter.isInCombat = false;
                    fighter.Cancel();
                    tabTargetList.Clear();
                    targetIndex = 0;
                    return;
                }
                //tabTargetList.Clear();
                //targetIndex = 0;
            }

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


    }
}


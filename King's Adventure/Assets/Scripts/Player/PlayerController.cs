using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Properites 
    public bool CanMove {get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] bool canSprint = true;
    [SerializeField] bool canJump = true;
    [SerializeField] bool canCrouch = true;
    [SerializeField] bool canUseHeadBob = true;

    [Header("Controls")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;

    // Settings for movement
    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float crouchSpeed = 1.5f;

    // Settings for Looking Around
    [Header("Look Settings")]
    [SerializeField, Range(1, 10)] float lookSpeedX = 2f;
    [SerializeField, Range(1, 10)] float lookSpeedY = 2f;
    [SerializeField, Range(1, 180)] float upperLookLimit = 80f;
    [SerializeField, Range(1, 180)] float lowerLookLimit = 80f;
    [SerializeField] bool HideCursor = true;

    // Settings for Jumping 
    [Header("Jumping Settings")]
    [SerializeField] float jumpForce = 8f;
    [SerializeField] float gravity = 30f;

    // Settings for Jumping
    [Header("Crouch Settings")]
    [SerializeField] float crouchHeight = 0.5f;
    [SerializeField] float standingHeight = 2f;
    [SerializeField] float timeToCrouch = 0.25f;
    [SerializeField] Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] Vector3 standingCenter = new Vector3(0, 0, 0);
    bool isCrouching;
    bool duringCrouchAnimation;

    // Settings for Headbob
    [Header("Headbob Settings")]
    [SerializeField] float walkBobSpeed = 14f;
    [SerializeField] float walkBobAmount = 0.05f;
    [SerializeField] float sprintBobSpeed = 18f;
    [SerializeField] float sprintBobAmount = 0.1f;
    [SerializeField] float crouchBobSpeed = 8f;
    [SerializeField] float crouchBobAmount = 0.025f;
    float defaultYPos = 0;
    float timer;

    // References
    Camera playerCamera;
    CharacterController characterController;

    Vector3 moveDirection;
    Vector2 currentInput;

    float rotationX = 0;

    // Start is called before the first frame update
    public void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();

        // Makes usre we have our default y position
        defaultYPos = playerCamera.transform.localPosition.y;

        // Hide Cursor
        
        if(HideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        // Movement: To check if we can move
        if(CanMove)
        {
            // Movement
            HandleMovementInput();
            HandleMouseLook();

            // Check if we can jump
            if(canJump)
            {
                HandleJump();
            }

            // Check if we can crouch
            if(canCrouch)
            {
                HandleCrouch();
            }

            // Check if we can headbob
            if(canUseHeadBob)
            {
                HandleHeadbob();
            }

            // Applying the movement to 
            ApplyFinialMovements();
        }   
    }

    void HandleMovementInput()
    {
        // Movment: Determan the movement
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    void HandleMouseLook()
    {
        // MouseLook: Looking up and down
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // MouseLook: Looking Left and right
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    void HandleJump()
    {
        if(ShouldJump)
        {
            moveDirection.y = jumpForce;
        }
    }

    void HandleCrouch()
    {
        if(ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    void HandleHeadbob() 
    {
        if(!characterController.isGrounded)
        {
            return;
        }

        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultYPos + Mathf.Sin(timer) *
            (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount), playerCamera.transform.localPosition.z);
        }
    }

    void ApplyFinialMovements()
    {
        // Movement: Applying the movement from our Character Controller
        if(!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    IEnumerator CrouchStand()
    {
        // Checks if we are above something when we are crouching
        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
        {
            yield break;
        }

        duringCrouchAnimation = true;

        float timeElaped = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElaped < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElaped/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElaped/timeToCrouch);
            timeElaped += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }
}

// ------------------------------------------ 
// Modification of the BasicFPCC by Alucard Jay Kay 
// ------------------------------------------ 

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Layer Mask")]
    [Tooltip("Layer Mask for sphere/raycasts. Assign the Player object to a Layer, then Ignore that layer here.")]
    public LayerMask castingMask;                              // Layer mask for casts. You'll want to ignore the player.

    // - Components -
    private CharacterController controller;                    // CharacterController component
    private Transform playerTx;                                // this player object

    [Header("Main Camera")]
    [Tooltip("Drag the FPC Camera here")]
    public Transform cameraTx;                                 // Main Camera, as child of BasicFPCC object

    [Header("Optional Player Graphic")]
    [Tooltip("optional capsule to visualize player in scene view")]
    public Transform playerGFX;                                // optional capsule graphic object

    [Space(5)]

    // Input Variables that can be assigned externally
    // the cursor can also be manually locked or freed by calling the public void SetLockCursor( bool doLock )
    [HideInInspector] public float inputLookX = 0;      //
    [HideInInspector] public float inputLookY = 0;      //
    [HideInInspector] public float inputMoveX = 0;      // range -1f to +1f
    [HideInInspector] public float inputMoveY = 0;      // range -1f to +1f
    [HideInInspector] public bool inputKeyDownCursor = false;  // is key Pressed

    [Header("Look Settings")]
    public float mouseSensitivityX = 1f;             // speed factor of look X
    public float mouseSensitivityY = 1f;             // speed factor of look Y
    [Tooltip("larger values for less filtering, more responsiveness")]
    public float mouseSnappiness = 20f;              // default was 10f; larger values of this cause less filtering, more responsiveness
    public bool invertLookY = false;                 // toggle invert look Y
    public float clampLookY = 90f;                   // maximum look up/down angle

    [Header("Move Settings")]
    public float walkSpeed = 7f;                     // regular movement speed
    public float gravity = -9.81f;                   // gravity / fall rate

    [Header("Grounded Settings")]
    [Tooltip("The starting position of the isGrounded spherecast. Set to the sphereCastRadius plus the CC Skin Width. Enable showGizmos to visualize.")]
    // this should be just above the base of the cc, in the amount of the skin width (in case the cc sinks in)
    //public float startDistanceFromBottom = 0.2f;  
    public float groundCheckY = 0.33f;               // 0.25 + 0.08 (sphereCastRadius + CC skin width)
    [Tooltip("The position of the ceiling checksphere. Set to the height minus sphereCastRadius plus the CC Skin Width. Enable showGizmos to visualize.")]
    // this should extend above the cc (by approx skin width) so player can still move when not at full height (not crouching, trying to stand up), 
    // otherwise if it's below the top then the cc gets stuck
    public float ceilingCheckY = 1.83f;              // 2.00 - 0.25 + 0.08 (height - sphereCastRadius + CC skin width) 
    [Space(5)]
    public float sphereCastRadius = 0.25f;           // radius of area to detect for ground
    public float sphereCastDistance = 0.75f;         // How far spherecast moves down from origin point
    [Space(5)]
    public float raycastLength = 0.75f;              // secondary raycasts (match to sphereCastDistance)
    public Vector3 rayOriginOffset1 = new Vector3(-0.2f, 0f, 0.16f);
    public Vector3 rayOriginOffset2 = new Vector3(0.2f, 0f, -0.16f);

    [Header("Debug Gizmos")]
    [Tooltip("Show debug gizmos and lines")]
    public bool showGizmos = false;                  // Show debug gizmos and lines

    // - private reference variables -
    private float defaultHeight = 0;                 // reference to scale player crouch
    private float cameraStartY = 0;                  // reference to move camera with crouch

    [Header("- reference variables -")]
    public float xRotation = 0f;                     // the up/down angle the player is looking
    private float lastSpeed = 0;                     // reference for calculating speed
    private Vector3 fauxGravity = Vector3.zero;      // calculated gravity
    private float accMouseX = 0;                     // reference for mouse look smoothing
    private float accMouseY = 0;                     // reference for mouse look smoothing
    private Vector3 lastPos = Vector3.zero;          // reference for player velocity 
    [Space(5)]
    public bool isGrounded = false;
    public float groundSlopeAngle = 0f;              // Angle of the slope in degrees
    public Vector3 groundSlopeDir = Vector3.zero;    // The calculated slope as a vector
    private float groundOffsetY = 0;                 // calculated offset relative to height
    public bool isSlipping = false;
    [Space(5)]
    public bool isSliding = false;
    public float slideTimer = 0;                     // current slide duration
    public Vector3 slideForward = Vector3.zero;      // direction of the slide
    [Space(5)]
    public bool isCeiling = false;
    private float ceilingOffsetY = 0;                // calculated offset relative to height
    [Space(5)]
    public bool cursorActive = false;                // cursor state

    [Header("Sound")]
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] private AudioClip[] walkClips;
    [SerializeField] private float walkStepDuration = 0.2f;
    private Coroutine walkSoundCoroutine = null;

    [HideInInspector] public bool toggleInput = true;
    [HideInInspector] public Action OnConfirmClick;
    [HideInInspector] public Action OnPauseClick;
    [HideInInspector] public PlayerInput playerInput;

    public static PlayerController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        ProcessLook();
        ProcessMovement();
    }

    void Initialize()
    {
        if (!cameraTx) { Debug.LogError("* " + gameObject.name + ": BasicFPCC has NO CAMERA ASSIGNED in the Inspector *"); }

        controller = GetComponent<CharacterController>();

        playerTx = transform;
        defaultHeight = controller.height;
        lastSpeed = 0;
        fauxGravity = Vector3.up * gravity;
        lastPos = playerTx.position;
        cameraStartY = cameraTx.localPosition.y;
        groundOffsetY = groundCheckY;
        ceilingOffsetY = ceilingCheckY;

        RefreshCursor();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!toggleInput)
        {
            return;
        }
        inputMoveX = context.ReadValue<Vector2>().x;
        inputMoveY = context.ReadValue<Vector2>().y;

        if ((inputMoveX != 0 || inputMoveY != 0) && walkSoundCoroutine == null)
        {
            walkSoundCoroutine = StartCoroutine(PlayWalkSound());
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!toggleInput)
        {
            return;
        }
        inputLookX = context.ReadValue<Vector2>().x;
        inputLookY = context.ReadValue<Vector2>().y;
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (!toggleInput)
        {
            return;
        }
        if (context.started)
        {
            OnConfirmClick?.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnPauseClick?.Invoke();
        }
    }

    void ProcessLook()
    {
        accMouseX = Mathf.Lerp(accMouseX, inputLookX, mouseSnappiness * Time.deltaTime);
        accMouseY = Mathf.Lerp(accMouseY, inputLookY, mouseSnappiness * Time.deltaTime);

        float mouseX = accMouseX * mouseSensitivityX * 100f * Time.deltaTime;
        float mouseY = accMouseY * mouseSensitivityY * 100f * Time.deltaTime;

        // rotate camera X
        xRotation += (invertLookY == true ? mouseY : -mouseY);
        xRotation = Mathf.Clamp(xRotation, -clampLookY, clampLookY);

        cameraTx.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // rotate player Y
        playerTx.Rotate(Vector3.up * mouseX);
    }

    void ProcessMovement()
    {
        // - variables -
        float vScale = 1f; // for calculating GFX scale (optional)
        float h = defaultHeight;
        float nextSpeed = walkSpeed;
        Vector3 calc; // used for calculations
        Vector3 move; // direction calculation

        // player current speed
        float currSpeed = (playerTx.position - lastPos).magnitude / Time.deltaTime;
        currSpeed = (currSpeed < 0 ? 0 - currSpeed : currSpeed); // abs value

        // - Check if Grounded -
        GroundCheck();

        isSlipping = (groundSlopeAngle > controller.slopeLimit ? true : false);

        // - Check Ceiling above for Head Room -
        CeilingCheck();

        move = (playerTx.right * inputMoveX) + (playerTx.forward * inputMoveY);

        if (move.magnitude > 1f)
        {
            move = move.normalized;
        }

        // - Height -

        // crouch/stand up smoothly
        float lastHeight = controller.height;
        float nextHeight = Mathf.Lerp(controller.height, h, 5f * Time.deltaTime);

        // if crouching, or only stand if there is no ceiling
        if (nextHeight < lastHeight || !isCeiling)
        {
            controller.height = Mathf.Lerp(controller.height, h, 5f * Time.deltaTime);

            // fix vertical position
            calc = playerTx.position;
            calc.y += (controller.height - lastHeight) / 2f;
            playerTx.position = calc;

            // offset camera
            calc = cameraTx.localPosition;
            calc.y = (controller.height / defaultHeight) + cameraStartY - (defaultHeight * 0.5f);
            cameraTx.localPosition = calc;

            // calculate offset
            float heightFactor = (defaultHeight - controller.height) * 0.5f;

            // offset ground check
            groundOffsetY = heightFactor + groundCheckY;

            // offset ceiling check
            ceilingOffsetY = heightFactor + controller.height - (defaultHeight - ceilingCheckY);

            // scale gfx (optional)
            if (playerGFX)
            {
                calc = playerGFX.localScale;
                calc.y = Mathf.Lerp(calc.y, vScale, 5f * Time.deltaTime);
                playerGFX.localScale = calc;
            }
        }

        // - Slipping Jumping Gravity - 

        // smooth speed
        float speed;

        if (isGrounded)
        {
            //if (isSlipping) // slip down slope
            //{
            //    // movement left/right while slipping down
            //    // player rotation to slope
            //    Vector3 slopeRight = Quaternion.LookRotation(Vector3.right) * groundSlopeDir;
            //    float dot = Vector3.Dot(slopeRight, playerTx.right);
            //    // move on X axis, with Y rotation relative to slopeDir
            //    move = slopeRight * (dot > 0 ? inputMoveX : -inputMoveX);

            //    // speed
            //    nextSpeed = Mathf.Lerp(currSpeed, runSpeed, 5f * Time.deltaTime);

            //    // increase angular gravity
            //    float mag = fauxGravity.magnitude;
            //    calc = Vector3.Slerp(fauxGravity, groundSlopeDir * runSpeed, 4f * Time.deltaTime);
            //    fauxGravity = calc.normalized * mag;
            //}
            // reset angular fauxGravity movement
            fauxGravity.x = 0;
            fauxGravity.z = 0;

            if (fauxGravity.y < 0) // constant grounded gravity
            {
                //fauxGravity.y = -1f;
                fauxGravity.y = Mathf.Lerp(fauxGravity.y, -1f, 4f * Time.deltaTime);
            }

            //    // - Jump -
            //    if ( !isSliding && !isCeiling && inputKeyDownJump ) // jump
            //    {
            //        fauxGravity.y = Mathf.Sqrt( jumpHeight * -2f * gravity );
            //    }

            //    // --

            //    // - smooth speed -
            // take less time to slow down, more time speed up
            float lerpFactor = (lastSpeed > nextSpeed ? 4f : 2f);
            speed = Mathf.Lerp(lastSpeed, nextSpeed, lerpFactor * Time.deltaTime);
        }
        else // no friction, speed changes slower
        {
            speed = Mathf.Lerp(lastSpeed, nextSpeed, 0.125f * Time.deltaTime);
        }

        // prevent floating if jumping into a ceiling
        //if ( isCeiling )
        //{
        //    speed = crouchSpeed; // clamp speed to crouched

        //    if ( fauxGravity.y > 0 )
        //    {
        //        fauxGravity.y = -1f; // 0;
        //    }
        //}

        lastSpeed = speed; // update reference

        // - Add Gravity -

        fauxGravity.y += gravity * Time.deltaTime;

        //- Move -

        calc = move * speed * Time.deltaTime;
        calc += fauxGravity * Time.deltaTime;

        controller.Move(calc);

        // - DEBUG - 

#if UNITY_EDITOR
        // slope angle and fauxGravity debug info
        if (showGizmos)
        {
            calc = playerTx.position;
            calc.y += groundOffsetY;
            Debug.DrawRay(calc, groundSlopeDir.normalized * 5f, Color.blue);
            Debug.DrawRay(calc, fauxGravity, Color.green);
        }
#endif
    }

    // lock/hide or show/unlock cursor
    public void SetLockCursor(bool doLock)
    {
        cursorActive = doLock;
        RefreshCursor();
    }

    void ToggleLockCursor()
    {
        cursorActive = !cursorActive;
        RefreshCursor();
    }

    void RefreshCursor()
    {
        if (!cursorActive && Cursor.lockState != CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.Locked; }
        if (cursorActive && Cursor.lockState != CursorLockMode.None) { Cursor.lockState = CursorLockMode.None; }
    }

    // check the area above, for standing from crouch
    void CeilingCheck()
    {
        Vector3 origin = new Vector3(playerTx.position.x, playerTx.position.y + ceilingOffsetY, playerTx.position.z);

        isCeiling = Physics.CheckSphere(origin, sphereCastRadius, castingMask);
    }

    // find if isGrounded, slope angle and directional vector
    void GroundCheck()
    {
        //Vector3 origin = new Vector3( transform.position.x, transform.position.y - (controller.height / 2) + startDistanceFromBottom, transform.position.z );
        Vector3 origin = new Vector3(playerTx.position.x, playerTx.position.y + groundOffsetY, playerTx.position.z);

        // Out hit point from our cast(s)
        RaycastHit hit;

        // SPHERECAST
        // "Casts a sphere along a ray and returns detailed information on what was hit."
        if (Physics.SphereCast(origin, sphereCastRadius, Vector3.down, out hit, sphereCastDistance, castingMask))
        {
            // Angle of our slope (between these two vectors). 
            // A hit normal is at a 90 degree angle from the surface that is collided with (at the point of collision).
            // e.g. On a flat surface, both vectors are facing straight up, so the angle is 0.
            groundSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            // Find the vector that represents our slope as well. 
            //  temp: basically, finds vector moving across hit surface 
            Vector3 temp = Vector3.Cross(hit.normal, Vector3.down);
            //  Now use this vector and the hit normal, to find the other vector moving up and down the hit surface
            groundSlopeDir = Vector3.Cross(temp, hit.normal);

            // --
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }   // --

        // Now that's all fine and dandy, but on edges, corners, etc, we get angle values that we don't want.
        // To correct for this, let's do some raycasts. You could do more raycasts, and check for more
        // edge cases here. There are lots of situations that could pop up, so test and see what gives you trouble.
        RaycastHit slopeHit1;
        RaycastHit slopeHit2;

        // FIRST RAYCAST
        if (Physics.Raycast(origin + rayOriginOffset1, Vector3.down, out slopeHit1, raycastLength))
        {
            // Debug line to first hit point
#if UNITY_EDITOR
            if (showGizmos) { Debug.DrawLine(origin + rayOriginOffset1, slopeHit1.point, Color.red); }
#endif
            // Get angle of slope on hit normal
            float angleOne = Vector3.Angle(slopeHit1.normal, Vector3.up);

            // 2ND RAYCAST
            if (Physics.Raycast(origin + rayOriginOffset2, Vector3.down, out slopeHit2, raycastLength))
            {
                // Debug line to second hit point
#if UNITY_EDITOR
                if (showGizmos) { Debug.DrawLine(origin + rayOriginOffset2, slopeHit2.point, Color.red); }
#endif
                // Get angle of slope of these two hit points.
                float angleTwo = Vector3.Angle(slopeHit2.normal, Vector3.up);
                // 3 collision points: Take the MEDIAN by sorting array and grabbing middle.
                float[] tempArray = new float[] { groundSlopeAngle, angleOne, angleTwo };
                System.Array.Sort(tempArray);
                groundSlopeAngle = tempArray[1];
            }
            else
            {
                // 2 collision points (sphere and first raycast): AVERAGE the two
                float average = (groundSlopeAngle + angleOne) / 2;
                groundSlopeAngle = average;
            }
        }
    }

    // this script pushes all rigidbodies that the character touches
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.
        body.velocity = hit.moveDirection * lastSpeed;
    }

    // STEP SOUND
    IEnumerator PlayWalkSound()
    {
        while (inputMoveX != 0 || inputMoveY != 0)
        {
            walkAudioSource.Stop();
            walkAudioSource.clip = walkClips[Random.Range(0, walkClips.Length)];
            walkAudioSource.pitch = Random.Range(0.9f, 1.1f);
            walkAudioSource.Play();

            yield return new WaitForSeconds(walkStepDuration);
        }

        walkSoundCoroutine = null;
    }

    // Debug Gizmos
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            if (!Application.isPlaying)
            {
                groundOffsetY = groundCheckY;
                ceilingOffsetY = ceilingCheckY;
            }

            Vector3 startPoint = new Vector3(transform.position.x, transform.position.y + groundOffsetY, transform.position.z);
            Vector3 endPoint = startPoint + new Vector3(0, -sphereCastDistance, 0);
            Vector3 ceilingPoint = new Vector3(transform.position.x, transform.position.y + ceilingOffsetY, transform.position.z);

            Gizmos.color = (isGrounded == true ? Color.green : Color.white);
            Gizmos.DrawWireSphere(startPoint, sphereCastRadius);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(endPoint, sphereCastRadius);

            Gizmos.DrawLine(startPoint, endPoint);

            Gizmos.color = (isCeiling == true ? Color.red : Color.white);
            Gizmos.DrawWireSphere(ceilingPoint, sphereCastRadius);
        }
    }
#endif
}
using UnityEngine;

namespace ElmanGameDevTools.PlayerSystem
{
    [AddComponentMenu("Elman Game Dev Tools/Player System/Player Controller")]
    public class PlayerController : MonoBehaviour
    {
        [Header("REFERENCES")]
        [Tooltip("Reference to the Character Controller component")]
        public CharacterController controller;
        [Tooltip("Reference to the player camera transform")]
        public Transform playerCamera;

        [Header("MOVEMENT SETTINGS")]
        [Tooltip("Base walking speed")]
        public float speed = 6f;
        [Tooltip("Running speed")]
        public float runSpeed = 9f;
        [Tooltip("Height the player can jump")]
        public float jumpHeight = 1f;
        [Tooltip("Gravity force applied to the player")]
        public float gravity = -9.81f;
        [Tooltip("Mouse look sensitivity")]
        public float sensitivity = 2f;
        [Tooltip("Key to activate running")]
        public KeyCode runKey = KeyCode.LeftShift;
        [Tooltip("Key to activate crouching")]
        public KeyCode crouchKey = KeyCode.LeftControl;

        [Header("CROUCH SETTINGS")]
        [Tooltip("Height when crouching")]
        private float crouchHeight = 1.3f;
        [Tooltip("Smooth time for crouch transitions")]
        public float crouchSmoothTime = 0.2f;

        [Header("CAMERA SETTINGS")]
        [Tooltip("Maximum upward look angle")]
        public float maxLookUpAngle = 90f;
        [Tooltip("Maximum downward look angle")]
        public float maxLookDownAngle = -90f;
        [Tooltip("Enable head bobbing effect")]
        public bool enableHeadBob = true;
        [Tooltip("Head bob speed while walking")]
        public float walkBobSpeed = 14f;
        [Tooltip("Head bob amount while walking")]
        public float walkBobAmount = 0.05f;
        [Tooltip("Head bob speed while running")]
        public float runBobSpeed = 18f;
        [Tooltip("Head bob amount while running")]
        public float runBobAmount = 0.03f;

        [Header("CAMERA EFFECTS")]
        [Tooltip("Enable camera tilting when moving")]
        public bool enableCameraTilt = true;
        [Range(0f, 10f), Tooltip("Amount of camera tilt")]
        public float tiltAmount = 2f;
        [Range(1f, 20f), Tooltip("Smoothness of tilt transition")]
        public float tiltSmoothness = 8f;
        [Range(0f, 2f), Tooltip("Tilt multiplier when running")]
        public float runTiltMultiplier = 1.2f;
        [Range(0f, 1f), Tooltip("Tilt multiplier when crouching")]
        public float crouchTiltMultiplier = 0.5f;

        [Header("FOV SETTINGS")]
        [Tooltip("Enable FOV changes when running")]
        public bool enableRunFov = true;
        [Tooltip("Normal field of view")]
        public float normalFov = 60f;
        [Tooltip("Field of view when running")]
        public float runFov = 70f;
        [Range(1f, 20f), Tooltip("Speed of FOV transition")]
        public float fovChangeSpeed = 8f;

        [Header("STANDING DETECTION")]
        [Tooltip("GameObject marker for standing height detection")]
        public GameObject standingHeightMarker;
        [Tooltip("Radius for standing clearance check")]
        public float standingCheckRadius = 0.2f;
        [Tooltip("Layer mask for obstacles detection")]
        public LayerMask obstacleLayerMask = ~0;
        [Tooltip("Minimum clearance required to stand up")]
        public float minStandingClearance = 0.01f;
        [Tooltip("Cooldown between standing checks")]
        public float standCheckCooldown = 0.1f;

        // Private variables
        private Vector3 velocity;
        private Vector3 cameraOriginalLocalPos;
        private float xRotation;
        private float currentTilt;
        private float currentFov;
        private float targetFov;
        private float timer;
        private float originalHeight;
        private float targetHeight;
        private float currentHeightVelocity;
        private float cameraHeightVelocity;
        private float markerHeightOffset;
        private float lastStandCheckTime;
        private float currentMovementSpeed;
        private float defaultYPos;
        private float cameraBaseHeight;
        private float lastGroundedTime;
        private bool isGrounded;
        private bool isCrouching;
        private bool wantsToStand;
        private bool markerInitialized;
        private bool isCrouchKeyHeld;
        private bool wasRunningWhenJumped;
        private MovementState currentMovementState = MovementState.Walking;

        private const float ungroundedDuration = 0.2f;

        /// <summary>
        /// Movement states for the player
        /// </summary>
        public enum MovementState { Walking, Running, Crouching, Jumping }

        // Public properties for external access
        public bool IsGrounded => isGrounded;
        public Vector3 Velocity => velocity;
        public float CurrentSpeed => currentMovementSpeed;

        void Start()
        {
            InitializeController();
        }

        /// <summary>
        /// Initializes the player controller
        /// </summary>
        private void InitializeController()
        {
            Cursor.lockState = CursorLockMode.Locked;
            originalHeight = controller.height;
            targetHeight = originalHeight;
            defaultYPos = playerCamera.localPosition.y;
            cameraBaseHeight = defaultYPos;
            cameraOriginalLocalPos = playerCamera.localPosition;

            // Initialize camera FOV
            if (playerCamera.GetComponent<Camera>() != null)
            {
                currentFov = targetFov = normalFov;
                playerCamera.GetComponent<Camera>().fieldOfView = currentFov;
            }

            // Initialize standing height marker
            if (standingHeightMarker != null)
            {
                markerHeightOffset = standingHeightMarker.transform.position.y - transform.position.y;
                markerInitialized = true;
            }

            currentMovementSpeed = speed;
        }

        void Update()
        {
            bool wasGrounded = isGrounded;
            isGrounded = controller.isGrounded;

            // Update grounded state
            if (isGrounded) lastGroundedTime = Time.time;
            if (!wasGrounded && isGrounded) Land();

            // Reset vertical velocity when grounded
            if (isGrounded && velocity.y < 0) velocity.y = -2f;

            // Handle all controller functions
            HandleCrouching();
            UpdateMovementState();
            HandleMovement();
            HandleControllerHeightAdjustment();
            HandleCameraControl();
            HandleCameraTilt();
            HandleFovChange();

            if (enableHeadBob) HandleHeadBob();
        }

        /// <summary>
        /// Handles landing from jumps or falls
        /// </summary>
        private void Land()
        {
            currentMovementState = isCrouching ? MovementState.Crouching : MovementState.Walking;
            wasRunningWhenJumped = false;
        }

        /// <summary>
        /// Checks if player is effectively grounded (includes coyote time)
        /// </summary>
        private bool IsEffectivelyGrounded()
        {
            return isGrounded || (Time.time - lastGroundedTime <= ungroundedDuration && velocity.y <= 0);
        }

        /// <summary>
        /// Updates the current movement state based on input and conditions
        /// </summary>
        private void UpdateMovementState()
        {
            if (!IsEffectivelyGrounded())
            {
                currentMovementState = MovementState.Jumping;
                return;
            }

            if (isCrouching)
            {
                currentMovementState = MovementState.Crouching;
                currentMovementSpeed = speed * 0.5f;
            }
            else
            {
                bool wantsToRun = Input.GetKey(runKey) && Input.GetAxis("Vertical") > 0.1f;
                currentMovementState = wantsToRun ? MovementState.Running : MovementState.Walking;
                currentMovementSpeed = wantsToRun ? runSpeed : speed;
            }
        }

        /// <summary>
        /// Handles player movement input and physics
        /// </summary>
        private void HandleMovement()
        {
            // Get movement input
            Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
            if (move.magnitude > 1f) move.Normalize();

            // Handle jumping
            if (Input.GetButtonDown("Jump") && IsEffectivelyGrounded() && !isCrouching)
            {
                wasRunningWhenJumped = currentMovementState == MovementState.Running;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                currentMovementState = MovementState.Jumping;
            }

            // Update standing detection marker
            UpdateMarkerPosition();

            // Apply movement
            controller.Move(move * currentMovementSpeed * Time.deltaTime);

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// Handles camera tilting based on movement
        /// </summary>
        private void HandleCameraTilt()
        {
            if (!enableCameraTilt || !IsEffectivelyGrounded())
            {
                currentTilt = Mathf.Lerp(currentTilt, 0f, tiltSmoothness * Time.deltaTime);
                ApplyCameraTilt();
                return;
            }

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            if (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f)
            {
                float targetTilt = -moveX * tiltAmount;

                // Add slight tilt based on mouse movement when moving forward/backward
                if (Mathf.Abs(moveZ) > 0.1f && Mathf.Abs(moveX) < 0.1f)
                    targetTilt = -Input.GetAxis("Mouse X") * tiltAmount * 0.5f;

                // Apply modifiers
                if (currentMovementState == MovementState.Running) targetTilt *= runTiltMultiplier;
                if (isCrouching) targetTilt *= crouchTiltMultiplier;

                currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSmoothness * Time.deltaTime);
            }
            else
            {
                currentTilt = Mathf.Lerp(currentTilt, 0f, tiltSmoothness * Time.deltaTime);
            }

            ApplyCameraTilt();
        }

        /// <summary>
        /// Applies the current tilt to the camera
        /// </summary>
        private void ApplyCameraTilt()
        {
            if (playerCamera == null) return;
            Vector3 rot = playerCamera.localEulerAngles;
            playerCamera.localEulerAngles = new Vector3(rot.x, rot.y, currentTilt);
        }

        /// <summary>
        /// Handles FOV changes based on movement state
        /// </summary>
        private void HandleFovChange()
        {
            if (!enableRunFov || playerCamera.GetComponent<Camera>() == null) return;

            Camera cam = playerCamera.GetComponent<Camera>();
            bool runningFov = IsEffectivelyGrounded() ? currentMovementState == MovementState.Running : wasRunningWhenJumped;
            targetFov = runningFov ? runFov : normalFov;

            // Slower FOV transition in air
            float fovMultiplier = IsEffectivelyGrounded() ? fovChangeSpeed : fovChangeSpeed * 0.5f;
            currentFov = Mathf.Lerp(currentFov, targetFov, fovMultiplier * Time.deltaTime);
            cam.fieldOfView = currentFov;
        }

        /// <summary>
        /// Updates the standing detection marker position
        /// </summary>
        private void UpdateMarkerPosition()
        {
            if (standingHeightMarker != null && markerInitialized)
            {
                standingHeightMarker.transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + markerHeightOffset,
                    transform.position.z
                );
                standingHeightMarker.transform.rotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// Handles crouching input and state
        /// </summary>
        private void HandleCrouching()
        {
            // Crouch key pressed
            if (Input.GetKeyDown(crouchKey))
            {
                isCrouchKeyHeld = true;
                if (!isCrouching)
                {
                    isCrouching = true;
                    wantsToStand = false;
                    targetHeight = crouchHeight;
                    currentMovementState = MovementState.Crouching;
                }
            }

            // Crouch key released
            if (Input.GetKeyUp(crouchKey))
            {
                isCrouchKeyHeld = false;
                if (isCrouching) wantsToStand = true;
            }

            // Attempt to stand up
            if (wantsToStand && !isCrouchKeyHeld && Time.time - lastStandCheckTime > standCheckCooldown)
            {
                lastStandCheckTime = Time.time;
                if (CanStandUp())
                {
                    isCrouching = false;
                    targetHeight = originalHeight;
                    wantsToStand = false;
                }
            }

            // Prevent standing if crouch key is held
            if (isCrouching && isCrouchKeyHeld) wantsToStand = false;
        }

        /// <summary>
        /// Smoothly adjusts the controller height for crouching
        /// </summary>
        private void HandleControllerHeightAdjustment()
        {
            float prevHeight = controller.height;
            float newHeight = Mathf.SmoothDamp(controller.height, targetHeight, ref currentHeightVelocity, crouchSmoothTime);
            float heightDiff = newHeight - prevHeight;
            controller.height = newHeight;

            // Adjust position to prevent sinking into ground
            if (heightDiff > 0) controller.Move(Vector3.up * heightDiff * 0.5f);

            AdjustCameraPosition();
        }

        /// <summary>
        /// Adjusts camera position based on controller height
        /// </summary>
        private void AdjustCameraPosition()
        {
            float camHeight = cameraBaseHeight * (controller.height / originalHeight);
            float newCamHeight = Mathf.SmoothDamp(playerCamera.localPosition.y, camHeight, ref cameraHeightVelocity, crouchSmoothTime);
            playerCamera.localPosition = new Vector3(
                playerCamera.localPosition.x,
                newCamHeight,
                playerCamera.localPosition.z
            );
        }

        /// <summary>
        /// Handles mouse look input
        /// </summary>
        private void HandleCameraControl()
        {
            // Horizontal rotation
            transform.Rotate(0f, Input.GetAxis("Mouse X") * sensitivity, 0f);

            // Vertical rotation with clamping
            xRotation -= Input.GetAxis("Mouse Y") * sensitivity;
            xRotation = Mathf.Clamp(xRotation, maxLookDownAngle, maxLookUpAngle);
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        /// <summary>
        /// Handles head bobbing effect
        /// </summary>
        private void HandleHeadBob()
        {
            if (!IsEffectivelyGrounded())
            {
                // Smoothly return camera to base height when in air
                float headBobHeight = cameraBaseHeight * (controller.height / originalHeight);
                playerCamera.localPosition = new Vector3(
                    playerCamera.localPosition.x,
                    Mathf.Lerp(playerCamera.localPosition.y, headBobHeight, Time.deltaTime * 12f),
                    playerCamera.localPosition.z
                );
                timer = 0;
                return;
            }

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            float headBobBaseHeight = cameraBaseHeight * (controller.height / originalHeight);

            // Apply head bob when moving
            if (Mathf.Abs(moveX) > 0.15f || Mathf.Abs(moveZ) > 0.15f)
            {
                bool isRunning = Input.GetKey(runKey) && !isCrouching && moveZ > 0.1f;

                // Adjust parameters based on state
                float speedMult = isCrouching ? 0.6f : 1f;
                float amountMult = isCrouching ? 0.4f : 1f;

                float bobSpeed = (isRunning ? runBobSpeed : walkBobSpeed) * speedMult;
                float bobAmount = (isRunning ? runBobAmount : walkBobAmount) * amountMult;

                timer += Time.deltaTime * bobSpeed;
                playerCamera.localPosition = new Vector3(
                    playerCamera.localPosition.x,
                    headBobBaseHeight + Mathf.Sin(timer) * bobAmount,
                    playerCamera.localPosition.z
                );
            }
            else
            {
                // Smoothly return to base height when not moving
                timer = 0;
                playerCamera.localPosition = new Vector3(
                    playerCamera.localPosition.x,
                    Mathf.Lerp(playerCamera.localPosition.y, headBobBaseHeight, Time.deltaTime * 8f),
                    playerCamera.localPosition.z
                );
            }
        }

        /// <summary>
        /// Checks if player can stand up from crouching position
        /// </summary>
        private bool CanStandUp()
        {
            if (standingHeightMarker == null || !markerInitialized) return true;

            Collider[] hits = Physics.OverlapSphere(standingHeightMarker.transform.position, standingCheckRadius, obstacleLayerMask);
            foreach (Collider col in hits)
            {
                // Ignore self and children
                if (col.transform == transform || col.transform == standingHeightMarker.transform || col.transform.IsChildOf(transform))
                    continue;

                // Check if obstacle is low enough to prevent standing
                if (col.bounds.min.y < standingHeightMarker.transform.position.y + minStandingClearance)
                    return false;
            }
            return true;
        }

        // Public API methods
        public bool IsCrouching() => isCrouching;
        public MovementState GetMovementState() => currentMovementState;
        public bool WasRunningWhenJumped() => wasRunningWhenJumped;

        /// <summary>
        /// Draws debug gizmos in the editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (standingHeightMarker != null && markerInitialized)
            {
                Gizmos.color = CanStandUp() ? Color.green : Color.red;
                Gizmos.DrawWireSphere(standingHeightMarker.transform.position, standingCheckRadius);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, standingHeightMarker.transform.position);
            }
        }
    }
}
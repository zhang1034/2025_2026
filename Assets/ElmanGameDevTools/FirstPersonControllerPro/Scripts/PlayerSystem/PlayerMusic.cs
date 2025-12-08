using UnityEngine;
using ElmanGameDevTools.PlayerSystem;

namespace ElmanGameDevTools.PlayerAudio
{
    /// <summary>
    /// Handles player footstep sounds based on movement state and surface type
    /// </summary>
    public class PlayerMusic : MonoBehaviour
    {
        [Header("Surface Settings")]
        public string[] surfaceTags; // Array of surface tags for different footstep sounds
        public AudioClip[] footstepClips; // Array of audio clips for different surfaces

        [Header("Audio Settings")]
        public AudioSource audioSource;
        [Range(0f, 1f)]
        public float volume = 1f;

        [Header("Pitch Settings")]
        public float walkPitch = 1.0f;
        public float runPitch = 1.5f;
        public float crouchPitch = 0.7f;

        [Header("References")]
        public PlayerController playerController;

        // Private variables
        private bool isGrounded;
        private bool isRunning = false;
        private const float GROUND_CHECK_DISTANCE = 1.5f;

        /// <summary>
        /// Initializes the audio component and sets up references
        /// </summary>
        private void Start()
        {
            InitializeAudioSource();
            FindPlayerController();
        }

        /// <summary>
        /// Sets up the AudioSource component with default settings
        /// </summary>
        private void InitializeAudioSource()
        {
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.volume = volume;
            audioSource.pitch = walkPitch;
        }

        /// <summary>
        /// Attempts to find PlayerController component if not assigned
        /// </summary>
        private void FindPlayerController()
        {
            if (playerController == null)
            {
                playerController = GetComponent<PlayerController>();

                if (playerController == null)
                {
                    Debug.LogWarning("PlayerController not found. Please assign it manually for better performance.");
                }
            }
        }

        /// <summary>
        /// Main update loop handling footstep sounds based on player state
        /// </summary>
        private void Update()
        {
            isGrounded = CheckIfGrounded();
            UpdateMovementState();
            HandleFootstepSounds();
            UpdateAudioSettings();
        }

        /// <summary>
        /// Updates player movement state (running, crouching, walking)
        /// </summary>
        private void UpdateMovementState()
        {
            bool isActuallyCrouching = playerController != null && playerController.IsCrouching();
            bool wantsToRun = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !isActuallyCrouching;

            // Update running state only if not crouching
            if (wantsToRun && !isRunning && !isActuallyCrouching)
            {
                isRunning = true;
            }
            else if (!wantsToRun && isRunning)
            {
                isRunning = false;
            }

            // Force running to false when crouching
            if (isActuallyCrouching)
            {
                isRunning = false;
            }
        }

        /// <summary>
        /// Handles playback of footstep sounds based on movement
        /// </summary>
        private void HandleFootstepSounds()
        {
            if (!isGrounded)
            {
                StopAudioIfPlaying();
                return;
            }

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

            if (isMoving)
            {
                PlayAppropriateFootstep();
            }
            else
            {
                StopAudioIfPlaying();
            }
        }

        /// <summary>
        /// Plays the appropriate footstep sound based on surface and movement state
        /// </summary>
        private void PlayAppropriateFootstep()
        {
            AudioClip currentClip = GetCurrentSurfaceClip();
            if (currentClip != null && (!audioSource.isPlaying || audioSource.clip != currentClip))
            {
                audioSource.clip = currentClip;
                audioSource.Play();
            }

            UpdatePitchBasedOnMovement();
        }

        /// <summary>
        /// Updates audio pitch based on current movement state
        /// </summary>
        private void UpdatePitchBasedOnMovement()
        {
            bool isActuallyCrouching = playerController != null && playerController.IsCrouching();

            if (isActuallyCrouching)
            {
                audioSource.pitch = crouchPitch;
            }
            else if (isRunning)
            {
                audioSource.pitch = runPitch;
            }
            else
            {
                audioSource.pitch = walkPitch;
            }
        }

        /// <summary>
        /// Stops audio playback if currently playing
        /// </summary>
        private void StopAudioIfPlaying()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        /// <summary>
        /// Updates audio settings like volume in real-time
        /// </summary>
        private void UpdateAudioSettings()
        {
            audioSource.volume = volume;
        }

        /// <summary>
        /// Checks if player is grounded using raycast
        /// </summary>
        /// <returns>True if player is on ground</returns>
        private bool CheckIfGrounded()
        {
            RaycastHit hit;
            return Physics.Raycast(transform.position, Vector3.down, out hit, GROUND_CHECK_DISTANCE);
        }

        /// <summary>
        /// Gets the appropriate audio clip based on current surface
        /// </summary>
        /// <returns>AudioClip for current surface, null if no match found</returns>
        private AudioClip GetCurrentSurfaceClip()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, GROUND_CHECK_DISTANCE))
            {
                int index = System.Array.IndexOf(surfaceTags, hit.collider.tag);
                if (index >= 0 && index < footstepClips.Length)
                {
                    return footstepClips[index];
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the running state manually (for external control)
        /// </summary>
        /// <param name="running">True to set running state</param>
        public void SetRunning(bool running)
        {
            isRunning = running;
        }

        /// <summary>
        /// Returns whether the player is currently running
        /// </summary>
        /// <returns>True if running</returns>
        public bool IsRunning()
        {
            return isRunning;
        }
    }
}
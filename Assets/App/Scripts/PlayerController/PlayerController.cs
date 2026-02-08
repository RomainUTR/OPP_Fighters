using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Title("Configuration")]
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    [Required]
    public PlayerSettingsSO settings;

    [Title("References")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera cam;

    [FoldoutGroup("Runtime Debug Info")]
    [ShowInInspector, ReadOnly]
    float yaw;

    [FoldoutGroup("Runtime Debug Info")]
    [ShowInInspector, ReadOnly]
    float pitch;

    float smoothYaw;
    float smoothPitch;
    float yawSmoothV;
    float pitchSmoothV;

    [FoldoutGroup("Runtime Debug Info")]
    [ShowInInspector, ReadOnly]
    float verticalVelocity;

    [FoldoutGroup("Runtime Debug Info")]
    [ShowInInspector, ReadOnly]
    Vector3 velocity;

    Vector3 smoothV;

    [FoldoutGroup("Runtime Debug Info")]
    [ShowInInspector, ReadOnly]
    bool jumping;

    float lastGroundedTime;

    [FoldoutGroup("Runtime Debug Info")]
    [ShowInInspector]
    bool disabled;

    [FoldoutGroup("Runtime Debug Info")]
    [ShowInInspector, ReadOnly]
    float stepTimer;

    private InputSystem_Actions ctx;

    private void Awake()
    {
        ctx = new InputSystem_Actions();
        RebindStorage.Load(ctx.asset);
    }

    void OnEnable()
    {
        ctx.Enable();
    }

    void OnDisable()
    {
        ctx.Disable();
    }

    void Start()
    {
        cam = Camera.main;
        if (settings.lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        controller = GetComponent<CharacterController>();

        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Break();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            disabled = !disabled;
        }

        if (disabled)
        {
            return;
        }

        Vector2 input = ctx.Controller.Move.ReadValue<Vector2>();

        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir);

        float currentSpeed = (ctx.Controller.Sprint.IsPressed()) ? settings.runSpeed : settings.walkSpeed;
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, settings.smoothMoveTime);

        if (controller.isGrounded && inputDir.sqrMagnitude > 0.01f)
        {
            bool isSpinning = ctx.Controller.Sprint.IsPressed();
            float currentInterval = isSpinning ? settings.runStepInterval : settings.walkStepInterval;

            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayFootstepSound();
                stepTimer = currentInterval;
            }
        } else
        {
            stepTimer = 0;
        }

            verticalVelocity -= settings.gravity * Time.deltaTime;
        velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);

        var flags = controller.Move(velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below)
        {
            jumping = false;
            lastGroundedTime = Time.time;
            verticalVelocity = 0;
        }

        if (ctx.Controller.Jump.WasPressedThisFrame())
        {
            float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
            if (controller.isGrounded || (!jumping && timeSinceLastTouchedGround < 0.15f))
            {
                jumping = true;
                verticalVelocity = settings.jumpForce;
            }
        }

        Vector2 mouseDelta = ctx.Controller.Look.ReadValue<Vector2>();

        float mX = mouseDelta.x * Time.deltaTime;
        float mY = mouseDelta.y * Time.deltaTime;

        yaw += mX * settings.mouseSensitivity;
        pitch -= mY * settings.mouseSensitivity;
        pitch = Mathf.Clamp(pitch, settings.pitchMinMax.x, settings.pitchMinMax.y);
        smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchSmoothV, settings.rotationSmoothTime);
        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawSmoothV, settings.rotationSmoothTime);
    }

    private void LateUpdate()
    {
        if (disabled) return;

        transform.eulerAngles = Vector3.up * smoothYaw;
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;
    }

    void PlayFootstepSound()
    {
        if (settings.soundData != null)
        {
            AudioManager.Instance.PlayClipAt(settings.soundData, transform.position);
        }
    }
}
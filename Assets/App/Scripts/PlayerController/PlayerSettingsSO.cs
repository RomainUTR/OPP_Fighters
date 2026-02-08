using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings_new", menuName = "Player/PlayerSettingsSO")]
public class PlayerSettingsSO : ScriptableObject
{
    [BoxGroup("Movement Settings")]
    public float walkSpeed = 3;

    [BoxGroup("Movement Settings")]
    public float runSpeed = 6;

    [BoxGroup("Movement Settings")]
    [Tooltip("Temps pour atteindre la vitesse cible (inertie)")]
    public float smoothMoveTime = 0.1f;

    [BoxGroup("Physics Settings")]
    public float jumpForce = 8;

    [BoxGroup("Physics Settings")]
    public float gravity = 18;

    [BoxGroup("Camera Settings")]
    public bool lockCursor = true;

    [BoxGroup("Camera Settings")]
    public float mouseSensitivity = 10;

    [BoxGroup("Camera Settings")]
    [MinMaxSlider(-90, 90, true)]
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    [BoxGroup("Camera Settings")]
    public float rotationSmoothTime = 0.1f;

    [BoxGroup("Audio Settings")]
    public SoundData soundData;

    [BoxGroup("Audio Settings")]
    [SuffixLabel("sec", true)]
    public float walkStepInterval = 0.5f;

    [BoxGroup("Audio Settings")]
    [SuffixLabel("sec", true)]
    public float runStepInterval = 0.3f;
}

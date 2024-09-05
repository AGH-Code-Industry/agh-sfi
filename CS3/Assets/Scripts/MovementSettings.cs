using UnityEngine;

namespace DeadFusion.GameCore
{
    [CreateAssetMenu(fileName = "Movement Settings", menuName = "ScriptableObjects/Movement Settings", order = 0)]
    public class MovementSettings : ScriptableObject
    {
        [Header("Movement Speed")]
        [Range(0f, 15f)]
        public float walkSpeed;
        [Range(0f, 15f)]
        public float runSpeed;
        [Range(0f, 15f)]
        public float crouchSpeed;

        [Space()]
        public float stateChangeSmoothness;
        public Vector2 runAxisMultiplier;

        [Range(0f, 90f)]
        public float minimumSlideAngle;
        public float slideForceMultiplier;
        public LayerMask slopeRaycastMask;

        [Header("Camera settings")]
        [Range(0f, 180f)]
        public float normalFOV;
        [Range(0f, 180f)]
        public float runFOV;

        [Header("Jumping")]
        public float gravity;
        public float jumpHeight;

        [Header("Height")]
        public float standHeight;
        public float crouchHeight;
        public float crouchingSpeed;

        [Header("Movement Animations")]
        public AnimationCurve cameraBob;
        public float sprintAmplitudeMultiplier;
        public float bobSpeed;
    }
}
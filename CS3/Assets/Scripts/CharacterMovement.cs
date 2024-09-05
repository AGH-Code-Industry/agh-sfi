using DeadFusion.Utility;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeadFusion.GameCore
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        public BoolOverrideList freeze;
        public BoolOverrideList blockSprint;
        public float freezeTimer;

        public Vector2 moveAxis { get; private set; }
        public float speed { get; private set; }

        public CameraMovement cameraMovement;
        [SerializeField] MovementSettings movementSettings;
        [SerializeField] InputActionAsset inputActions;
        [SerializeField] Transform cameraHolder;
        [SerializeField] CameraFOV cameraFov;

        InputAction moveAction;
        Keyboard keyboard;
        CharacterController controller;
        CameraFOV.FOV_Override fovOverride;

        bool frozen;
        bool isGrounded;

        float fov;
        float gravityVelocity;

        float animTimer = 0;
        float animLerp = 0;

        float normalAngle;
        Vector3 slideDirection;
        Vector3 groundNormal;
        Vector3 groundCross;

        Vector3 _debugSpherecastPosition;
        Vector3 _debugSpherecastPosition2;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            keyboard = InputSystem.GetDevice<Keyboard>();

            moveAction = inputActions.FindAction("MainMovement");
            moveAction.Enable();

            fovOverride = cameraFov.AddOverride(movementSettings.normalFOV, 3, "Character Movement");
            fov = movementSettings.normalFOV;
            animTimer = movementSettings.cameraBob.keys[0].time;
        }

        void Update()
        {
            moveAxis = GetAxis();
            CalculateInfo();

            ApplyCameraAnimations();

            Vector3 movement = GetDirection() * speed * Time.deltaTime;
            Vector3 gravityForce = GetGravityForce();
            //Vector3 crouchingForce = ChangePlayerHeightAndReturnForce();

            if (!frozen) { controller.Move(movement + gravityForce); }
            else { controller.Move(gravityForce); }
        }

        public void TeleportPlayer(Vector3 position, Vector2 rotation)
        {
            freezeTimer = 0.1f;

            transform.position = position;
            cameraMovement.SetRotation(rotation.x, rotation.y);
        }

        public bool IsGrounded()
        {
            return isGrounded;
        }

        public bool IsRunning()
        {
            return (keyboard.leftShiftKey.isPressed && moveAxis.y > 0f) && (controller.height == movementSettings.standHeight) && !blockSprint.isAnyActive && !frozen;
        }

        private Vector2 GetAxis()
        {
            Vector2 newAxis = moveAction.ReadValue<Vector2>();
            if (IsRunning())
            {
                newAxis *= movementSettings.runAxisMultiplier;
            }
            return newAxis;
        }

        private void CalculateInfo()
        {
            CalculateFreeze();
            CalculateSpeed();
            CalculateGroundData();
            CalculateFieldOfView();
            CalculateGravityVelocity();
        }

        private void CalculateGravityVelocity()
        {
            float DEFAULT_GRAVITY_WHEN_GROUNDED = -3.5f;

            if (IsGrounded() && gravityVelocity < 0 && normalAngle < movementSettings.minimumSlideAngle)
            {
                gravityVelocity = DEFAULT_GRAVITY_WHEN_GROUNDED;
            }

            if ((controller.collisionFlags & CollisionFlags.Above) != 0 && gravityVelocity > 0f)
            {
                gravityVelocity = 0f;
            }

            gravityVelocity += movementSettings.gravity * Time.deltaTime;

            if (keyboard.spaceKey.wasPressedThisFrame && IsGrounded() && !frozen)
            {
                gravityVelocity = Mathf.Sqrt(movementSettings.jumpHeight * -2 * movementSettings.gravity);
            }
        }

        private void CalculateFieldOfView()
        {
            float desiredFov = (IsRunning() && IsMoving()) ? movementSettings.runFOV : movementSettings.normalFOV;
            fov = Mathf.Lerp(fov, desiredFov, Time.deltaTime * 8);

            if (frozen)
                fov = movementSettings.normalFOV;

            fovOverride.fov = fov;
        }

        private void CalculateGroundData()
        {
            isGrounded = (controller.collisionFlags & CollisionFlags.Below) != 0;
            if (normalAngle > movementSettings.minimumSlideAngle)
            {
                isGrounded = false;
            }

            float ERROR_MARGIN = 0.02f;

            RaycastHit hit;
            Ray sphereRay = new Ray(transform.position + controller.center, Vector3.down);
            float distance = (controller.height / 2) - controller.radius + ERROR_MARGIN;

            if (Physics.SphereCast(sphereRay, controller.radius, out hit, distance, movementSettings.slopeRaycastMask))
            {
                normalAngle = Vector3.Angle(Vector3.up, hit.normal);
                _debugSpherecastPosition = hit.point + hit.normal * controller.radius;
            }
            else
            {
                normalAngle = 0f;
                _debugSpherecastPosition = Vector3.zero;
            }

            RaycastHit hitStraight;
            Physics.Raycast(transform.position + controller.center, Vector3.down, out hitStraight, controller.height / 2 + 0.15f, movementSettings.slopeRaycastMask);

            groundCross = Vector3.Cross(hit.normal, Vector3.up);
            slideDirection = Vector3.Cross(hit.normal, groundCross);

            groundNormal = hit.normal;
        }

        private void CalculateSpeed()
        {
            float currentSpeed = speed;
            float newSpeed;

            float desiredSpeed = IsRunning() ? movementSettings.runSpeed : movementSettings.walkSpeed;
            newSpeed = Mathf.Lerp(currentSpeed, desiredSpeed, movementSettings.stateChangeSmoothness * Time.deltaTime);
            if (controller.height < movementSettings.standHeight)
            {
                newSpeed = movementSettings.crouchSpeed;
            }

            speed = newSpeed;
        }

        private void CalculateFreeze()
        {
            freezeTimer -= Time.deltaTime;
            bool _freeze = freezeTimer > 0f || freeze.isAnyActive;

            frozen = _freeze;
        }

        private bool IsMoving()
        {
            return moveAxis.x != 0 || moveAxis.y != 0;
        }

        private bool CanStand()
        {
            float ERROR_MARGIN = 0.01f;

            Ray ray = new Ray(transform.position + controller.center, Vector3.up);

            float heightDifference = movementSettings.standHeight - controller.height;
            float distance = ((movementSettings.standHeight / 2) + heightDifference - controller.center.y) - controller.radius;
            distance -= ERROR_MARGIN;

            bool canStand = !Physics.SphereCast(ray, controller.radius, distance, movementSettings.slopeRaycastMask);

            _debugSpherecastPosition2 = (transform.position + controller.center) + Vector3.up * distance;

            return canStand;
        }

        private Vector3 GetDirection()
        {
            Vector3 direction = transform.forward * moveAxis.y + transform.right * moveAxis.x;
            direction = Vector3.ClampMagnitude(direction, 1);

            return direction;
        }

        private Vector3 GetGravityForce()
        {
            Vector3 gravityForce;
            if (normalAngle > movementSettings.minimumSlideAngle)
            {
                gravityForce = -gravityVelocity * slideDirection * Time.deltaTime;
            }
            else
            {
                gravityForce = gravityVelocity * Vector3.up * Time.deltaTime;
            }

            return gravityForce;
        }

        private Vector3 ChangePlayerHeightAndReturnForce()
        {
            float _playerHeight = controller.height;
            bool canStand = CanStand();
            if (keyboard.leftCtrlKey.isPressed)
            {
                _playerHeight = Mathf.MoveTowards(_playerHeight, movementSettings.crouchHeight, Time.deltaTime * movementSettings.crouchingSpeed);
            }
            else if (canStand)
            {
                _playerHeight = Mathf.MoveTowards(_playerHeight, movementSettings.standHeight, Time.deltaTime * movementSettings.crouchingSpeed);
            }

            Vector3 crouchForce = Vector3.zero;

            _playerHeight = Mathf.Clamp(_playerHeight, movementSettings.crouchHeight, movementSettings.standHeight);
            if (IsGrounded())
            {
                float difference = _playerHeight - controller.height;
                if (difference < 0f)
                {
                    crouchForce = Vector3.up * difference * 2;
                }
            }
            controller.center = new Vector3(0f, (movementSettings.standHeight - _playerHeight) / 2);
            controller.height = _playerHeight;

            return crouchForce;
        }

        private void ApplyCameraAnimations()
        {
            if (IsMoving() && IsGrounded() && !frozen)
            {
                animTimer += Time.deltaTime * movementSettings.bobSpeed * speed;
                if (animTimer >= movementSettings.cameraBob.keys[movementSettings.cameraBob.keys.Length - 1].time)
                {
                    animTimer -= movementSettings.cameraBob.keys[movementSettings.cameraBob.keys.Length - 1].time;
                }
                animLerp = Mathf.Lerp(animLerp, movementSettings.cameraBob.Evaluate(animTimer), Time.deltaTime * 10);
            }
            else
            {
                animTimer = movementSettings.cameraBob.keys[movementSettings.cameraBob.keys.Length - 1].time / 2;
                animLerp = Mathf.Lerp(animLerp, 0, Time.deltaTime * 10);
            }
            float amplitudeMultiplier = Mathf.Lerp(1f, movementSettings.sprintAmplitudeMultiplier, Mathf.Clamp01((speed - movementSettings.walkSpeed) / (movementSettings.runSpeed - movementSettings.walkSpeed)));
            cameraHolder.localRotation = Quaternion.Euler(new Vector3(animLerp * 10 * amplitudeMultiplier, cameraHolder.localRotation.eulerAngles.y, cameraHolder.localRotation.eulerAngles.z));
        }

        private void OnDestroy()
        {
            if (fovOverride != null)
                cameraFov.RemoveOverride(fovOverride);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (controller != null)
            {
                Gizmos.DrawWireSphere(_debugSpherecastPosition, controller.radius);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_debugSpherecastPosition2, controller.radius);
            }
        }
#endif
    }
}